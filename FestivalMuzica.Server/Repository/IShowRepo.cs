using System;
using System.Collections.Generic;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Server.Repository
{
    public interface IShowRepo : IRepository<long, Show>
    {
        IEnumerable<Show> FindByArtist(string artist);
        IEnumerable<Show> FindByDate(DateTime date);
        IEnumerable<string> GetArtisti();
        IEnumerable<Show> FindByArtistAndTime(string artist, string time);
    }
}