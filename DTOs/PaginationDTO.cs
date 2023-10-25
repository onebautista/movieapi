namespace ApiPeliculas.DTOs
{
    public class PaginationDTO
    {
        public int Pagina { get; set; } = 1;

        private int cantRegPorPagina = 10;
        private readonly int cantMaxRegPorPagina = 50;

        public int CantRegPorPagina 
        { 
            get => cantRegPorPagina;
            set
            {
                cantRegPorPagina = (value > cantMaxRegPorPagina) ? cantMaxRegPorPagina : value; //100>50 = 50

            } 
        }
    }
}
