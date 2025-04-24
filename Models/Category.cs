using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication1.Models;

public class Category
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [ValidateNever]
    public ICollection<Product> Products { get; set; }
}