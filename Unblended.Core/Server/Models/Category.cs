using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Unblended.Core.Shared;

namespace Unblended.Core.Server.Models;

public class Category : ICategory
{
    public Category(char categoryId, string? title = null)
    {
        CategoryId = categoryId;
        Title = title;
    }
    
    [Key]
    [RegularExpression(@"0|Ñ|[A-Z]")]
    public char CategoryId { get; }
    
    public string? Title 
    { get; } 

    
    [NotMapped]
    public IColor? Color 
    { get; } 
        = null;
}