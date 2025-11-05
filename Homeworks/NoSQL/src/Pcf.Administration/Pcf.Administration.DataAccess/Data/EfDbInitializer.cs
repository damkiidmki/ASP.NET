using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess;
using Pcf.Administration.DataAccess.Data;

public class MongoDbInitializer : IDbInitializer
{
    private readonly MongoDbContext _context;

    public MongoDbInitializer(MongoDbContext context)
    {
        _context = context;
    }

    public void InitializeDb()
    {
        SeedData();
    }

    private void SeedData()
    {
        SeedRoles();
        SeedEmployees();
    }

    private void SeedRoles()
    {
        var roleCollection = _context.GetCollection<Role>();
        var existingRoles = roleCollection.Find(_ => true).Any();

        if (!existingRoles)
        {
            var roles = FakeDataFactory.Roles;
            roleCollection.InsertMany(roles);
        }
    }

    private void SeedEmployees()
    {
        var employeeCollection = _context.GetCollection<Employee>();
        var existingEmployees = employeeCollection.Find(_ => true).Any();

        if (!existingEmployees)
        {
            var employees = FakeDataFactory.Employees;
            employeeCollection.InsertMany(employees);
        }
    }
}