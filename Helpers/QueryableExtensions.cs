using ApiPeliculas.DTOs;

namespace ApiPeliculas.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO) 
        {
            return queryable
                .Skip((paginationDTO.Pagina - 1) * paginationDTO.CantRegPorPagina)
                .Take(paginationDTO.CantRegPorPagina);
        }
    }
}
