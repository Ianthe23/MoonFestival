using System.Collections.ObjectModel;
using System.Linq;
using festival_muzica_avalonia.Models;
using festival_muzica_avalonia.Service;

namespace festival_muzica_avalonia.ViewModels
{
    public class TicketsViewModel : PaginatedViewModel<Ticket>
    {
        private readonly ServiceFestival _service;

        public TicketsViewModel(ServiceFestival service)
        {
            ItemsPerPage = 4;
            _service = service;
            LoadTickets();
        }

        public void LoadTickets()
        {
            var tickets = _service.getTickets();
            Items = new ObservableCollection<Ticket>(tickets);
        }
    }
}
