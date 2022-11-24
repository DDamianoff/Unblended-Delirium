using SerkPlus.Core.Shared;
using SerkPlus.Core.Simple.Models;
using SerkPlus.Core.Simple.Utils;

namespace SerkPlus.Core.Simple;

public static class IdeaManipulator
{
    private static List<Idea> IdeaList { get; }

    static IdeaManipulator()
    {
        IdeaList = new List<Idea>();
        var ideaFiles = IoHelper.GetIdeaFileNames();

        if (ideaFiles is not null)
            foreach (var ideaFile in ideaFiles)
                IdeaList.Add(Idea.LoadFromFileName((IdeaFileName)ideaFile));
    }
    
    public static IReadOnlyCollection<IIdea> Ideas => IdeaList;
}