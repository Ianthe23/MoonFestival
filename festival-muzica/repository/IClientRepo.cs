interface IClientRepo: IRepository<long,Client>
{
    Client? FindByName(string name);
}
