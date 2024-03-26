using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BuildyBackend.Core.Helpers
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// ***** -------------------------------> Deprecated 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpContext"></param>
        /// <param name="queryable"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async static Task InsertParamPaginationHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }
            double count = await queryable.CountAsync();
            httpContext.Response.Headers.Add("totalSizeRecords", count.ToString());
        }

        /// <summary>
        /// Versión mejorada:
        /// Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/20232284#overview
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpContext"></param>
        /// <param name="queryable"></param>
        /// <param name="recordsPerPage"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async static Task InsertParamPaginationHeader<T>(this HttpContext httpContext, IQueryable<T> queryable, int recordsPerPage)
        {
            double count = await queryable.CountAsync();
            double countPages = Math.Ceiling(count / recordsPerPage);
            httpContext.Response.Headers.Add("totalSizeRecords", countPages.ToString());
        }

    }
}