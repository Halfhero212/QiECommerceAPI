using System.ComponentModel.DataAnnotations;

namespace QiECommerceAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // Price in IQD
        [Range(0, 999999999)]
        public decimal PriceIQD { get; set; }

        [Range(0, 1000000)]
        public int Stock { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(100)]
        public string? SupplierName { get; set; }
    }
}
