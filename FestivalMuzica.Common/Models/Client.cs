using System;

namespace FestivalMuzica.Common.Models
{
    [Serializable]
    public class Client : Entity<long>
    {
        public string Name { get; set; }

        public Client() { }

        public Client(string name)
        {
            Name = name;
        }

        public Client(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"Client{{Id={Id}, Name='{Name}'}}";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (Client)obj;
            return Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
    }
} 