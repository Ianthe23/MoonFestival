using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using System.Data.SQLite;
using System.Configuration;
using festival_muzica.repository;
using log4net.Core;

namespace festival_muzica
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static string? GetConnectionStringByName(string name)
        {
            string? returnValue = null;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];

            if (settings != null)
            {
                returnValue = settings.ConnectionString;
            }

            return returnValue;
        }

        static void Main(string[] args)
        {
            // Clear existing logs
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            string logsPath = Path.Combine(projectRoot, "logs");
            
            if (Directory.Exists(logsPath))
            {
                foreach (var file in Directory.GetFiles(logsPath, "*.log"))
                {
                    File.Delete(file);
                }
            }
            
            Directory.CreateDirectory(logsPath);
            GlobalContext.Properties["LogPath"] = logsPath;

            // Configure log4net
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            string configFilePath = Path.Combine(baseDir, "log4net.config");
            log.Info($"Loading log4net configuration from: {configFilePath}");
            
            if (!File.Exists(configFilePath)) {
                log.Error($"Configuration file not found at: {configFilePath}");
                return;
            }
            
            XmlConfigurator.Configure(logRepository, new FileInfo(configFilePath));
            log.Info("Successfully configured log4net");

            // Test logging
            log.Debug("Debug message - testing log4net configuration");
            log.Info("Info message - testing log4net configuration");
            log.Warn("Warning message - testing log4net configuration");
            log.Error("Error message - testing log4net configuration");

            // Create database tables
            CreateDatabase.CreateTables();

            log.Info("Starting the application...");

            IDictionary<String, string> props = new Dictionary<String, string>();
            string? connectionString = GetConnectionStringByName("festival-csharp");
            if (connectionString == null)
            {
                throw new InvalidOperationException("Connection string not found in configuration");
            }
            props.Add("ConnectionString", connectionString);
            Console.WriteLine("Connection string: {0}", props["ConnectionString"]);

            IEmployeeRepo employeeRepo = new EmployeeDataBaseRepo(props);
            IShowRepo showRepo = new ShowDataBaseRepo(props);
            ITicketRepo ticketRepo = new TicketDataBaseRepo(props);
            IClientRepo clientRepo = new ClientDataBaseRepo(props);

            // employeeRepo.Save(new Employee(1, "Ionescu Mihai", "mihai101", "1234567890"));
            // employeeRepo.Save(new Employee(2, "Popescu Ana", "ana23", "0987654321"));

            // showRepo.Save(new Show(1, "Bon-Jovi", "Jon Bon Jovi", DateTime.Parse("2025-03-17 10:00:00"), "Bucharest", 300000, 150000));
            // showRepo.Save(new Show(2, "Coldplay", "Chris Martin", DateTime.Parse("2025-03-18 10:00:00"), "Bucharest", 300000, 150000));

            // Client client1 = new Client(1, "Alexe Maria");
            // Client client2 = new Client(2, "Rosu Diana");

            // clientRepo.Save(client1);
            // clientRepo.Save(client2);

            // ticketRepo.Save(new Ticket(1, client1, 3, 100));
            // ticketRepo.Save(new Ticket(2, client2, 10, 100));

            // ticketRepo.Delete(1);
            // ticketRepo.Delete(2);
            // ticketRepo.Delete(3);
            // ticketRepo.Delete(4);

            var employees = employeeRepo.FindAll();
            foreach (var employee in employees)
            {
                Console.WriteLine(employee);
            }

            var shows = showRepo.FindAll();
            foreach (var show in shows)
            {
                Console.WriteLine(show);
            }

            var tickets = ticketRepo.FindAll();
            foreach (var ticket in tickets)
            {
                Console.WriteLine(ticket);
            }

            var clients = clientRepo.FindAll();
            foreach (var client in clients)
            {
                Console.WriteLine(client);
            }

            log.Info("Application ended.");
    
        }
    }
}
