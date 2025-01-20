using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAPI_Rest_ArleliotRoom.Models.Entities;

namespace WebAPI_Rest_ArleliotRoom.Data
{
    public class WebAPI_Rest_ArleliotRoomContext : DbContext
    {
        public WebAPI_Rest_ArleliotRoomContext(DbContextOptions<WebAPI_Rest_ArleliotRoomContext> options)
            : base(options)
        {
        }

        public DbSet<Salle> Salles { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<Utilisateur> Utilisateurs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Salle>().ToTable("Salles");
            modelBuilder.Entity<Reservation>().ToTable("Reservations");
            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateurs");
        }
    }

}

