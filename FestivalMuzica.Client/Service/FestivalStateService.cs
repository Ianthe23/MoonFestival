using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using FestivalMuzica.Common.Models;
using Avalonia.Threading;

namespace FestivalMuzica.Client.Service
{
    /// <summary>
    /// A singleton service that maintains the state of festival data and
    /// provides observable collections that automatically notify subscribers of changes.
    /// </summary>
    public class FestivalStateService : INotifyPropertyChanged
    {
        private static FestivalStateService _instance;
        private static readonly object _lock = new object();
        private readonly ServiceFestival _serviceFestival;

        private ObservableCollection<Show> _shows = new();
        private ObservableCollection<Ticket> _tickets = new();
        private ObservableCollection<FestivalMuzica.Common.Models.Client> _clients = new();

        public event PropertyChangedEventHandler PropertyChanged;

        // Singleton pattern
        public static FestivalStateService Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("FestivalStateService must be initialized with Initialize() before accessing Instance");
                }
                return _instance;
            }
        }

        // Collections accessible to view models
        public ObservableCollection<Show> Shows => _shows;
        public ObservableCollection<Ticket> Tickets => _tickets;
        public ObservableCollection<FestivalMuzica.Common.Models.Client> Clients => _clients;

        /// <summary>
        /// Checks if the FestivalStateService has been initialized.
        /// </summary>
        public static bool IsInitialized => _instance != null;

        /// <summary>
        /// Gets the instance if it has been initialized, or creates a new instance using the provided services.
        /// </summary>
        public static FestivalStateService GetOrInitialize(ServiceFestival serviceFestival, SignalRService signalRService)
        {
            if (_instance == null)
            {
                Initialize(serviceFestival, signalRService);
            }
            return _instance;
        }

        private FestivalStateService(ServiceFestival serviceFestival, SignalRService signalRService)
        {
            _serviceFestival = serviceFestival;

            // Set up SignalR event handlers
            signalRService.OnShowUpdated += OnShowUpdated;
            signalRService.OnTicketSold += OnTicketSold;
            signalRService.OnClientAdded += OnClientAdded;

            // Initial data load
            RefreshAll();
        }

        public static void Initialize(ServiceFestival serviceFestival, SignalRService signalRService)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new FestivalStateService(serviceFestival, signalRService);
                }
            }
        }

        public void RefreshAll()
        {
            RefreshShows();
            RefreshTickets();
            RefreshClients();
        }

        public void RefreshShows()
        {
            var shows = _serviceFestival.getShows();
            _shows.Clear();
            foreach (var show in shows)
            {
                _shows.Add(show);
            }
            OnPropertyChanged(nameof(Shows));
        }

        public void RefreshTickets()
        {
            var tickets = _serviceFestival.getTickets();
            _tickets.Clear();
            foreach (var ticket in tickets)
            {
                _tickets.Add(ticket);
            }
            OnPropertyChanged(nameof(Tickets));
        }

        public void RefreshClients()
        {
            var clients = _serviceFestival.getClients();
            _clients.Clear();
            foreach (var client in clients)
            {
                _clients.Add(client);
            }
            OnPropertyChanged(nameof(Clients));
        }

        public void ForceRefreshAll()
        {
            Console.WriteLine("FestivalStateService: Force refreshing all data");
            RefreshShows();
            RefreshTickets();
            RefreshClients();
            
            // Notify UIs that all data has changed
            NotifyAllDataChanged();
        }

        private void OnShowUpdated(Show updatedShow)
        {
            Console.WriteLine($"SignalR notification received: Show updated - {updatedShow.Name} (Available: {updatedShow.AvailableSeats}, Sold: {updatedShow.SoldSeats})");
            
            // First, update the show directly for immediate UI update
            UpdateShowDirectly(updatedShow);
            
            // Use background priority to update even when window is not in focus
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    // For deeper changes, do a complete refresh of the collection
                    Console.WriteLine("[BACKGROUND UPDATE] Performing full refresh of shows collection");
                    RefreshShows();
                    
                    // Force UI to update by notifying property changed
                    OnPropertyChanged(nameof(Shows));
                    
                    // Broadcast to anyone listening to this collection
                    NotifyAllDataChanged();
                    
                    Console.WriteLine($"[BACKGROUND UPDATE] Show update complete. Collection now has {_shows.Count} shows.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BACKGROUND UPDATE] Error updating shows on SignalR notification: {ex.Message}");
                }
            }, Avalonia.Threading.DispatcherPriority.Background);
        }

        private void OnTicketSold(Ticket newTicket)
        {
            Console.WriteLine($"SignalR notification received: Ticket sold - {newTicket.ShowName}, Client: {newTicket.Client.Name}, Seats: {newTicket.NumberOfSeats}");
            
            // Use background priority to update even when window is not in focus
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    // Force a complete refresh of all data
                    Console.WriteLine("[BACKGROUND UPDATE] Performing full refresh of ticket collection and related data");
                    RefreshTickets();
                    RefreshShows(); // Shows are affected by ticket sales (available seats)
                    
                    // Force UI to update
                    OnPropertyChanged(nameof(Tickets));
                    OnPropertyChanged(nameof(Shows));
                    
                    // Ticket sale affects multiple views, so notify all
                    NotifyAllDataChanged();
                    
                    Console.WriteLine($"[BACKGROUND UPDATE] Ticket update complete. Collection now has {_tickets.Count} tickets.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BACKGROUND UPDATE] Error updating tickets on SignalR notification: {ex.Message}");
                }
            }, Avalonia.Threading.DispatcherPriority.Background);
        }

        private void OnClientAdded(FestivalMuzica.Common.Models.Client newClient)
        {
            Console.WriteLine($"SignalR notification received: Client added - {newClient.Name}");
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Check if client already exists
                var existingClient = _clients.FirstOrDefault(c => c.Id == newClient.Id);
                if (existingClient == null)
                {
                    Console.WriteLine($"Adding new client: {newClient.Name}");
                    _clients.Add(newClient);
                    OnPropertyChanged(nameof(Clients));
                }
                else
                {
                    Console.WriteLine($"Client already exists: {newClient.Name}");
                }
            });
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Add this method to trigger UI updates across all views
        public void NotifyAllDataChanged()
        {
            Console.WriteLine("FestivalStateService: Notifying all data changed");
            OnPropertyChanged(nameof(Shows));
            OnPropertyChanged(nameof(Tickets));
            OnPropertyChanged(nameof(Clients));
        }

        // Add this method to force UI updates by directly modifying the collection
        public void UpdateShowDirectly(Show updatedShow)
        {
            Console.WriteLine($"[DIRECT UPDATE] Updating show directly: {updatedShow.Name}");
            
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    // Find the existing show in the collection
                    var existingShow = _shows.FirstOrDefault(s => s.Id == updatedShow.Id);
                    if (existingShow != null)
                    {
                        // Get the index and replace it directly
                        int index = _shows.IndexOf(existingShow);
                        
                        // Make a clean copy to ensure it's treated as a new object
                        var showCopy = new Show
                        {
                            Id = updatedShow.Id,
                            Name = updatedShow.Name,
                            ArtistName = updatedShow.ArtistName,
                            Date = updatedShow.Date,
                            Location = updatedShow.Location,
                            AvailableSeats = updatedShow.AvailableSeats,
                            SoldSeats = updatedShow.SoldSeats
                        };
                        
                        // Replace the show in the collection
                        _shows[index] = showCopy;
                        
                        Console.WriteLine($"[DIRECT UPDATE] Successfully replaced show at index {index}");
                        
                        // Force UI update by notifying property changed
                        OnPropertyChanged(nameof(Shows));
                    }
                    else
                    {
                        // If show doesn't exist, add it
                        _shows.Add(updatedShow);
                        Console.WriteLine($"[DIRECT UPDATE] Added new show to collection");
                        
                        // Force UI update
                        OnPropertyChanged(nameof(Shows));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DIRECT UPDATE] Error updating show directly: {ex.Message}");
                }
            }, Avalonia.Threading.DispatcherPriority.Background);
        }
    }
} 