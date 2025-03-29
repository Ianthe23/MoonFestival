using System.Collections.ObjectModel;
using System.Linq;
using festival_muzica_avalonia.Models;
using festival_muzica_avalonia.Service;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace festival_muzica_avalonia.ViewModels
{
    public class TicketsViewModel : PaginatedViewModel<Ticket>
    {
        private readonly ServiceFestival _service;
        private string _searchText;
        private ObservableCollection<Ticket> _filteredItems;

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public TicketsViewModel(ServiceFestival service)
        {
            ItemsPerPage = 4;
            _service = service;
            LoadTickets();

            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(text => FilterTickets(text));
        }

        public void LoadTickets()
        {
            var tickets = _service.getTickets();
            _filteredItems = new ObservableCollection<Ticket>(tickets);
            Items = new ObservableCollection<Ticket>(_filteredItems);
        }

        private void FilterTickets(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Items = new ObservableCollection<Ticket>(_filteredItems);
            }
            else
            {
                var filtered = _filteredItems.Where(t =>
                    t.Id.ToString().Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    t.ShowName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    t.client.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                Items = new ObservableCollection<Ticket>(filtered);

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
