using System.Collections.ObjectModel;
using festival_muzica_avalonia.Models;
using festival_muzica_avalonia.Service;

namespace festival_muzica_avalonia.ViewModels;

public class ClientsViewModel : PaginatedViewModel<Client>
{
    private readonly ServiceFestival _service;

    public ClientsViewModel(ServiceFestival service)
    {
        _service = service;
        LoadClients();
    }

    public void LoadClients()
    {
        var clients = _service.getClients();
        Items = new ObservableCollection<Client>(clients);
    }
}

