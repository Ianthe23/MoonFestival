using System.Collections.ObjectModel;
using System.Linq;
using festival_muzica_avalonia.Models;
using festival_muzica_avalonia.Service;
using System;
using ReactiveUI;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace festival_muzica_avalonia.ViewModels
{
    public class ShowsViewModel : PaginatedViewModel<Show>
    {
        private readonly ServiceFestival _service;
        private string _searchText;
        private ObservableCollection<Show> _filteredItems;

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public ShowsViewModel(ServiceFestival service)
        {
            _service = service;
            LoadShows();

            // Setup the reactive search with debounce
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(text => FilterShows(text));
        }

        public void LoadShows()
        {
            var shows = _service.getShows();
            _filteredItems = new ObservableCollection<Show>(shows);
            Items = new ObservableCollection<Show>(_filteredItems);
        }

        private void FilterShows(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Items = new ObservableCollection<Show>(_filteredItems);
            }
            else
            {
                var filtered = _filteredItems.Where(s =>
                    s.Name.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    s.ArtistName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    s.Location.Contains(text, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                Items = new ObservableCollection<Show>(filtered);

                // Reset the pagination to the first page
                CurrentPage = 1;
                this.RaisePropertyChanged(nameof(CurrentPage));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 