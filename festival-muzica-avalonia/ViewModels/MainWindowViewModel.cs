using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using festival_muzica_avalonia.Models;
using festival_muzica_avalonia.Service;
using festival_muzica_avalonia.Views;
using Avalonia.Controls;

namespace festival_muzica_avalonia.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ServiceFestival _service;
        private Employee? _loggedEmployee;
        private UserControl? _currentView;
        private string _artist = string.Empty;
        private string _time = string.Empty;
        private string _clientName = string.Empty;
        private string _numberOfSeats = string.Empty;
        private Show? _selectedShow;
        private ObservableCollection<Show> _shows = new();
        private ObservableCollection<Ticket> _tickets = new();
        private ObservableCollection<Client> _clients = new();

        public ObservableCollection<Show> Shows
        {
            get => _shows;
            set
            {
                if (_shows != value)
                {
                    _shows = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Ticket> Tickets
        {
            get => _tickets;
            set
            {
                if (_tickets != value)
                {
                    _tickets = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set
            {
                if (_clients != value)
                {
                    _clients = value;
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

        public MainWindowViewModel(ServiceFestival service, Employee loggedEmployee)
        {
            _service = service;
            LoggedEmployee = loggedEmployee;
            LoadShows();
            LoadTickets();
            LoadClients();
            ShowShowsView();
        }

        public void LoadShows()
        {
            var shows = _service.getShows();
            Shows = new ObservableCollection<Show>(shows);
        }

        public void LoadTickets()
        {
            var tickets = _service.getTickets();
            Tickets = new ObservableCollection<Ticket>(tickets);
        }

        public void LoadClients()
        {
            var clients = _service.getClients();
            Clients = new ObservableCollection<Client>(clients);
        }

        public void ShowShowsView()
        {
            CurrentView = new ShowsView { DataContext = new ShowsViewModel(_service) };
        }

        public void ShowTicketsView()
        {
             CurrentView = new TicketsView { DataContext = new TicketsViewModel(_service) };
        }

        public void ShowClientsView()
        {
            CurrentView = new ClientsView { DataContext = new ClientsViewModel(_service) };
        }

        public void ShowShowsByArtistTimeView(string artist, string time)
        {
            CurrentView = new ShowsByArtistTimeView { DataContext = new ShowsArtistTimeModel(_service, artist, time) };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}