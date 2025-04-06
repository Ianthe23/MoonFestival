using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using Avalonia.Controls;
using FestivalMuzica.Client.Service;
using FestivalMuzica.Client.Views;
using Avalonia.Threading;

// Aliases for model types
using ClientModel = FestivalMuzica.Common.Models.Client;
using Show = FestivalMuzica.Common.Models.Show;
using Ticket = FestivalMuzica.Common.Models.Ticket;
using Employee = FestivalMuzica.Common.Models.Employee;

namespace FestivalMuzica.Client.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ServiceFestival _service;
        private readonly SignalRService _signalRService;
        private Employee? _loggedEmployee;
        private UserControl? _currentView;
        private string _artist = string.Empty;
        private string _time = string.Empty;
        private string _clientName = string.Empty;
        private string _numberOfSeats = string.Empty;
        private Show? _selectedShow;
        private string _connectionStatus = "Disconnected";

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                if (_connectionStatus != value)
                {
                    _connectionStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Artist
        {
            get => _artist;
            set
            {
                if (_artist != value)
                {
                    _artist = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ClientName
        {
            get => _clientName;
            set
            {
                if (_clientName != value)
                {
                    _clientName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NumberOfSeats
        {
            get => _numberOfSeats;
            set
            {
                if (_numberOfSeats != value)
                {
                    _numberOfSeats = value;
                    OnPropertyChanged();
                }
            }
        }

        public Show? SelectedShow
        {
            get => _selectedShow;
            set
            {
                if (_selectedShow != value)
                {
                    _selectedShow = value;
                    OnPropertyChanged();
                }
            }
        }

        public Employee? LoggedEmployee
        {
            get => _loggedEmployee;
            set
            {
                if (_loggedEmployee != value)
                {
                    _loggedEmployee = value;
                    OnPropertyChanged();
                }
            }
        }

        public UserControl? CurrentView
        {
            get => _currentView;
            set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindowViewModel(ServiceFestival service, SignalRService signalRService, Employee loggedEmployee)
        {
            _service = service;
            _signalRService = signalRService;
            LoggedEmployee = loggedEmployee;
            
            // Initialize or get the shared state service
            var stateService = FestivalStateService.GetOrInitialize(service, signalRService);
            
            // When SignalR receives a ticket sold notification, update views
            signalRService.OnTicketSold += ticket => {
                Console.WriteLine($"[BACKGROUND UPDATE] Ticket sold notification received via SignalR: {ticket.ShowName}");
                
                // IMPORTANT: Use a lower priority dispatcher to ensure UI updates even when window is not focused
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => 
                {
                    try
                    {
                        Console.WriteLine("[BACKGROUND UPDATE] Refreshing views after ticket sale");
                        
                        // Create a new instance of each view model to force a complete refresh
                        if (CurrentView is FestivalMuzica.Client.Views.ShowsView)
                        {
                            var newViewModel = new ShowsViewModel(_service);
                            ((FestivalMuzica.Client.Views.ShowsView)CurrentView).DataContext = newViewModel;
                            Console.WriteLine("[BACKGROUND UPDATE] Replaced ShowsView DataContext with new instance");
                        }
                        else if (CurrentView is FestivalMuzica.Client.Views.TicketsView)
                        {
                            var newViewModel = new TicketsViewModel(_service);
                            ((FestivalMuzica.Client.Views.TicketsView)CurrentView).DataContext = newViewModel;
                            Console.WriteLine("[BACKGROUND UPDATE] Replaced TicketsView DataContext with new instance");
                        }
                        
                        // Force a refresh of the state service data
                        var stateService = FestivalStateService.GetOrInitialize(_service, _signalRService);
                        stateService.ForceRefreshAll();
                        stateService.NotifyAllDataChanged();
                        
                        // Force property changed notifications on all view properties
                        OnPropertyChanged(nameof(CurrentView));
                        
                        Console.WriteLine("[BACKGROUND UPDATE] View update complete");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[BACKGROUND UPDATE] Error updating views: {ex.Message}");
                    }
                }, Avalonia.Threading.DispatcherPriority.Background);
            };
            
            // Set up a refresh timer to periodically update data regardless of window focus
            // This is critical for ensuring updates happen even when the window isn't active
            System.Timers.Timer refreshTimer = new System.Timers.Timer(5000); // 5 second refresh interval
            refreshTimer.Elapsed += (s, e) => {
                // Only refresh if SignalR is connected - otherwise we might be disconnected/offline
                if (signalRService.IsConnected) {
                    Console.WriteLine("[TIMER] Performing scheduled background refresh");
                    
                    // Invoke on UI thread at Background priority
                    Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
                        try {
                            // Force data refresh
                            stateService.ForceRefreshAll();
                            
                            // Force UI update
                            if (CurrentView is FestivalMuzica.Client.Views.ShowsView) {
                                var newViewModel = new ShowsViewModel(_service);
                                ((FestivalMuzica.Client.Views.ShowsView)CurrentView).DataContext = newViewModel;
                            }
                            
                            // Force property changed for UI refresh
                            OnPropertyChanged(nameof(CurrentView));
                        }
                        catch (Exception ex) {
                            Console.WriteLine($"[TIMER] Error in refresh timer: {ex.Message}");
                        }
                    }, Avalonia.Threading.DispatcherPriority.Background);
                }
            };
            
            // Start the timer
            refreshTimer.AutoReset = true;
            refreshTimer.Start();
            
            // Add similar handler for show updates
            signalRService.OnShowUpdated += show => {
                Console.WriteLine($"[BACKGROUND UPDATE] Show updated notification received via SignalR: {show.Name}");
                
                // Use background priority to update even when window isn't focused
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => 
                {
                    try 
                    {
                        Console.WriteLine("[BACKGROUND UPDATE] Refreshing views after show update");
                        
                        // Create a new instance of ShowsViewModel if we're on the shows view
                        if (CurrentView is FestivalMuzica.Client.Views.ShowsView)
                        {
                            var newViewModel = new ShowsViewModel(_service);
                            ((FestivalMuzica.Client.Views.ShowsView)CurrentView).DataContext = newViewModel;
                            Console.WriteLine("[BACKGROUND UPDATE] Replaced ShowsView DataContext with new instance");
                        }
                        
                        // Force data refresh
                        var stateService = FestivalStateService.GetOrInitialize(_service, _signalRService);
                        stateService.ForceRefreshAll();
                        stateService.NotifyAllDataChanged();
                        
                        // Force property changed notifications
                        OnPropertyChanged(nameof(CurrentView));
                        
                        Console.WriteLine("[BACKGROUND UPDATE] Show update complete");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[BACKGROUND UPDATE] Error updating views: {ex.Message}");
                    }
                }, Avalonia.Threading.DispatcherPriority.Background);
            };
            
            // Set up ConnectionStatus updates from SignalR
            signalRService.OnConnected += _ => {
                ConnectionStatus = "Connected";
            };
            
            signalRService.OnDisconnected += () => {
                ConnectionStatus = "Disconnected";
            };
            
            // Start the connection
            _ = signalRService.StartConnectionAsync();
            
            // Create and set the view
            CurrentView = new FestivalMuzica.Client.Views.ShowsView { DataContext = new ShowsViewModel(_service) };
            Console.WriteLine("ShowsView displayed");
        }

        public void LoadShows()
        {
            // Refresh the shared state
            FestivalStateService.Instance.RefreshShows();
        }

        public void LoadTickets()
        {
            // Refresh the shared state
            FestivalStateService.Instance.RefreshTickets();
        }

        public void LoadClients()
        {
            // Refresh the shared state
            FestivalStateService.Instance.RefreshClients();
        }

        public void ShowShowsView()
        {
            // Force refresh the data
            FestivalStateService.GetOrInitialize(_service, _signalRService).RefreshShows();
            
            // Create and set the view
            CurrentView = new FestivalMuzica.Client.Views.ShowsView { DataContext = new ShowsViewModel(_service) };
            Console.WriteLine("ShowsView displayed");
        }

        public void ShowTicketsView()
        {
            // Force refresh the data
            FestivalStateService.GetOrInitialize(_service, _signalRService).RefreshTickets();
            
            // Create and set the view
            CurrentView = new FestivalMuzica.Client.Views.TicketsView { DataContext = new TicketsViewModel(_service) };
            Console.WriteLine("TicketsView displayed");
        }

        public void ShowClientsView()
        {
            // Force refresh the data
            FestivalStateService.GetOrInitialize(_service, _signalRService).RefreshClients();
            
            // Create and set the view
            CurrentView = new FestivalMuzica.Client.Views.ClientsView { DataContext = new ClientsViewModel(_service) };
            Console.WriteLine("ClientsView displayed");
        }

        public void ShowShowsByArtistTimeView(string artist, string time)
        {
            // Create a new ViewModel each time with the search parameters
            var viewModel = new ShowsArtistTimeModel(_service, artist, time);
            CurrentView = new FestivalMuzica.Client.Views.ShowsByArtistTimeView { DataContext = viewModel };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}