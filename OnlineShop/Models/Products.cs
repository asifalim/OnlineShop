using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Products
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string? Image { get; set; }
        [Display(Name="Product Color")]
        [Required]
        public string productColor {  get; set; }
        [Display(Name ="Available")]
        public bool isAvailable {  get; set; }
        [Required]
        [Display(Name ="Product Type")]
        public int ProductTypeId {  get; set; }
        [ForeignKey("ProductTypeId")]
        public ProductTypes ProductTypes { get; set; }


    }
}
