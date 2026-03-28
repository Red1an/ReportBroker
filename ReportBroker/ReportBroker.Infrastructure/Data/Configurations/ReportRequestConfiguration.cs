using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportBroker.Domain.Entities;

namespace ReportBroker.Infrastructure.Data.Configurations
{
    public class ReportRequestConfiguration : IEntityTypeConfiguration<ReportRequest>
    {
        public void Configure(EntityTypeBuilder<ReportRequest> builder) 
        {
            builder.ToTable("report_requests");

            builder.HasKey(rr => rr.Id);

            builder.Property(rr => rr.Id)
                .HasColumnName("id");

            builder.Property(r => r.ReportId)
            .HasColumnName("report_id")
            .IsRequired();

            builder.Property(r => r.UserId)
                .HasColumnName("user_id")
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(r => r.RequestAt)
                .HasColumnName("request_at");
        }
    }
}
