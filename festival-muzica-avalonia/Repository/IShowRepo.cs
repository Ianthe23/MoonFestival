using System;
using System.Collections.Generic;
using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Repository
{
    public interface IShowRepo : IRepository<long, Show>
    {
        IEnumerable<Show> FindByArtist(string artist);
        IEnumerable<Show> FindByDate(DateTime date);
        IEnumerable<string> GetArtisti();
    }
}