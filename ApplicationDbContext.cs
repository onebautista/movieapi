using ApiPeliculas.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }


        // API fluented para indicarle una relacion compuesta
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeliculasActors>()
                .HasKey(x => new { x.ActorId, x.PeliculaId });

            modelBuilder.Entity<PeliculasGenres>()
                .HasKey(x => new { x.GenreId, x.PeliculaId });

            base.OnModelCreating(modelBuilder);
        }




        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }

        //relacion muchos a muchos
        public DbSet<PeliculasActors> PeliculasActors { get; set; }
        public DbSet<PeliculasGenres> PeliculasGenres { get; set; }


    }
}
