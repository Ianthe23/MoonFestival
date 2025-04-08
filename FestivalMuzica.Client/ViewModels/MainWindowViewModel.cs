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
            
            // Get or initialize the state service
            var stateService = FestivalStateService.GetOrInitialize(service, signalRService);
            
            // Connect SignalR events directly to state service methods
            signalRService.OnTicketSold += ticket => {
                Console.WriteLine($"[CLIENT {LoggedEmployee?.Username}] Ticket sold notification received via SignalR: {ticket.ShowName}");
                
                // Directly refresh the state - no need to check current view
                stateService.RefreshShows();
                stateService.RefreshTickets();
            };
            
            signalRService.OnShowUpdated += show => {
                Console.WriteLine($"[CLIENT {LoggedEmployee?.Username}] Show updated notification received via SignalR: {show.Name}");
                
                // Directly refresh the state - no need to check current view
                stateService.RefreshShows();
            };
            
            // Set up ConnectionStatus updates from SignalR
            signalRService.OnConnected += _ => {
                Dispatcher.UIThread.Post(() => {
                    ConnectionStatus = "Connected";
                });
            };
            
            signalRService.OnDisconnected += () => {
                Dispatcher.UIThread.Post(() => {
                    ConnectionStatus = "Disconnected";
                });
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