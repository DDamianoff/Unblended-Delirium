using SerkPlus.Core.Shared.Utils;
using SerkPlus.Core.Shared;


namespace SerkPlus.Core.Simple.Models;

public sealed class Idea : IIdea
{
    private IdeaFile _file;
    
    #region IIdea
    public string Id 
    { get; private set; }

    public DateTime Created
    { get; private set; }
    
    public string Content
    { get; private set; }
    
    public char CategoryId
    { get; private set; }

    #endregion
    
    internal void DeleteFile()
    {
        _file.Destroy();
    }
    
    internal void Update(string content)
    {
        var newData = IdeaFileName.GenerateNewInfo(CategoryId);
        
        _file.Destroy();
        _file = IdeaFile.CreateNew(newData, content);

        Id = newData.Identifier;
        Created = newData.EpochTime.ToDateTime();
        Content = Content;
    }

    #region Exposed Builder Methods

    internal static Idea LoadFromFileName (IdeaFileName fileName) => new (fileName);

    internal static Idea CreateNew(string content, char category) => new (content, category);

    internal static Idea CreateNew(string content) => new (content);
    
    #endregion

    #region Builders
    private Idea(IdeaFileName fileName)
    {
        Id = fileName.Identifier;
        Created = fileName.EpochTime.ToDateTime();
        CategoryId = fileName.CategoryId;
        _file = IdeaFile.RepresentExistingFile(fileName);
        Content = _file.ReadContent();
    }
    private Idea(string content, char category)
    {
        var newFileName = IdeaFileName.GenerateNewInfo(category);

        Id = newFileName.Identifier;
        Content = content;
        CategoryId = category;
        Created = newFileName.EpochTime.ToDateTime();
        _file = IdeaFile.CreateNew(newFileName, content);
    }
    private Idea(string content)
    {
        var newFileName = IdeaFileName.GenerateNewInfo();
        
        Id = newFileName.Identifier;
        Content = content;
        CategoryId = newFileName.CategoryId;
        Created = newFileName.EpochTime.ToDateTime();
        _file = IdeaFile.CreateNew(newFileName, content);
    }
    #endregion
}