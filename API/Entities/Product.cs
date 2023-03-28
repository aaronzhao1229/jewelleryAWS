namespace API.Entities
{
  public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Price { get; set; } // use long rather than decimal basesd on the payment provider we are using later on; a long is like a decimal withour using decimal points. So 100.00 will be 10000 in database
        public string PictureUrl { get; set; }
        public string Category { get; set; } // Type
        public int QuantityInStock { get; set; } 
        public string PublicId { get; set; }
    }
}