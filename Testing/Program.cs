using System.Collections.ObjectModel;
using System.Text;

var ideas = IdeaManipulator.ListAll();

foreach (var idea in ideas)
    Console.WriteLine(idea);

IdeaManipulator.CreateNewByContent("I Gotta GOU");
IdeaManipulator.CreateNewByContent("Nice");
IdeaManipulator.CreateNewByContent("I'm my own best friend");

IdeaManipulator.RemoveIdea(ideas.First());

IdeaManipulator.UpdateIdea(ideas.First(), "I'm alone now");


internal static class IoHelper
{
    private const string IdeasPath = @"C:\Users\Tei\Documents\test\";
     
    public static string ReadContent(string path)  
    {  
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);  
        StreamReader sr = new StreamReader(fs);
        sr.BaseStream.Seek(0, SeekOrigin.Begin);  
        string str = sr.ReadLine() ?? "";
        
        sr.Close();  
        fs.Close();
        
        return str;
    }

    public static string[] GetAllIdeaNames() => Directory
        .GetFiles(IdeasPath)
        .Select(Path.GetFileName)
        .ToArray();

    public static IEnumerable<Idea> RestoreAllIdeas ()
    {
        var idealist = new Collection<Idea>();

        foreach (string name in GetAllIdeaNames())
            idealist.Add(new Idea(name,ReadContent(string.Concat(IdeasPath, name))));
        
        return idealist;
    }

    public static void GenerateIdeaFile(Idea idea)
    {
        var created = File.Create(string.Concat(IdeasPath, idea.FileName));
        created.Write(Encoding.Default.GetBytes(idea.Content));
        created.Close();
    }
    public static void DestroyByFileName(string fileName)
    {
        var filepath = string.Concat(IdeasPath, fileName);
        File.Delete(filepath);
    }
}

/// <summary>
///  This fails to be consistent: if two or more files are added in the same
///  second, they will be overlapped / overwritten in the same file so... migrating to UTC timestamp.
///  Edit: Still, using UTC will make file name size almost double sized so, I could salt
///  UnixTS with UTC ticks
/// </summary>
internal static class PosixTimeHelper
{
    private static DateTime InitialPosixTime => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    
    public static int Now => (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static DateTime ToPosixToDateTime(this int posixTimeStamp)
        => InitialPosixTime
            .AddSeconds(posixTimeStamp)
            .ToLocalTime();
    
    public static DateTime ToPosixToDateTime(this string posixTimeStamp)
    {
        if (int.TryParse(posixTimeStamp, out int parsedTimeStamp))
            return InitialPosixTime
                .AddSeconds(parsedTimeStamp)
                .ToLocalTime();

        throw new ArgumentException("Provided string is not a Unix Timestamp valid value.");
    }
    
    public static string Salt (this int unixTimeStamp) 
        => $"{unixTimeStamp}{DateTime.Now.ToUniversalTime().Millisecond}";
}

// TODO: create interface for public access.
public sealed class Idea
{
    public Idea(string filename, string content)
    {
        SetUpByFileName(filename);
        Content = content;
    }
    
    private Idea(string content, char category)
    {
        var newFileName = $"{category}{PosixTimeHelper.Now.Salt()}.txt";
        SetUpByFileName(newFileName);
        Content = content;
    }
    
    public string Id { get; private set; }

    public string Content { get; private set; }
    public DateTime Created { get; private set; }
    public char CategoryId { get; private set; }
    public string FileName { get; private set; }
    
    public override string ToString() => $"Id: {Id}\n" +
                                         $"Created: {Created}\n" +
                                         $"File name: {FileName}\n" +
                                         $"Content: {Content}\n" +
                                         $"Category: {CategoryId}";
    
    internal static Idea GenerateByContentAndCategory(string content, char category)
    {
        var idea = new Idea(content, category);
        
        IoHelper.GenerateIdeaFile(idea);
        
        return idea;
    }

    private void SetUpByFileName(string filename)
    {
        var useful = filename[..11];
        
        FileName = filename;
        CategoryId = useful[0];
        Id = filename[1..14];
        Created = useful[1..].ToPosixToDateTime();
    }

    private string GetNewFileName() => $"{CategoryId}{PosixTimeHelper.Now.Salt()}.txt";
    
    internal void DestroyFile()
    {
        IoHelper.DestroyByFileName(FileName);
    }

    internal void GenerateFile()
    {
        IoHelper.GenerateIdeaFile(this);
    }
    internal void Update(string content, char? category)
    {
        DestroyFile();
        
        Content = content;
        if (category is not null)
            CategoryId = (char)category;
        
        SetUpByFileName(GetNewFileName());
        IoHelper.GenerateIdeaFile(this);
    }
}

public static class IdeaManipulator
{
    private static List<Idea> Keeper { get; } = IoHelper.RestoreAllIdeas().ToList();
    
    public static void CreateNewByContent(string content, char category = '0')
    {
        Idea.GenerateByContentAndCategory(content, category);
    }
    
    public static void AddIdea(Idea idea)
    {
        idea.GenerateFile();
        Keeper.Add(idea);
    }
    
    public static void RemoveIdea(Idea idea)
    {
        idea.DestroyFile();
        Keeper.Remove(idea);
    }

    public static void UpdateIdea(Idea idea, string content, char? category = null) => idea.Update(content, category);

    public static ReadOnlyCollection<Idea> ListAll() => Keeper.AsReadOnly();
}