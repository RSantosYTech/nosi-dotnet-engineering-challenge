using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Data
{
    /// <summary>
    /// Main class for the Entity Framework implementation.
    /// </summary>
    public class ContentDbContext : DbContext
    {
        //
        public ContentDbContext(DbContextOptions<ContentDbContext> options) : base(options) 
        {
            
        }

        //DbSets/Tables registers

        /// <summary>
        /// Main 'Content' table.
        /// </summary>
        public DbSet<Content> Contents { get; set; }

        /// <summary>
        /// Auxiliary 'ContentGenres' table, associating any number of unique genres with corresponding Content records.
        /// </summary>
        public DbSet<ContentGenre> ContentGenres { get; set; }


        //Overriding OnModelCreating to build table relations.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Content>()
                .HasMany(e => e.GenreList)
                .WithOne(e => e.Content)
                .HasForeignKey(e => e.ContentId)
                .HasPrincipalKey(e => e.Id);
        }

    }
}
