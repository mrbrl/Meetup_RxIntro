using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFlights
{
    public class FlightPrice : IComparable<FlightPrice>
    {
        public int Price { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public string Airline { get; set; }

        public int CompareTo(FlightPrice other) => Price.CompareTo(other.Price);

        public override string ToString() => $"{Airline} - {From} to {To}: {Price:C}";
    }

    public class FlightPriceChecker
    {
        public FlightPriceChecker(string airline)
        {
            Airline = airline;
        }

        public string Airline { get; }

        Random _rnd = new Random();

        public async Task<IList<FlightPrice>> GetPricesAsync(string from, string to)
        {
            Console.WriteLine($"Getting flight prices for {Airline} from {from} to {to}...");
            await Task.Delay(_rnd.Next(2000, 10000));

            var mockedData = Enumerable
                .Range(1, _rnd.Next(3, 20))
                .Select(_ => new FlightPrice {
                    Price = _rnd.Next(1000, 2000),
                    From = from,
                    To = to,
                    Airline = Airline
                })
                .ToList();

            var cheapestLocalFlight = mockedData.Min();
            Console.WriteLine($"Done getting prices for {Airline} - cheapest flight: {cheapestLocalFlight}!");
            return mockedData;
        }
    }
}