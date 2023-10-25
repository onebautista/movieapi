namespace ApiPeliculas.Entities
{
    public class PeliculasActors
    {
        public int ActorId { get; set; }
        public int PeliculaId { get; set; }

        //extra columns
        public string Personaje { get; set; }
        public int Orden { get; set; }

        //propiedades de navegacion
        public Actor Actor { get; set; }
        public Pelicula Pelicula { get; set; }
             
    }
}
