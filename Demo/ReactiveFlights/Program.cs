#define VERSION1
//#define VERSION2
//#define VERSION3
//#define VERSION4


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;


namespace ReactiveFlights
{
	class Program
	{
		static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();
		static async Task MainAsync(string[] args)
		{
			var from = "SIN";
			var to = "SFO";

			var singaporeAirlines = new FlightPriceChecker("SQ");
			var japanAirlines = new FlightPriceChecker("JL");
			var united = new FlightPriceChecker("UA");

			var runningTasks = new List<Task<IList<FlightPrice>>>
			{
				singaporeAirlines.GetPricesAsync(from, to),
				japanAirlines.GetPricesAsync(from, to),
				united.GetPricesAsync(from, to)
				// ...many more
			};

			/*
			 * This is the original version and not using any Rx.
			 * 
			 */
#if VERSION1
			while (runningTasks.Any())
			{
				// Wait for the first task to finish
				var completed = await Task.WhenAny(runningTasks);

				// Remove from our running list   
				runningTasks.Remove(completed);

				// Process the completed task (updates a property we may be binding to)
				UpdateCheapestFlight(completed.Result);
			}
			Console.WriteLine($"Cheapest flight: {CheapestFlight}");
#endif
			/*
			 * Rx alternative 1: simplifies the original version a lot but cannot be awaited
			 * 
			 */
#if VERSION2
			runningTasks
				// Project tasks into observables
				.Select(getFlightPriceTask => getFlightPriceTask.ToObservable())
				// Merge all tasks into one "lane".
				.Merge()
				.Subscribe(flightPricesPerAirline => UpdateCheapestFlight(flightPricesPerAirline));
#endif

			/*
			 * Rx alternative 2: moves processing out of the subscription and makes code awaitable.
			 *
			 */
#if VERSION3
			await runningTasks
				.Select(getFlightPriceTask => getFlightPriceTask.ToObservable())
				.Merge()
				// Process right here instead of in Subscribe()
				.Do(flightPricesPerAirline => UpdateCheapestFlight(flightPricesPerAirline))
				// This here completes if all tasks have signalled completion.
				.Take(runningTasks.Count);

			Console.WriteLine($"Cheapest flight: {CheapestFlight}");
#endif

			/*
			 * Rx alternative 3: eliminates the helper method which is searching the cheapest flight price
			 *                   and replaces is with Rx. This is the purest Rx version.
			 */
#if VERSION4
			var minFlightPrice = await runningTasks
				.Select(getFlightPriceTask => getFlightPriceTask.ToObservable())
				.Merge()
				// Get airline minimum
				.Select(airlineFlightPrices => airlineFlightPrices.Min())
                // "Take" all found minimums
                .Take(runningTasks.Count)
				// ...and from them the minimum.
				.Min();

			Console.WriteLine($"Cheapest flight: {minFlightPrice}");
#endif

			Console.ReadKey();
		}

		// This may be a property we're binding to.
		public static FlightPrice CheapestFlight { get; set; }

		static void UpdateCheapestFlight(IList<FlightPrice> prices)
		{
			var currentCheapestFlight = prices.Min();
			if (CheapestFlight == null || currentCheapestFlight.Price < CheapestFlight.Price)
			{
				CheapestFlight = currentCheapestFlight;
				Console.WriteLine($"Cheapest flight so far: {CheapestFlight}");
			}
		}
	}
}