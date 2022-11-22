using Unblended.Core.Simple.Models;
using Unblended.Core.Simple.Utils;

namespace Unblended.Core.Simple.Models;

internal readonly struct IdeaFile
{
    #region Builders

    private IdeaFile(IdeaFileName fileName)
    {
        FileName = fileName;
        _filePath = fileName.ToIdeaPath();
    }
    private IdeaFile(IdeaFileName fileName, string content)
    {
        FileName = fileName;
        _filePath = fileName.ToIdeaPath();
        Generate(content);
    }

    #endregion
    
    internal readonly IdeaFileName FileName;
    
    private readonly string _filePath;
    internal static IdeaFile RepresentExistingFile(IdeaFileName fileName) => new (fileName);
   

    internal static IdeaFile CreateNew(IdeaFileName fileName, string content) => new(fileName, content);
    

    internal string ReadContent()
    {
        return IoHelper.GetIdeaFileContent(_filePath);
    }

    
    internal void Destroy()
    {
        IoHelper.DeleteFile(_filePath);
    }
    private void Generate(string content)
    {
        IoHelper.CreateIdeaFile(_filePath, content);
    }
}