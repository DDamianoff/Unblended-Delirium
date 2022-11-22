using Unblended.Core.Simple.Interfaces;
using Unblended.Core.Simple.Models;
using Unblended.Core.Simple.Utils;

namespace Unblended.Core.Simple;

public static class IdeaManipulator
{
    private static List<Idea> IdeaList { get; }

    static IdeaManipulator()
    {
        IdeaList = new List<Idea>();
        var ideaFiles = IoHelper.GetIdeaFileNames();

        if (ideaFiles is not null)
            foreach (var ideaFile in ideaFiles)
                IdeaList.Add(Idea.LoadFromFileName(ideaFile));
    }
    
    public static IReadOnlyCollection<IIdea> Ideas => IdeaList;
}