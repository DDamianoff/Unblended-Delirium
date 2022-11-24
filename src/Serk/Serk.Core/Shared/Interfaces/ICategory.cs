namespace SerkPlus.Core.Shared;

public interface ICategory
{ 
    char CategoryId 
    { get; }
    
    string? Title 
    { get; }
    
    IColor? Color 
    { get; }
}