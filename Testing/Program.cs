using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

var ideas = IdeaManipulator.Ideas;


using (var webClient = new HttpClient())
{
    foreach (var _ in Enumerable.Repeat(0,30))
    {
        var result = await webClient.GetAsync("https://whatthecommit.com/index.txt");
        var message = await result.Content.ReadAsStringAsync();
        
        IdeaManipulator.CreateNewByContent(message, 'I');
    }
}

Console.WriteLine(IdeaManipulator.ExportToJson());
internal static class IoHelper
{
    private static readonly string IdeasPath;
     
    public static string ReadContent(string path) => File.ReadAllText(path);

    public static IEnumerable<string> GetAllIdeaNames() => Directory
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

    private static void EnsureIdeaPathCreation()
    {
        if ( ! Directory.Exists(IdeasPath))
            Directory.CreateDirectory(IdeasPath);
    }

    static IoHelper()
    {
        IdeasPath = $"{Path.GetTempPath()}ideas/";
        EnsureIdeaPathCreation();
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
    private static DateTime InitialPosixTime => new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    
    public static int Now => (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static DateTime ToDateTime(this string posixTimeStamp)
    {
        if (int.TryParse(posixTimeStamp, out int parsedTimeStamp))
            return InitialPosixTime
                .AddSeconds(parsedTimeStamp)
                .ToLocalTime();

        throw new ArgumentException("Provided string is not a Unix Timestamp valid value.");
    }
    
    public static string Salt (this int unixTimeStamp)
    {
        return $"{unixTimeStamp}"
                   .PadRight(10,'0') 
               + $"{DateTime.Now.ToUniversalTime().Millisecond}"
                   .PadRight(3,'0');
    }
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
        Created = useful[1..].ToDateTime();
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
        var result = Idea.GenerateByContentAndCategory(content, category);
        Keeper.Add(result);
    }

    public static void RemoveIdea(Idea idea)
    {
        idea.DestroyFile();
        Keeper.Remove(idea);
    }

    public static void UpdateIdea(Idea idea, string content, char? category = null) => idea.Update(content, category);

    public static IReadOnlyCollection<Idea> Ideas => Keeper;

    public static string ExportToJson()
    {
        var serialized = JsonSerializer.Serialize(Keeper, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        return serialized;
    }
}