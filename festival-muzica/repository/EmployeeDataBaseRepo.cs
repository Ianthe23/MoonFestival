using log4net;
using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using repository.utils;

namespace festival_muzica.repository
{
    public class EmployeeDataBaseRepo : IEmployeeRepo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmployeeDataBaseRepo));
        private readonly IDictionary<string, string> props;

        public EmployeeDataBaseRepo(IDictionary<string, string> props)
        {
            this.props = props ?? throw new ArgumentNullException(nameof(props));
            log.Info("Initializing EmployeeDataBaseRepo");
        }

        public Employee? FindByUsername(string username)
        {
            log.InfoFormat("Entering FindByUsername with username: {0}", username);
            IDbConnection con = DBUtils.getConnection(props);
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Employee WHERE username=@username";
                cmd.Parameters.Add(new SQLiteParameter("@username", username));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        string name = dataReader.GetString(1);
                        string usernameRead = dataReader.GetString(2);
                        string password = dataReader.GetString(3);
                        return new Employee(id, name, username, password);
                    }
                }
            }
            log.InfoFormat("Exiting FindByUsername with username: {0}", username);
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
                        string name = dataReader.GetString(1);
                        string username = dataReader.GetString(2);
                        string password = dataReader.GetString(3);
                        return new Employee(idRead, name, username, password);
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
                    while(dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        string name = dataReader.GetString(1);
                        string username = dataReader.GetString(2);
                        string password = dataReader.GetString(3);
                        employees.Add(new Employee(id, name, username, password));
                    }
                }
            }
            log.InfoFormat("Exiting FindAll with {0} employees", employees.Count);
            return employees;
        }

        public Employee? Save(Employee entity)
        {
            log.InfoFormat("Entering Save with entity: {0}", entity);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Employee (name, username, password) VALUES (@name, @username, @password)";
                    cmd.Parameters.Add(new SQLiteParameter("@name", entity.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@username", entity.Username));
                    cmd.Parameters.Add(new SQLiteParameter("@password", entity.Password));
                    cmd.ExecuteNonQuery();
                    log.InfoFormat("Exiting Save with entity: {0}", entity);
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
                log.ErrorFormat("Error deleting entity: {0}", e.Message);
                throw;
            }
        }

        public Employee? Update(Employee entity)
        {
            log.InfoFormat("Entering Update with entity: {0}", entity);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Employee SET name=@name, username=@username, password=@password WHERE id=@id";
                    cmd.Parameters.Add(new SQLiteParameter("@name", entity.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@username", entity.Username));
                    cmd.Parameters.Add(new SQLiteParameter("@password", entity.Password));
                    cmd.Parameters.Add(new SQLiteParameter("@id", entity.Id));
                    cmd.ExecuteNonQuery();
                    log.InfoFormat("Exiting Update with entity: {0}", entity);
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
