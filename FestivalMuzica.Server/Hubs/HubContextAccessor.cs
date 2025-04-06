using Microsoft.AspNetCore.SignalR;
using System;

namespace FestivalMuzica.Server.Hubs
{
    /// <summary>
    /// A class that provides access to the IHubContext after it is available,
    /// solving the circular dependency problem.
    /// </summary>
    public class HubContextAccessor<THub> where THub : Hub
    {
        private IHubContext<THub> _hubContext;

        public IHubContext<THub> HubContext
        {
            get { return _hubContext; }
            set 
            { 
                _hubContext = value;
                HubContextChanged?.Invoke(this, _hubContext);
            }
        }

        public event EventHandler<IHubContext<THub>> HubContextChanged;
    }
} 