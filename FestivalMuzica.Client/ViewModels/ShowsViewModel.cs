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
    public class ShowsViewModel : PaginatedViewModel<Show>, IDisposable
    {
        private readonly ServiceFestival _service;
        private string _searchText;
        private ObservableCollection<Show> _allShows;
        private bool _isDisposed;

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public ShowsViewModel(ServiceFestival service)
        {
            _service = service;
            _allShows = new ObservableCollection<Show>();
            
            try 
            {
                // Make sure FestivalStateService is initialized
                var stateService = FestivalStateService.GetOrInitialize(service, null);
                
                // Subscribe to changes in the shared state service
                stateService.PropertyChanged += OnStatePropertyChanged;
                
                // Add direct subscription to the shows collection for additional UI updates
                stateService.Shows.CollectionChanged += (sender, e) => 
                {
                    Console.WriteLine($"ShowsViewModel: Direct shows collection changed notification received. Action: {e.Action}");
                    Dispatcher.UIThread.InvokeAsync(() => 
                    {
                        LoadShows();
                        this.RaisePropertyChanged(nameof(Items));
                        this.RaisePropertyChanged(nameof(PaginatedItems));
                    });
                };
                
                // Set up a refresh timer to force updates regardless of focus
                var refreshTimer = new System.Timers.Timer(2000); // 2 second refresh
                refreshTimer.Elapsed += (s, e) => {
                    Dispatcher.UIThread.InvokeAsync(() => {
                        try {
                            Console.WriteLine("[SHOWS VIEWMODEL TIMER] Forcing refresh");
                            LoadShows();
                            FilterShows(SearchText);
                            this.RaisePropertyChanged(nameof(Items));
                            this.RaisePropertyChanged(nameof(PaginatedItems));
                        }
                        catch (Exception ex) {
                            Console.WriteLine($"[SHOWS VIEWMODEL TIMER] Error: {ex.Message}");
                        }
                    });
                };
                refreshTimer.AutoReset = true;
                refreshTimer.Start();
                
                // Initialize with current data
                LoadShows();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing ShowsViewModel: {ex.Message}");
                // Fallback to direct loading if state service fails
                var shows = _service.getShows();
                _allShows = new ObservableCollection<Show>(shows);
                FilterShows(SearchText);
            }

            // Setup the reactive search with debounce
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(text => FilterShows(text));
        }

        private void OnStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FestivalStateService.Shows))
            {
                Console.WriteLine("ShowsViewModel: State service notified shows collection changed");
                
                // Always update on UI thread with background priority
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    try
                    {
                        Console.WriteLine("[BACKGROUND UPDATE] ShowsViewModel: Reloading shows");
                        LoadShows();
                        
                        // THIS IS THE KEY CHANGE: Force refresh of the visible items after data has been updated
                        FilterShows(SearchText); // Re-apply current filter to refresh displayed items
                        Console.WriteLine($"[BACKGROUND UPDATE] ShowsViewModel: Shows reloaded and displayed, count: {_allShows.Count}");
                        
                        // Force the UI to refresh by raising change notifications
                        this.RaisePropertyChanged(nameof(Items));
                        this.RaisePropertyChanged(nameof(PaginatedItems));
                        this.RaisePropertyChanged(nameof(TotalPages));
                        
                        // Important: Force refresh of any dependent bound properties
                        foreach (var item in Items)
                        {
                            // Notify property changed for each item to update any bound properties
                            if (item is INotifyPropertyChanged notifyItem)
                            {
                                // Use reflection to force property changed notification on all properties
                                typeof(INotifyPropertyChanged).GetMethod("OnPropertyChanged")?.Invoke(notifyItem, 
                                    new object[] { new PropertyChangedEventArgs(string.Empty) });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[BACKGROUND UPDATE] ShowsViewModel: Error reloading shows - {ex.Message}");
                    }
                }, Avalonia.Threading.DispatcherPriority.Background);
            }
        }

        public void LoadShows()
        {
            try
            {
                // Get shows from shared state instead of service directly
                var stateService = FestivalStateService.GetOrInitialize(_service, null);
                var shows = stateService.Shows.ToList(); // Get a copy to avoid collection modification issues
                
                Console.WriteLine($"ShowsViewModel: Loading {shows.Count} shows from state service");
                _allShows = new ObservableCollection<Show>(shows);
                
                // Apply filtering
                FilterShows(SearchText);
                
                // Explicitly notify UI to update
                this.RaisePropertyChanged(nameof(Items));
                this.RaisePropertyChanged(nameof(PaginatedItems));
                this.RaisePropertyChanged(nameof(TotalPages));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading shows from state service: {ex.Message}");
                // Fallback to direct loading
                var shows = _service.getShows();
                _allShows = new ObservableCollection<Show>(shows);
                FilterShows(SearchText);
            }
        }

        private void FilterShows(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Items = new ObservableCollection<Show>(_allShows);
            }
            else
            {
                var filtered = _allShows.Where(s =>
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
                    Console.WriteLine($"Error disposing ShowsViewModel: {ex.Message}");
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