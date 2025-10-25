using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Configurations;

public class PreferenceConfigurations : IEntityTypeConfiguration<Preference>
{
    public void Configure(EntityTypeBuilder<Preference> builder)
    {
        builder.ToTable("Preferences");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(50);
        builder
            .HasMany(x => x.CustomerPreferences)
            .WithOne(x => x.Preference)
            .HasForeignKey(x => x.PreferenceId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasData(FakeDataFactory.Preferences);
    }
}