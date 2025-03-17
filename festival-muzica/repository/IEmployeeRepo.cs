interface IEmployeeRepo : IRepository<long,Employee>
{
    Employee? FindByUsername(string username);
}