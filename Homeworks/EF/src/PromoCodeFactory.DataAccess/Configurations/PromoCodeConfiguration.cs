using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Configurations;

public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.ToTable("PromoCodes");
        builder.HasKey(pc => pc.Id);
        builder.Property(pc => pc.Code).HasMaxLength(50);
        builder.Property(pc => pc.ServiceInfo).HasMaxLength(50);
        builder.Property(pc => pc.BeginDate);
        builder.Property(pc => pc.EndDate);
        builder.Property(pc => pc.PartnerName).HasMaxLength(50);
        builder
            .HasOne(pc => pc.Customer)
            .WithMany(c => c.PromoCodes)
            .HasForeignKey(x => x.CustomerId)
            .IsRequired();
        builder
            .HasOne(pc => pc.Preference)
            .WithMany()
            .HasForeignKey(x => x.PreferenceId)
            .IsRequired();
    }
}