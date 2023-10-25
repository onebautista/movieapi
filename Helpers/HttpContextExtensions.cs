using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPaginationParam<T>(this HttpContext httpContext,
            IQueryable<T> queryable, int cantRegPorPagina)
        {
            double quantity = await queryable.CountAsync();
            double pageQuantity = Math.Ceiling(quantity / cantRegPorPagina);

            httpContext.Response.Headers.Add("cantidadPaginas", pageQuantity.ToString());
        }
    }
}
