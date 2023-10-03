

using Microsoft.EntityFrameworkCore;
using System;

namespace ShipmentAssistant.Models
{
    public class HistoryContext : DbContext
    {
        public HistoryContext(DbContextOptions<HistoryContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<HistoryLog> HistoryLogs { get; set; }
    }
}
