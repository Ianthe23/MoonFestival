using System.Collections.ObjectModel;
using festival_muzica_avalonia.Models;
using festival_muzica_avalonia.Service;

namespace festival_muzica_avalonia.ViewModels
{
    public class ShowsViewModel : PaginatedViewModel<Show>
    {
        private readonly ServiceFestival _service;

        public ShowsViewModel(ServiceFestival service)
        {
            _service = service;
            LoadShows();
        }

        public void LoadShows()
        {
            var shows = _service.getShows();
            Items = new ObservableCollection<Show>(shows);
        }
    }
} 