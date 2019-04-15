using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamBoardApiServer.Models
{

    public class TeamBoardApiServerContext : DbContext
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<User> Users { get; set; }

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=TeamBoardApiServerDb;ConnectRetryCount=0");

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BoardToUser>()
                .HasKey(bc => new { bc.UserId, bc.BoardId });
            modelBuilder.Entity<BoardToUser>()
                .HasOne(bc => bc.Board)
                .WithMany(b => b.Members)
                .HasForeignKey(bc => bc.BoardId);
            modelBuilder.Entity<BoardToUser>()
                .HasOne(bc => bc.User)
                .WithMany(c => c.Boards)
                .HasForeignKey(bc => bc.UserId);
         
        }
    }
}
