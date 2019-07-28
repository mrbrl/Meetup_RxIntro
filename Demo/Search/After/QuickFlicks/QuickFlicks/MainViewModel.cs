using QuickFlicks.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace QuickFlicks.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IReadOnlyList<Movie> _movies;
        public IReadOnlyList<Movie> Movies
        {
            get => _movies;
            private set
            {
                _movies = value;
                RaisePropertyChanged(nameof(Movies));
            }
        }

        private string searchTerm;
        public string SearchTerm
        {
            get => searchTerm;
            set
            {
                if (searchTerm != value)
                {
                    searchTerm = value;
                    RaisePropertyChanged();
                    
                    //OnSearchTermChangedAsync(searchTerm)
                    //    .ContinueWith(tr => throw new Exception("Search Failed.", tr.Exception),
                    //    TaskContinuationOptions.OnlyOnFaulted);
                }
            }
        }

        //private CancellationTokenSource cts;
        //private async Task OnSearchTermChangedAsync(string searchTerm)
        //{
        //    Debug.WriteLine($"*** Searching for {searchTerm}");

        //    cts?.Cancel();

        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        var innerToken = cts = new CancellationTokenSource();

        //        var movieService = new MovieService();
        //        var movies = await movieService.GetMoviesForSearchAsync(searchTerm);
        //        if (!innerToken.IsCancellationRequested)
        //        {
        //            Movies = movies;
        //        }
        //    }
        //    else
        //    {
        //        Movies = null;
        //    }
        //}

        public MainViewModel()
        {
            // 1. need to known when text field in ui changes

            var movieService = new MovieService();

            Observable.FromEventPattern<PropertyChangedEventArgs>(this, nameof(PropertyChanged))
                .Where(x => x.EventArgs.PropertyName == nameof(SearchTerm))

                // 2 Slow It Down
                .Throttle(TimeSpan.FromMilliseconds(1000))
                .Select(
                    // turn async method into observable
                    _ => Observable.FromAsync(async ct =>
                    {
                        var movies = await movieService.GetMoviesForSearchAsync(searchTerm).ConfigureAwait(false);

                        if (ct.IsCancellationRequested)
                        {
                            movies = new List<Movie>();
                        }

                        return movies;
                    })
                )
                // Observes current obs until new one, and cancel previous
                .Switch()
                .Subscribe(foundMovies => Movies = foundMovies);

        }
    }
}
