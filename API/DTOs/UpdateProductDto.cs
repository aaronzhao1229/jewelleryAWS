using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class UpdateProductDto
    {
        public int Id { get; set;}
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(100, Double.PositiveInfinity)] // specified the price range allowed
        public long Price { get; set; } 

      
        public IFormFile File { get; set; }  // use IFormFile to represent a file sent with the HttpRequest

        [Required]
        public string Category { get; set; }

        [Required]
        [Range(0, 100)]
        public int QuantityInStock { get; set; } 
    }
}