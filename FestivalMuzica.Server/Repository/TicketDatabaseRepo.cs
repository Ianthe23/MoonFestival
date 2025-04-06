using log4net;
using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using FestivalMuzica.Common.Models;
using FestivalMuzica.Server.Repository.Utils;

namespace FestivalMuzica.Server.Repository
{
    public class TicketDatabaseRepo : ITicketRepo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TicketDatabaseRepo));
        private readonly IDictionary<string, string> props;

        public TicketDatabaseRepo(IDictionary<string, string> props)
        {
            this.props = props ?? throw new ArgumentNullException(nameof(props));
            log.Info("Initializing TicketDataBaseRepo");
        }

        public IEnumerable<Ticket> FindByShow(Show show)
        {
            log.InfoFormat("Entering FindByShow with id: {0}", show.Id);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Ticket> tickets = new List<Ticket>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = """
                SELECT t.id, t.showId, t.showName, t.clientId, c.name, t.numberOfSeats, t.price
                FROM Ticket t
                JOIN Client c ON t.clientId = c.id
                WHERE t.showId = @showId
                """;
                cmd.Parameters.Add(new SQLiteParameter("@showId", show.Id));
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        long showId = dataReader.GetInt64(1);
                        string showName = dataReader.GetString(2);
                        long clientId = dataReader.GetInt64(3);
                        string clientName = dataReader.GetString(4);
                        int numberOfSeats = dataReader.GetInt32(5);
                        int price = dataReader.GetInt32(6);
                        tickets.Add(new Ticket(id, showId, showName, new FestivalMuzica.Common.Models.Client(clientId, clientName), numberOfSeats, price));
                    }
                }
            }
            log.InfoFormat("Exiting FindByShow with id: {0}", show.Id);
            return tickets;
        }

        public IEnumerable<Ticket> FindByClient(FestivalMuzica.Common.Models.Client client)
        {
            log.InfoFormat("Entering FindByClient with id: {0}", client.Id);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Ticket> tickets = new List<Ticket>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = """
                SELECT t.id, t.showId, t.showName, t.clientId, c.name, t.numberOfSeats, t.price
                FROM Ticket t
                JOIN Client c ON t.clientId = c.id
                WHERE c.id = @clientId
                """;
                cmd.Parameters.Add(new SQLiteParameter("@clientId", client.Id));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        long showId = dataReader.GetInt64(1);
                        string showName = dataReader.GetString(2);
                        long clientId = dataReader.GetInt64(3);
                        string clientName = dataReader.GetString(4);
                        int numberOfSeats = dataReader.GetInt32(5);
                        int price = dataReader.GetInt32(6);
                        tickets.Add(new Ticket(id, showId, showName, new FestivalMuzica.Common.Models.Client(clientId, clientName), numberOfSeats, price));
                    }
                }
            }
            log.InfoFormat("Exiting FindByClient with id: {0}", client.Id);
            return tickets;
        }

        public IEnumerable<Ticket> FindByShowAndClient(Show show, FestivalMuzica.Common.Models.Client client)
        {
            log.InfoFormat("Entering FindByShowAndClient with id: {0} and id: {1}", show.Id, client.Id);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Ticket> tickets = new List<Ticket>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = """
                SELECT t.id, t.showId, t.showName, t.clientId, c.name, t.numberOfSeats, t.price
                FROM Ticket t
                JOIN Client c ON t.clientId = c.id
                WHERE t.showId = @showId AND t.clientId = @clientId
                """;
                cmd.Parameters.Add(new SQLiteParameter("@showId", show.Id));
                cmd.Parameters.Add(new SQLiteParameter("@clientId", client.Id));
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        long showId = dataReader.GetInt64(1);
                        string showName = dataReader.GetString(2);
                        long clientId = dataReader.GetInt64(3);
                        string clientName = dataReader.GetString(4);
                        int numberOfSeats = dataReader.GetInt32(5);
                        int price = dataReader.GetInt32(6);
                        tickets.Add(new Ticket(id, showId, showName, new FestivalMuzica.Common.Models.Client(clientId, clientName), numberOfSeats, price));
                    }
                }
            }
            log.InfoFormat("Exiting FindByShowAndClient with id: {0} and id: {1}", show.Id, client.Id);
            return tickets; 
        }

        public Ticket? FindOne(long id)
        {
            log.InfoFormat("Entering FindOne with id: {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = """
                SELECT t.id, t.showId, t.showName, t.clientId, c.name, t.numberOfSeats, t.price
                FROM Ticket t
                JOIN Client c ON t.clientId = c.id
                WHERE t.id = @id
                """;
                cmd.Parameters.Add(new SQLiteParameter("@id", id));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        long idRead = dataReader.GetInt64(0);
                        long showId = dataReader.GetInt64(1);
                        string showName = dataReader.GetString(2);
                        long clientId = dataReader.GetInt64(3);
                        string clientName = dataReader.GetString(4);
                        int numberOfSeats = dataReader.GetInt32(5);
                        int price = dataReader.GetInt32(6);
                        return new Ticket(idRead, showId, showName, new FestivalMuzica.Common.Models.Client(clientId, clientName), numberOfSeats, price);
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
                SELECT t.id, t.showId, t.showName, t.clientId, c.name, t.numberOfSeats, t.price
                FROM Ticket t
                JOIN Client c ON t.clientId = c.id
                """;
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        long showId = dataReader.GetInt64(1);
                        string showName = dataReader.GetString(2);
                        long clientId = dataReader.GetInt64(3);
                        string clientName = dataReader.GetString(4);
                        int numberOfSeats = dataReader.GetInt32(5);
                        int price = dataReader.GetInt32(6);
                        tickets.Add(new Ticket(id, showId, showName, new FestivalMuzica.Common.Models.Client(clientId, clientName), numberOfSeats, price));
                        log.InfoFormat("Found ticket with id: {0}", id);
                    }
                }
            }
            log.InfoFormat("Exiting FindAll");
            return tickets;
        }

        public Ticket? Save(Ticket entity)
        {
            log.InfoFormat("Entering Save with id: {0}", entity.Id);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = """
                    INSERT INTO Ticket (showId, showName, clientId, numberOfSeats, price) VALUES (@showId, @showName, @clientId, @numberOfSeats, @price)
                    """;
                    cmd.Parameters.Add(new SQLiteParameter("@showId", entity.ShowId));
                    cmd.Parameters.Add(new SQLiteParameter("@showName", entity.ShowName));
                    cmd.Parameters.Add(new SQLiteParameter("@clientId", entity.Client.Id));
                    cmd.Parameters.Add(new SQLiteParameter("@numberOfSeats", entity.NumberOfSeats));
                    cmd.Parameters.Add(new SQLiteParameter("@price", entity.Price));
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

        public Ticket? Delete(long id)
        {
            log.InfoFormat("Entering Delete with id: {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = """
                    DELETE FROM Ticket WHERE id=@id
                    """;
                    cmd.Parameters.Add(new SQLiteParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    log.InfoFormat("Exiting Delete with id: {0}", id);
                    return null;
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error deleting entity with id: {0} and error: {1}", id, e.Message);
                throw;
            }   
        }

        public Ticket? Update(Ticket entity)
        {
            log.InfoFormat("Entering Update with id: {0}", entity.Id);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {     
                    cmd.CommandText = """
                    UPDATE Ticket SET showId=@showId, showName=@showName, clientId=@clientId, numberOfSeats=@numberOfSeats, price=@price WHERE id=@id
                    """;
                    cmd.Parameters.Add(new SQLiteParameter("@showId", entity.ShowId));
                    cmd.Parameters.Add(new SQLiteParameter("@showName", entity.ShowName));
                    cmd.Parameters.Add(new SQLiteParameter("@clientId", entity.Client.Id));
                    cmd.Parameters.Add(new SQLiteParameter("@numberOfSeats", entity.NumberOfSeats));
                    cmd.Parameters.Add(new SQLiteParameter("@price", entity.Price));
                    cmd.Parameters.Add(new SQLiteParameter("@id", entity.Id));
                    cmd.ExecuteNonQuery();
                    log.InfoFormat("Exiting Update with id: {0}", entity.Id);
                    return entity;
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error updating entity with id: {0} and error: {1}", entity.Id, e.Message);
                throw;
            }
        }
    }
}   
