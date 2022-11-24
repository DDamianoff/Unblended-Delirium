namespace Serk.Core.Interfaces;

public interface IIdea
{
    string Id { get; }
    
    DateTime Created { get; }
    
    string Content { get; }
    
    char CategoryId { get; }
}