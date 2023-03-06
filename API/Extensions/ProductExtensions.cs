using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Extensions
{
    public static class ProductExtensions
    {
        public static IQueryable<Product> Sort(this IQueryable<Product> query, string orderBy) // in order to extend an IQueryable, we use the this keyword as the first thing that we put inside the parameters and then we specifiwaht it si that we're extending (IQueryable in this case)
        {
            if (string.IsNullOrWhiteSpace(orderBy)) return query.OrderBy(p => p.Name); // if nothing is in orderBy string

            query = orderBy switch
            {
                "price" => query.OrderBy(p => p.Price),
                "priceDesc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name),  // default case

            };
            return query;
        }

        public static IQueryable<Product> Search(this IQueryable<Product> query, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower(); //Trim() is to get rid of additional white space

            return query.Where(p => p.Name.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<Product> Filter(this IQueryable<Product> query, string categories)
        {
            var categoryList = new List<string>();

            if (!string.IsNullOrEmpty(categories))
                categoryList.AddRange(categories.ToLower().Split(",").ToList());

            query = query.Where(p => categoryList.Count == 0 || categoryList.Contains(p.Category.ToLower())); // check if categoryList is empty. If it is empty, then return nothing. If it isn't, it returns all of the categories that match anything that's inside the category list.

            return query;
        }
    }
}