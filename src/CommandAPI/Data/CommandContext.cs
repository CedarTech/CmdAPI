using System.Diagnostics.CodeAnalysis;
using CommandAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandAPI.Data
{
    public class CommandContext : DbContext
    {
        public CommandContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        public DbSet<Command> CommandItems { get; set; }
    }
}