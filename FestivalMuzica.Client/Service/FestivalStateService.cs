using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using FestivalMuzica.Common.Models;
using Avalonia.Threading;
using System.Threading.Tasks;

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
            
            // Use Task.Run to process the update in the background
            Task.Run(() => {
                try
                {
                    Console.WriteLine($"Background processing: Show update for {updatedShow.Name}");
                    
                    // Update the show directly in the collection without waiting for UI thread
                    var existingShow = _shows.FirstOrDefault(s => s.Id == updatedShow.Id);
                    if (existingShow != null)
                    {
                        // Schedule the UI update with high priority
                        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
                            try {
                                // Update the existing show's properties
                                existingShow.AvailableSeats = updatedShow.AvailableSeats;
                                existingShow.SoldSeats = updatedShow.SoldSeats;
                                existingShow.Name = updatedShow.Name;
                                existingShow.ArtistName = updatedShow.ArtistName;
                                existingShow.Date = updatedShow.Date;
                                existingShow.Location = updatedShow.Location;
                                
                                // Force immediate update of the collection
                                _shows.Remove(existingShow);
                                _shows.Add(existingShow);
                                
                                // Notify that the collection has changed
                                OnPropertyChanged(nameof(Shows));
                                NotifyAllDataChanged();
                                
                                Console.WriteLine($"Show {updatedShow.Name} updated in UI successfully");
                            }
                            catch (Exception ex) {
                                Console.WriteLine($"Error in UI dispatch for show update: {ex.Message}");
                            }
                        }, Avalonia.Threading.DispatcherPriority.Send);
                    }
                    else
                    {
                        // If show doesn't exist, add it with high priority
                        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
                            try {
                                _shows.Add(updatedShow);
                                OnPropertyChanged(nameof(Shows));
                                NotifyAllDataChanged();
                                Console.WriteLine($"Show {updatedShow.Name} added to UI successfully");
                            }
                            catch (Exception ex) {
                                Console.WriteLine($"Error in UI dispatch for show add: {ex.Message}");
                            }
                        }, Avalonia.Threading.DispatcherPriority.Send);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in background processing for show update: {ex.Message}");
                }
            });
        }

        private void OnTicketSold(Ticket newTicket)
        {
            Console.WriteLine($"SignalR notification received: Ticket sold - {newTicket.ShowName}, Client: {newTicket.Client.Name}, Seats: {newTicket.NumberOfSeats}");
            
            // Process in background to ensure it works even when app is in background
            Task.Run(() => {
                try {
                    Console.WriteLine($"Background processing: Ticket sold for {newTicket.ShowName}");
                    
                    // Use high priority to ensure UI updates even in background
                    Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
                        try {
                            // Add the new ticket to the collection
                            _tickets.Add(newTicket);
                            
                            // Update the corresponding show's available seats
                            var show = _shows.FirstOrDefault(s => s.Id == newTicket.ShowId);
                            if (show != null)
                            {
                                show.AvailableSeats -= newTicket.NumberOfSeats;
                                show.SoldSeats += newTicket.NumberOfSeats;
                                
                                // Force immediate update of the show in the collection
                                _shows.Remove(show);
                                _shows.Add(show);
                            }
                            
                            // Notify UI of changes
                            OnPropertyChanged(nameof(Tickets));
                            OnPropertyChanged(nameof(Shows));
                            NotifyAllDataChanged();
                            
                            Console.WriteLine($"Ticket sale for {newTicket.ShowName} processed in UI successfully");
                        }
                        catch (Exception ex) {
                            Console.WriteLine($"Error in UI dispatch for ticket sale: {ex.Message}");
                        }
                    }, Avalonia.Threading.DispatcherPriority.Send);
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error in background processing for ticket sale: {ex.Message}");
                }
            });
        }

        private void OnClientAdded(FestivalMuzica.Common.Models.Client newClient)
        {
            Console.WriteLine($"SignalR notification received: Client added - {newClient.Name}");
            
            // Process in background to ensure it works even when app is in background
            Task.Run(() => {
                try {
                    Console.WriteLine($"Background processing: Client added {newClient.Name}");
                    
                    // Use high priority to ensure UI updates even in background
                    Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
                        try {
                            // Check if client already exists
                            var existingClient = _clients.FirstOrDefault(c => c.Id == newClient.Id);
                            if (existingClient == null)
                            {
                                Console.WriteLine($"Adding new client: {newClient.Name}");
                                _clients.Add(newClient);
                                OnPropertyChanged(nameof(Clients));
                                NotifyAllDataChanged();
                                Console.WriteLine($"Client {newClient.Name} added to UI successfully");
                            }
                            else
                            {
                                Console.WriteLine($"Client already exists: {newClient.Name}");
                            }
                        }
                        catch (Exception ex) {
                            Console.WriteLine($"Error in UI dispatch for client add: {ex.Message}");
                        }
                    }, Avalonia.Threading.DispatcherPriority.Send);
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error in background processing for client add: {ex.Message}");
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
        
    }
} 