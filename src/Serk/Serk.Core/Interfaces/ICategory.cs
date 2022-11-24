namespace Serk.Core.Interfaces;

public interface ICategory
{ 
    char CategoryId 
    { get; }
    
    string? Title 
    { get; }
    
    IColor? Color 
    { get; }
}