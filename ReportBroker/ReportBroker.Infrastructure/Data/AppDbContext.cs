using Microsoft.EntityFrameworkCore;
using ReportBroker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportBroker.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Report> Report => Set<Report>();
        public DbSet<ReportRequest> ReportRequest => Set<ReportRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);
        }
    }
}
