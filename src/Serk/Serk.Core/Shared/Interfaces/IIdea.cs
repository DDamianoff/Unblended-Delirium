namespace SerkPlus.Core.Shared;

public interface IIdea
{
    string Id { get; }
    
    DateTime Created { get; }
    
    string Content { get; }
    
    char CategoryId { get; }
}