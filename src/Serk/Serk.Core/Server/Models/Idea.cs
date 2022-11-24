using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SerkPlus.Core.Simple.Utils;
using SerkPlus.Core.Shared;
using SerkPlus.Core.Shared.Utils;

namespace SerkPlus.Core.Server.Models;

public class Idea : IIdea
{
    public Idea(string id, string content, DateTime created)
    {
        Id = EpochHelper.SaltedNow;
        Content = content;
        Created = created;
    }
    
    [Key]
    public string Id { get; set; }
    public DateTime Created { get; set; }
    
    [MaxLength(1024)]
    public string Content { get; set; }
    
    [DefaultValue('0')]
    public char CategoryId { get; set; }
}