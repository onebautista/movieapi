namespace ApiPeliculas.Entities
{
    public class PeliculasGenres
    {
        //id de ambas entidades
        public int GenreId { get; set; }
        public int PeliculaId { get; set;}

        //propiedades de navegacion entre ambas entidades 
        public Genre Genre { get; set; }
        public Pelicula Pelicula { get; set; }
    }
}
