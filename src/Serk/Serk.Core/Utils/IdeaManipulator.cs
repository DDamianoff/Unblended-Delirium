using Serk.Core.Interfaces;
using Serk.Core.Models;

namespace Serk.Core.Utils;

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