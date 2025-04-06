using System;

namespace FestivalMuzica.Common.Models
{
    [Serializable]
    public class Show : Entity<long>
    {
        public string Name { get; set; }
        public string ArtistName { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int AvailableSeats { get; set; }
        public int SoldSeats { get; set; }

        public Show() { }

        public Show(string name, string artistName, DateTime date, string location, int availableSeats, int soldSeats)
        {
            Name = name;
            ArtistName = artistName;
            Date = date;
            Location = location;
            AvailableSeats = availableSeats;
            SoldSeats = soldSeats;
        }

        public Show(long id, string name, string artistName, DateTime date, string location, int availableSeats, int soldSeats)
        {
            Id = id;
            Name = name;
            ArtistName = artistName;
            Date = date;
            Location = location;
            AvailableSeats = availableSeats;
            SoldSeats = soldSeats;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            Show show = (Show)obj;
            return Id == show.Id &&
                   Name.Equals(show.Name) &&
                   ArtistName.Equals(show.ArtistName) &&
                   Date.Equals(show.Date) &&
                   Location.Equals(show.Location) &&
                   AvailableSeats == show.AvailableSeats &&
                   SoldSeats == show.SoldSeats;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, ArtistName, Date, Location, AvailableSeats, SoldSeats);
        }
    }
} 