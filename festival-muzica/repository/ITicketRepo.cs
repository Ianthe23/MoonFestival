interface ITicketRepo: IRepository<long,Ticket>
{
    IEnumerable<Ticket> FindByShow(Show show);
    IEnumerable<Ticket> FindByName(string name);
}