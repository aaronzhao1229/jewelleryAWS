using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.RequestHelpers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, MetaData metaData)
        {
            var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase}; // apply camel case, otherwise it will be title case

            response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData, options)); // products.MetaData will be serialized into Json and which we can then return as pagination header
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination"); // we want this CORS header to be available in our clients. We can see this Pagination header on Seagger already, because swagger is using the same domain as our API server. But when we are on our React Client side, we need to specifically allow this header because it's a custom header that we're creating, we need a client to be able to read that header. If you called "Access-Control-Expose-Headers" anything different from how it is specified then you will not be able to read the pagination header on the client.
        }
    }
}