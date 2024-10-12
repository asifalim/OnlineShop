using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Areas.Admin.Models
{
    public class RoleUservm
    {
        [Required]
        [Display(Name ="User")]
        public string UserId {  get; set; }
        [Required]
        [Display(Name ="Role")]
        public string RoleId { get; set; }
    }
}
