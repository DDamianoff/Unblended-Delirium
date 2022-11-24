using System.Drawing;

namespace SerkPlus.Core.Shared;

public interface IColor
{
     ConsoleColor BaseColor
     { get; }
     
     KnownColor? DetailedColor
     { get; }
}