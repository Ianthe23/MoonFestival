using System;
using System.Collections.ObjectModel;
using FestivalMuzica.Common.Models;
using FestivalMuzica.Client.Service;
using System.Windows.Input;
using System.Reactive.Linq;
using ReactiveUI;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Threading;

namespace FestivalMuzica.Client.ViewModels;

public class ClientsViewModel : PaginatedViewModel<FestivalMuzica.Common.Models.Client>, IDisposable
{
    private readonly ServiceFestival _service;
    private string _searchText;
    private ObservableCollection<FestivalMuzica.Common.Models.Client> _allClients;
    private bool _isDisposed;

    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public ClientsViewModel(ServiceFestival service)
    {
        ItemsPerPage = 7;
        _service = service;
        _allClients = new ObservableCollection<FestivalMuzica.Common.Models.Client>();
        
        try 
        {
            // Make sure FestivalStateService is initialized
            var stateService = FestivalStateService.GetOrInitialize(service, null);
            
            // Subscribe to changes in the shared state service
            stateService.PropertyChanged += OnStatePropertyChanged;
            
            // Initialize with current data
            LoadClients();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing ClientsViewModel: {ex.Message}");
            // Fallback to direct loading if state service fails
            var clients = _service.getClients();
            _allClients = new ObservableCollection<FestivalMuzica.Common.Models.Client>(clients);
            FilterClients(SearchText);
        }

        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(text => FilterClients(text));
    }
    
    private void OnStatePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FestivalStateService.Clients))
        {
            Console.WriteLine("ClientsViewModel: State service notified clients collection changed");
            
            // Always update on UI thread
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    Console.WriteLine("ClientsViewModel: Reloading clients");
                    LoadClients();
                    Console.WriteLine($"ClientsViewModel: Clients reloaded, count: {_allClients.Count}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ClientsViewModel: Error reloading clients - {ex.Message}");
                }
            });
        }
    }

    public void LoadClients()
    {
        try
        {
            // Get clients from shared state instead of service directly
            var stateService = FestivalStateService.GetOrInitialize(_service, null);
            _allClients = new ObservableCollection<FestivalMuzica.Common.Models.Client>(stateService.Clients);
            FilterClients(SearchText);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading clients from state service: {ex.Message}");
            // Fallback to direct loading
            var clients = _service.getClients();
            _allClients = new ObservableCollection<FestivalMuzica.Common.Models.Client>(clients);
            FilterClients(SearchText);
        }
    }

    private void FilterClients(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Items = new ObservableCollection<FestivalMuzica.Common.Models.Client>(_allClients);
        }
        else
        {
            var filtered = _allClients.Where(c =>
                c.Id.ToString().Contains(text, StringComparison.OrdinalIgnoreCase) ||
                c.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
            ).ToList();
            Items = new ObservableCollection<FestivalMuzica.Common.Models.Client>(filtered);

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
                Console.WriteLine($"Error disposing ClientsViewModel: {ex.Message}");
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

