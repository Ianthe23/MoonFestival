using log4net;
using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using repository.utils;

namespace festival_muzica.repository
{
    public class TicketDataBaseRepo : ITicketRepo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TicketDataBaseRepo));
        private readonly IDictionary<string, string> props;

        public TicketDataBaseRepo(IDictionary<string, string> props)
        {
            this.props = props ?? throw new ArgumentNullException(nameof(props));
            log.Info("Initializing TicketDataBaseRepo");
        }

        public IEnumerable<Ticket> FindByShow(Show show)
        {
            log.InfoFormat("Entering FindByShow with show: {0}", show);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Ticket> tickets = new List<Ticket>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Ticket WHERE showId=@showId JOIN Client ON Ticket.clientId = Client.id";
                cmd.Parameters.Add(new SQLiteParameter("@showId", show.Id));
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        long showId = dataReader.GetInt64(1);
                        long clientId = dataReader.GetInt64(2);
                        string clientName = dataReader.GetString(3);
                        int numberOfSeats = dataReader.GetInt32(4);
                        int price = dataReader.GetInt32(5);
                        tickets.Add(new Ticket(id, showId, new Client(clientId, clientName), numberOfSeats, price));
                    }
                }
            }
            log.InfoFormat("Exiting FindByShow with show: {0}", show);
            return tickets;
        }

        public IEnumerable<Ticket> FindByName(string name)
        {
            log.InfoFormat("Entering FindByName with name: {0}", name);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Ticket> tickets = new List<Ticket>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Ticket WHERE clientId=@clientId JOIN Client ON Ticket.clientId = Client.id";
                cmd.Parameters.Add(new SQLiteParameter("@clientId", name));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        long showId = dataReader.GetInt64(1);
                        long clientId = dataReader.GetInt64(2);
                        string clientName = dataReader.GetString(3);
                        int numberOfSeats = dataReader.GetInt32(4);
                        int price = dataReader.GetInt32(5);
                        tickets.Add(new Ticket(id, showId, new Client(clientId, clientName), numberOfSeats, price));
                    }
                }
            }
            log.InfoFormat("Exiting FindByName with name: {0}", name);
            return tickets;
        }   

        public Ticket? FindOne(long id)
        {
            log.InfoFormat("Entering FindOne with id: {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Ticket WHERE id=@id JOIN Client ON Ticket.clientId = Client.id";
                cmd.Parameters.Add(new SQLiteParameter("@id", id));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        long idRead = dataReader.GetInt64(0);
                        long showId = dataReader.GetInt64(1);
                        long clientId = dataReader.GetInt64(2);
                        string clientName = dataReader.GetString(3);
                        int numberOfSeats = dataReader.GetInt32(4);
                        int price = dataReader.GetInt32(5);
                        return new Ticket(idRead, showId, new Client(clientId, clientName), numberOfSeats, price);
                    }
                }
            }
            log.InfoFormat("Exiting FindOne with id: {0}", id);
            return null;
        }

        public IEnumerable<Ticket> FindAll()
        {
            log.InfoFormat("Entering FindAll");
            IDbConnection con = DBUtils.getConnection(props);
            IList<Ticket> tickets = new List<Ticket>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = """
                SELECT t.id, t.showId, t.clientId, c.name, t.numberOfSeats, t.price
                FROM Ticket t
                JOIN Client c ON t.clientId = c.id
                """;
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        long showId = dataReader.GetInt64(1);
                        long clientId = dataReader.GetInt64(2);
                        string clientName = dataReader.GetString(3);
                        int numberOfSeats = dataReader.GetInt32(4);
                        int price = dataReader.GetInt32(5);
                        tickets.Add(new Ticket(id, showId, new Client(clientId, clientName), numberOfSeats, price));
                    }
                }
            }
            log.InfoFormat("Exiting FindAll");
            return tickets;
        }

        public Ticket? Save(Ticket entity)
        {
            log.InfoFormat("Entering Save with entity: {0}", entity);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Ticket (showId, clientId, numberOfSeats, price) VALUES (@showId, @clientId, @numberOfSeats, @price)";
                    cmd.Parameters.Add(new SQLiteParameter("@showId", entity.ShowId));
                    cmd.Parameters.Add(new SQLiteParameter("@clientId", entity.client.Id));
                    cmd.Parameters.Add(new SQLiteParameter("@numberOfSeats", entity.NumberOfSeats));
                    cmd.Parameters.Add(new SQLiteParameter("@price", entity.Price));
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

        public Ticket? Delete(long id)
        {
            log.InfoFormat("Entering Delete with id: {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Ticket WHERE id=@id";
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

        public Ticket? Update(Ticket entity)
        {
            log.InfoFormat("Entering Update with entity: {0}", entity);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {     
                    cmd.CommandText = "UPDATE Ticket SET showId=@showId, clientId=@clientId, numberOfSeats=@numberOfSeats, price=@price WHERE id=@id";
                    cmd.Parameters.Add(new SQLiteParameter("@showId", entity.ShowId));
                    cmd.Parameters.Add(new SQLiteParameter("@clientId", entity.client.Id));
                    cmd.Parameters.Add(new SQLiteParameter("@numberOfSeats", entity.NumberOfSeats));
                    cmd.Parameters.Add(new SQLiteParameter("@price", entity.Price));
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
