using System.Collections.ObjectModel;
using festival_muzica_avalonia.Models;
using festival_muzica_avalonia.Service;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace festival_muzica_avalonia.ViewModels;

public class ClientsViewModel : PaginatedViewModel<Client>
{
    private readonly ServiceFestival _service;
    private string _searchText;
    private ObservableCollection<Client> _filteredItems;

    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);

    }

    public ClientsViewModel(ServiceFestival service)
    {
        ItemsPerPage = 7;
        _service = service;
        LoadClients();

        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(text => FilterClients(text));
    }

    public void LoadClients()
    {
        var clients = _service.getClients();
        _filteredItems = new ObservableCollection<Client>(clients);
        Items = new ObservableCollection<Client>(_filteredItems);
    }

    private void FilterClients(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Items = new ObservableCollection<Client>(_filteredItems);
        }
        else
        {
            var filtered = _filteredItems.Where(c =>
                c.Id.ToString().Contains(text, StringComparison.OrdinalIgnoreCase) ||
                c.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
            ).ToList();
            Items = new ObservableCollection<Client>(filtered);

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

