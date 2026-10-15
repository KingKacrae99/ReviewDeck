using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReviewDeck.Models;

namespace ReviewDeck.Data
{
    public class ReviewDeckContext : DbContext
    {
        public ReviewDeckContext (DbContextOptions<ReviewDeckContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<JobApplication> JobApplications { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                 .HasIndex(user => user.Email)
                 .IsUnique();
        }
    }

}
