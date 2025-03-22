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
        public List<Show> getShows()
        {
            return showDataBaseRepo?.FindAll().ToList() ?? new List<Show>();
        }
    }
}
