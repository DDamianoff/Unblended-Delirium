using System.Text;
using Serk.Core.Models;

namespace Serk.Core.Utils;

internal static class IoHelper
{
    public static readonly string DefaultIdeaFileExtension;
    public static readonly string IdeasPath;

    static IoHelper()
    {
        DefaultIdeaFileExtension = ".txt";
        IdeasPath = $"{Path.GetTempPath()}ideas/";
        
        if ( ! Directory.Exists(IdeasPath))
            Directory.CreateDirectory(IdeasPath);
    }

    public static void DeleteFile(string filepath)
    {
        File.Delete(filepath);
    }

    internal static string ToIdeaPath(this IdeaFileName fileName)
    {
        return string.Concat(IdeasPath, fileName.FileName);
    }
    
    public static string GetIdeaFileContent(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    public static void CreateIdeaFile(string filePath, string content)
    {
        var fileStream =  File.Create(filePath);
        fileStream.Write(Encoding.Default.GetBytes(content));
        fileStream.Close();
    }

    public static string[]? GetIdeaFileNames()
    {
        string[] ideaFileNames = Directory
            .GetFiles(IdeasPath)
            .Select(Path.GetFileName)
            .ToArray()!;

        return !ideaFileNames.Any() 
            ? null
            : ideaFileNames;
    }
}