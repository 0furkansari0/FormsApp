using System.ComponentModel.DataAnnotations;

namespace FormsApp.Models
{
    public class Product
    {
        [Display(Name = "Ürün Id")]
        public int ProductId { get; set; }

        [Required(ErrorMessage ="Ürün Adı giriniz.")]
        [StringLength(100)]
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0,1000000, ErrorMessage ="Fiyat uygun değil.")]
        [Display(Name = "Fiyat")]
        public decimal? Price { get; set; }
        
        [Display(Name = "Resim")]
        public string Image { get; set; } = string.Empty;

        [Display(Name = "Ürün Aktif mi?")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Kategori")]
        public int? CategoryId { get; set; }
        
    }
}