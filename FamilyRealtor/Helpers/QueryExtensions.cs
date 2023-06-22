using Microsoft.EntityFrameworkCore;

namespace FamilyRealtor.Helpers
{
    public static class QueryExtensions
    {
		public static async Task<int> CountPagesAsync<T>(this IQueryable<T> query, int pageSize)
		{
			return Convert.ToInt32(Math.Ceiling(await query.CountAsync() / (double)pageSize));
		}

		public static IQueryable<T> GetPage<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}