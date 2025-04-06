using System;
using System.Collections.ObjectModel;
using FestivalMuzica.Common.Models;
using FestivalMuzica.Client.Service;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Threading;

namespace FestivalMuzica.Client.ViewModels
{
    public class TicketsViewModel : PaginatedViewModel<Ticket>, IDisposable
    {
        private readonly ServiceFestival _service;
        private string _searchText;
        private ObservableCollection<Ticket> _allTickets;
        private bool _isDisposed;

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public TicketsViewModel(ServiceFestival service)
        {
            ItemsPerPage = 4;
            _service = service;
            _allTickets = new ObservableCollection<Ticket>();
            
            try 
            {
                // Make sure FestivalStateService is initialized
                var stateService = FestivalStateService.GetOrInitialize(service, null);
                
                // Subscribe to changes in the shared state service
                stateService.PropertyChanged += OnStatePropertyChanged;
                
                // Initialize with current data
                LoadTickets();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing TicketsViewModel: {ex.Message}");
                // Fallback to direct loading if state service fails
                var tickets = _service.getTickets();
                _allTickets = new ObservableCollection<Ticket>(tickets);
                FilterTickets(SearchText);
            }

            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(text => FilterTickets(text));
        }

        private void OnStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FestivalStateService.Tickets))
            {
                Console.WriteLine("TicketsViewModel: State service notified tickets collection changed");
                
                // Always update on UI thread
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    try
                    {
                        Console.WriteLine("TicketsViewModel: Reloading tickets");
                        LoadTickets();
                        Console.WriteLine($"TicketsViewModel: Tickets reloaded, count: {_allTickets.Count}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"TicketsViewModel: Error reloading tickets - {ex.Message}");
                    }
                });
            }
        }

        public void LoadTickets()
        {
            try
            {
                // Get tickets from shared state instead of service directly
                var stateService = FestivalStateService.GetOrInitialize(_service, null);
                _allTickets = new ObservableCollection<Ticket>(stateService.Tickets);
                FilterTickets(SearchText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tickets from state service: {ex.Message}");
                // Fallback to direct loading
                var tickets = _service.getTickets();
                _allTickets = new ObservableCollection<Ticket>(tickets);
                FilterTickets(SearchText);
            }
        }

        private void FilterTickets(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Items = new ObservableCollection<Ticket>(_allTickets);
            }
            else
            {
                var filtered = _allTickets.Where(t =>
                    t.Id.ToString().Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    t.ShowName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    t.Client.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                Items = new ObservableCollection<Ticket>(filtered);

                // Reset the pagination to the first page
                CurrentPage = 1;
                this.RaisePropertyChanged(nameof(CurrentPage));
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                try
                {
                    // Unsubscribe from state service when view model is disposed
                    var stateService = FestivalStateService.GetOrInitialize(_service, null);
                    stateService.PropertyChanged -= OnStatePropertyChanged;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disposing TicketsViewModel: {ex.Message}");
                }
                _isDisposed = true;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
