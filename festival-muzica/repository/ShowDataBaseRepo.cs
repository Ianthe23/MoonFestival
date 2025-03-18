using log4net;
using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using repository.utils;

namespace festival_muzica.repository
{
    public class ShowDataBaseRepo : IShowRepo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowDataBaseRepo));
        private readonly IDictionary<string, string> props;

        public ShowDataBaseRepo(IDictionary<string, string> props)
        {
            this.props = props ?? throw new ArgumentNullException(nameof(props));
            log.Info("Initializing ShowDataBaseRepo");
        }

        public IEnumerable<string> GetArtisti()
        {
            log.InfoFormat("Entering GetArtisti");
            IDbConnection con = DBUtils.getConnection(props);
            IList<string> artists = new List<string>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT DISTINCT artist FROM Show";
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        artists.Add(dataReader.GetString(0));
                    }
                }
            }
            log.InfoFormat("Exiting GetArtisti with {0} artists", artists.Count);
            return artists;
        }       

        public IEnumerable<Show> FindByArtist(string artist)
        {
            log.InfoFormat("Entering FindByArtist with artist: {0}", artist);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Show> shows = new List<Show>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Show WHERE artist=@artist";
                cmd.Parameters.Add(new SQLiteParameter("@artist", artist));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        string name = dataReader.GetString(1);
                        string artistName = dataReader.GetString(2);
                        DateTime date = dataReader.GetDateTime(3);
                        string location = dataReader.GetString(4);
                        int availableSeats = dataReader.GetInt32(5);
                        int soldSeats = dataReader.GetInt32(6);
                        shows.Add(new Show(id, name, artistName, date, location, availableSeats, soldSeats));
                    }
                }
            }
            log.InfoFormat("Exiting FindByArtist with artist: {0}", artist);
            return shows;
        }

        public IEnumerable<Show> FindByDate(DateTime date)
        {
            log.InfoFormat("Entering FindByDate with date: {0}", date);
            IDbConnection con = DBUtils.getConnection(props);
            IList<Show> shows = new List<Show>();

            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Show WHERE date=@date";
                cmd.Parameters.Add(new SQLiteParameter("@date", date));
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        string name = dataReader.GetString(1);
                        string artistName = dataReader.GetString(2);
                        DateTime dateRead = dataReader.GetDateTime(3);
                        string location = dataReader.GetString(4);
                        int availableSeats = dataReader.GetInt32(5);
                        int soldSeats = dataReader.GetInt32(6);
                        shows.Add(new Show(id, name, artistName, dateRead, location, availableSeats, soldSeats));
                    }
                }
            }
            log.InfoFormat("Exiting FindByDate with date: {0}", date);
            return shows;
        }

        public Show? FindOne(long id)
        {
            log.InfoFormat("Entering FindOne with id: {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Show WHERE id=@id";
                cmd.Parameters.Add(new SQLiteParameter("@id", id));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        long idRead = dataReader.GetInt64(0);
                        string name = dataReader.GetString(1);
                        string artistName = dataReader.GetString(2);
                        DateTime date = dataReader.GetDateTime(3);
                        string location = dataReader.GetString(4);
                        int availableSeats = dataReader.GetInt32(5);
                        int soldSeats = dataReader.GetInt32(6);
                        return new Show(idRead, name, artistName, date, location, availableSeats, soldSeats);
                    }
                }
            }   
            log.InfoFormat("Exiting FindOne with id: {0}", id);
            return null;
        }

        public IEnumerable<Show> FindAll()
        {
            log.InfoFormat("Entering FindAll");
            IDbConnection con = DBUtils.getConnection(props);
            IList<Show> shows = new List<Show>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Show";
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        string name = dataReader.GetString(1);
                        string artistName = dataReader.GetString(2);
                        DateTime date = dataReader.GetDateTime(3);
                        string location = dataReader.GetString(4);
                        int availableSeats = dataReader.GetInt32(5);
                        int soldSeats = dataReader.GetInt32(6);
                        shows.Add(new Show(id, name, artistName, date, location, availableSeats, soldSeats));
                        log.InfoFormat("Found show: {0}", shows.Last());
                    }
                }
            }
            log.InfoFormat("Exiting FindAll with {0} shows", shows.Count);
            return shows;   
        }

        public Show? Save(Show entity)
        {
            log.InfoFormat("Entering Save with entity: {0}", entity);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Show (name, artist, date, location, availableSeats, soldSeats) VALUES (@name, @artist, @date, @location, @availableSeats, @soldSeats)";
                    cmd.Parameters.Add(new SQLiteParameter("@name", entity.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@artist", entity.ArtistName));
                    cmd.Parameters.Add(new SQLiteParameter("@date", entity.Date));
                    cmd.Parameters.Add(new SQLiteParameter("@location", entity.Location));
                    cmd.Parameters.Add(new SQLiteParameter("@availableSeats", entity.AvailableSeats));
                    cmd.Parameters.Add(new SQLiteParameter("@soldSeats", entity.SoldSeats));
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

        public Show? Delete(long id)
        {
            log.InfoFormat("Entering Delete with id: {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Show WHERE id=@id";
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

        public Show? Update(Show entity)
        {
            log.InfoFormat("Entering Update with entity: {0}", entity);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Show SET name=@name, artist=@artist, date=@date, location=@location, availableSeats=@availableSeats, soldSeats=@soldSeats WHERE id=@id";
                    cmd.Parameters.Add(new SQLiteParameter("@name", entity.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@artist", entity.ArtistName));
                    cmd.Parameters.Add(new SQLiteParameter("@date", entity.Date));
                    cmd.Parameters.Add(new SQLiteParameter("@location", entity.Location));
                    cmd.Parameters.Add(new SQLiteParameter("@availableSeats", entity.AvailableSeats));
                    cmd.Parameters.Add(new SQLiteParameter("@soldSeats", entity.SoldSeats));
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
