using log4net;
using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using FestivalMuzica.Common.Models;
using FestivalMuzica.Server.Repository.Utils;

namespace FestivalMuzica.Server.Repository
{
    public class EmployeeDatabaseRepo : IEmployeeRepo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmployeeDatabaseRepo));
        private readonly IDictionary<string, string> props;

        public EmployeeDatabaseRepo(IDictionary<string, string> props)
        {
            this.props = props ?? throw new ArgumentNullException(nameof(props));
            log.Info("Initializing EmployeeDataBaseRepo");
        }

        public Employee? FindByUsernameAndPassword(string username, string password)
        {
            log.InfoFormat("Entering FindByUsernameAndPassword with username: {0} and password: {1}", username, password);
            IDbConnection con = DBUtils.getConnection(props);
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Employee WHERE username=@username AND password=@password";
                cmd.Parameters.Add(new SQLiteParameter("@username", username));
                cmd.Parameters.Add(new SQLiteParameter("@password", password));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        string usernameRead = dataReader.GetString(1);
                        string passwordRead = dataReader.GetString(2);
                        return new Employee(id, usernameRead, passwordRead);
                    }
                }
            }
            log.InfoFormat("Exiting FindByUsernameAndPassword with username: {0} and password: {1}", username, password);
            return null;
        }

        public Employee? FindOne(long id)
        {
            log.InfoFormat("Finding employee with id {0}", id);
            using var connection = DBUtils.getConnection(props);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Employee WHERE id = @id";
            command.Parameters.Add(new SQLiteParameter("@id", id));
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var employee = new Employee(
                    reader.GetInt64(0),
                    reader.GetString(1),
                    reader.GetString(2)
                );
                log.InfoFormat("Found employee {0}", employee);
                return employee;
            }
            log.InfoFormat("Employee with id {0} not found", id);
            return null;
        }

        public IEnumerable<Employee> FindAll()
        {
            log.Info("Finding all employees");
            using var connection = DBUtils.getConnection(props);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Employee";
            using var reader = command.ExecuteReader();
            var employees = new List<Employee>();
            while (reader.Read())
            {
                var employee = new Employee(
                    reader.GetInt64(0),
                    reader.GetString(1),
                    reader.GetString(2)
                );
                employees.Add(employee);
            }
            log.InfoFormat("Found {0} employees", employees.Count);
            return employees;
        }

        public Employee? Save(Employee entity)
        {
            log.InfoFormat("Entering Save");
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Employee (username, password) VALUES (@username, @password)";
                    cmd.Parameters.Add(new SQLiteParameter("@username", entity.Username));
                    cmd.Parameters.Add(new SQLiteParameter("@password", entity.Password));
                    cmd.ExecuteNonQuery();
                    log.InfoFormat("Exiting Save");
                    return entity;
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error saving entity: {0}", e.Message);
                throw;
            }
        }

        public Employee? Delete(long id)
        {
            log.InfoFormat("Deleting employee with id {0}", id);
            var employee = FindOne(id);
            if (employee == null)
            {
                log.InfoFormat("Employee with id {0} not found", id);
                return null;
            }
            using var connection = DBUtils.getConnection(props);
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Employee WHERE id = @id";
            command.Parameters.Add(new SQLiteParameter("@id", id));
            command.ExecuteNonQuery();
            log.InfoFormat("Deleted employee {0}", employee);
            return employee;
        }

        public Employee? Update(Employee entity)
        {
            log.InfoFormat("Updating employee {0}", entity);
            using var connection = DBUtils.getConnection(props);
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Employee SET username = @username, password = @password WHERE id = @id";
            command.Parameters.Add(new SQLiteParameter("@id", entity.Id));
            command.Parameters.Add(new SQLiteParameter("@username", entity.Username));
            command.Parameters.Add(new SQLiteParameter("@password", entity.Password));
            command.ExecuteNonQuery();
            log.InfoFormat("Updated employee {0}", entity);
            return entity;
        }
    }
}
