using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.RequestHelpers
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1; // default page number is 1 so they always get the first page when they make a request for a list of products. 
        // we need to create the page size property and to do this one slightly differently because we want to use the max page size. If they do request something that's bigger than 50 that we're going to set the page size 50. If they request something that's smaller than 50, we obviously want to set the page size to what they've requested.

        private int _pageSize = 6;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }


    }
}