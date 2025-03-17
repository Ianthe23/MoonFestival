interface IShowRepo: IRepository<long,Show>
{
    IEnumerable<Show> FindByArtist(string artist);
    IEnumerable<Show> FindByDate(DateTime date);
    IEnumerable<string> GetArtisti();
}