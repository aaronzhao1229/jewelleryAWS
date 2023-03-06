using Microsoft.EntityFrameworkCore;

namespace API.RequestHelpers
{
  public class PagedList<T> : List<T>
    {
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData
            {
                TotalCount = count,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double) pageSize)
            };
            AddRange(items); 
            // When we create any instance of our PageList, we wil pass it the parameters. When we return from this page list or we have this page list, we will have all of our metadata and we have items inside it as well

        }

        public MetaData MetaData { get; set; }

        public static async Task<PagedList<T>> ToPagedList(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync(); // at this point, query is going to be executed against the database because we need to execute this against the database to find out the total counts of items available
            
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            
            
            // var allItems = await query.ToListAsync();
            // if (allItems.Count() <= 6) pageNumber = 1;            

            return new PagedList<T>(items, count, pageNumber, pageSize);

        }
    }
}