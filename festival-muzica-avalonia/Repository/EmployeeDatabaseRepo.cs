using log4net;
using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using festival_muzica_avalonia.Repository.Utils;
using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Repository
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
            log.InfoFormat("Entering FindOne with id: {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Employee WHERE id=@id";
                cmd.Parameters.Add(new SQLiteParameter("@id", id));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        long idRead = dataReader.GetInt64(0);
                        string username = dataReader.GetString(1);
                        string password = dataReader.GetString(2);
                        return new Employee(idRead, username, password);
                    }   
                }
            }
            log.InfoFormat("Exiting FindOne with id: {0}", id);
            return null;
        }

        public IEnumerable<Employee> FindAll()
        {
            log.InfoFormat("Entering FindAll");
            IDbConnection con = DBUtils.getConnection(props);
            IList<Employee> employees = new List<Employee>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Employee";
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        string username = dataReader.GetString(1);
                        string password = dataReader.GetString(2);
                        employees.Add(new Employee(id, username, password));
                        log.InfoFormat("Found employee with id: {0}", id);
                    }
                }
            }
            log.InfoFormat("Exiting FindAll with {0} employees", employees.Count);
            return employees;
        }

        public Employee? Save(Employee entity)
        {
            log.InfoFormat("Entering Save with id: {0}", entity.Id);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Employee (username, password) VALUES (@username, @password)";
                    cmd.Parameters.Add(new SQLiteParameter("@username", entity.Username));
                    cmd.Parameters.Add(new SQLiteParameter("@password", entity.Password));
                    cmd.ExecuteNonQuery();
                    log.InfoFormat("Exiting Save with id: {0}", entity.Id);
                    return entity;
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error saving entity with id: {0} and error: {1}", entity.Id, e.Message); 
                throw;
            }
        }

        public Employee? Delete(long id)
        {
            log.InfoFormat("Entering Delete with id: {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Employee WHERE id=@id";
                    cmd.Parameters.Add(new SQLiteParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    log.InfoFormat("Exiting Delete with id: {0}", id);
                    return null;
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error deleting entity with error: {0}", e.Message);
                throw;
            }
        }

        public Employee? Update(Employee entity)
        {
            log.InfoFormat("Entering Update with id: {0}", entity.Id);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Employee SET username=@username, password=@password WHERE id=@id";
                    cmd.Parameters.Add(new SQLiteParameter("@username", entity.Username));
                    cmd.Parameters.Add(new SQLiteParameter("@password", entity.Password));
                    cmd.Parameters.Add(new SQLiteParameter("@id", entity.Id));
                    cmd.ExecuteNonQuery();
                    log.InfoFormat("Exiting Update with id: {0}", entity.Id);
                    return entity;
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error updating entity: {0}", e.Message);
                throw;
            }
        }
    }
}
