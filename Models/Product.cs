using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication1.Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    public string? Image { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    [Required]
    public bool Active { get; set; } = true;
    [Required]
    public int CategoryId { get; set; }
    [ValidateNever]
    public Category Category { get; set; }
}