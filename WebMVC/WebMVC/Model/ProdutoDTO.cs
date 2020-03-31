using System;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.Model
{
    public class ProdutoDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string PictureUri { get; set; }
    }
}

