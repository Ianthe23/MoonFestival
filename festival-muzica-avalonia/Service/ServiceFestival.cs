using festival_muzica_avalonia.Repository;
using festival_muzica_avalonia.Models;
using System.Collections.Generic;
using System.Linq;
using festival_muzica_avalonia.Repository.Utils;
using System;
namespace festival_muzica_avalonia.Service
{
    public class ServiceFestival : IService<long>
    {
        private IClientRepo? clientDataBaseRepo;
        private IEmployeeRepo? employeeDataBaseRepo;
        private IShowRepo? showDataBaseRepo;
        private ITicketRepo? ticketDataBaseRepo;
        private Employee? _loggedEmployee;

        public Employee? LoggedEmployee => _loggedEmployee;

        private readonly IDictionary<string, string> props;

        public ServiceFestival(IDictionary<string, string> props, IClientRepo clientDataBaseRepo, IEmployeeRepo employeeDataBaseRepo, IShowRepo showDataBaseRepo, ITicketRepo ticketDataBaseRepo)
        {
            this.props = props;
            this.clientDataBaseRepo = clientDataBaseRepo;
            this.employeeDataBaseRepo = employeeDataBaseRepo;
            this.showDataBaseRepo = showDataBaseRepo;
            this.ticketDataBaseRepo = ticketDataBaseRepo;
        }
        public Employee registerEmployee(string username, string password)
        {
            Employee employee = new Employee(username, password);

            Employee? existingEmployee = employeeDataBaseRepo?.FindByUsernameAndPassword(username, password);
            if (existingEmployee != null)
            {
                throw new ServiceException("Employee already exists");
            }
            employeeDataBaseRepo?.Save(employee);

            return employee;
        }
        public Employee loginEmployee(string username, string password)
        {
            Employee? employee = employeeDataBaseRepo?.FindByUsernameAndPassword(username, password);
            if (employee == null)
            {
                throw new RepoException("Invalid username or password");
            }
            _loggedEmployee = employee;
            return employee;
        }

        public List<Client> getClients()
        {
            return clientDataBaseRepo?.FindAll().ToList() ?? new List<Client>();
        }

        public void addClient(Client client)
        {
            if (clientDataBaseRepo == null)
            {
                throw new ServiceException("Client repository is not initialized");
            }
            clientDataBaseRepo.Save(client);
        }
        public List<Show> getShows()
        {
            return showDataBaseRepo?.FindAll().ToList() ?? new List<Show>();
        }

        public List<Ticket> getTickets()
        {
            return ticketDataBaseRepo?.FindAll().ToList() ?? new List<Ticket>();
        }

        public List<Show> getShowsByArtistAndTime(string artist, string time)
        {
            if (showDataBaseRepo == null)
            {
                throw new ServiceException("Show repository is not initialized");
            }
            return showDataBaseRepo.FindByArtistAndTime(artist, time).ToList();
        }

        public void updateShow(Show show)
        {
            if (showDataBaseRepo == null)
            {
                throw new ServiceException("Show repository is not initialized");
            }

            if (showDataBaseRepo.FindOne(show.Id) == null)
            {
                throw new ServiceException("Show not found");
            }

            showDataBaseRepo.Update(show);
        }
        
        public void addTicket(Ticket ticket)
        {
            if (ticketDataBaseRepo == null)
            {
                throw new ServiceException("Ticket repository is not initialized");
            }
            ticketDataBaseRepo.Save(ticket);
        }
        public void sellTicket(Show show, string clientName, string numberOfSeats)
        {
            if (showDataBaseRepo == null)
            {
                throw new ServiceException("Show repository is not initialized");
            }
            if (ticketDataBaseRepo == null)
            {
                throw new ServiceException("Ticket repository is not initialized");
            }

            if (showDataBaseRepo.FindOne(show.Id) == null)
            {
                throw new ServiceException("Show not found");
            }

            if (int.Parse(numberOfSeats) > show.AvailableSeats)
            {
                throw new ServiceException("Not enough available seats");
            }

            Client? client = clientDataBaseRepo?.FindByName(clientName);
            if (client == null)
            {
                client = new Client(clientName);
                addClient(client);
                client = clientDataBaseRepo?.FindByName(clientName);
                if (client == null)
                {
                    throw new ServiceException("Failed to save client");
                }
            }

            Ticket ticket = new Ticket(show.Id, show.Name, client, int.Parse(numberOfSeats), 10);

            IEnumerable<Ticket> existingTickets = ticketDataBaseRepo.FindByShowAndClient(show, client) ?? new List<Ticket>();
            if (existingTickets.Any())
            {
                throw new ServiceException("Client already has a ticket for this show");
            }

            show.AvailableSeats -= int.Parse(numberOfSeats);
            show.SoldSeats += int.Parse(numberOfSeats);
            updateShow(show);
            addTicket(ticket);
        }
    }
}
