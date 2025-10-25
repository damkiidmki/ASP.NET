using System;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Configurations;

namespace PromoCodeFactory.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<CustomerPreference> CustomersPreferences { get; set; }

    public DbSet<Preference> Preferences { get; set; }

    public DbSet<PromoCode> PromoCodes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerPreferenceConfiguration());
        modelBuilder.ApplyConfiguration(new PreferenceConfigurations());
        modelBuilder.ApplyConfiguration(new PromoCodeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}