using BuildyBackend.Core.DTO;

namespace BuildyBackend.Core.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> DoPagination<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.RecordsPerPage)
                .Take(paginationDTO.RecordsPerPage);
        }
    }
}
