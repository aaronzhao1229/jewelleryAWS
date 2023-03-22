using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class FallbackController : Controller // what we're returning from this is effectively a view our index html. 
    {
        public IActionResult Index() // call it index to match we specified in Program.cs
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML"); // this is what we need to serve our client application from our API service.
            
        }
    }
}