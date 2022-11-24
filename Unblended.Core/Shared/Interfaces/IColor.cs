using System.Drawing;

namespace Unblended.Core.Shared;

public interface IColor
{
     ConsoleColor BaseColor
     { get; }
     
     KnownColor? DetailedColor
     { get; }
}