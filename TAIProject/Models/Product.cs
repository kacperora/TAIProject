using System.ComponentModel.DataAnnotations;

namespace TAIProject.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[]? Picture { get; set; }
        public int AmountInStore { get; set; }
        public string Description { get; set; }
        public int UnitPrice { get; set; }
        public Guid CategoryID { get; set; }

    }
}
