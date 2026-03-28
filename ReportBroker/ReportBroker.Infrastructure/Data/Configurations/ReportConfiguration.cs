using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportBroker.Domain.Entities;
using ReportBroker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportBroker.Infrastructure.Data.Configurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("reports");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasColumnName("id");

            builder.Property(r => r.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasDefaultValue(ReportStatus.Pending);

            builder.Property(r => r.ViewCount)
                .HasColumnName("view_count");

            builder.Property(r => r.PaymentCount)
                .HasColumnName("payment_count");

            builder.Property(r => r.ConversionRatio)
             .HasColumnName("conversion_ratio");

            builder.Property(r => r.CreatedAt)
                .HasColumnName("created_at");

            builder.Property(r => r.CompletedAt)
                .HasColumnName("completed_at");

            builder.OwnsOne(r => r.Parameters, rp =>
            {
                rp.Property(p => p.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

                rp.Property(p => p.DesignId)
                .HasColumnName("design_id")
                .IsRequired();

                rp.Property(p => p.PeriodStart)
                .HasColumnName("period_start")
                .IsRequired();

                rp.Property(p => p.PeriodEnd)
                .HasColumnName("period_end")
                .IsRequired();
            });

            builder.HasIndex(
                "Parameters_ProductId",
                "Parameters_DesignId",
                "Parameters_PeriodStart",
                "Parameters_PeriodEnd")
                .IsUnique()
                .HasDatabaseName("ix_reports_unique_parameters");

            builder.HasMany(r => r.Requests)
                .WithOne(rr => rr.Report)
                .HasForeignKey(rr => rr.ReportId);
                
        }
    }
}
