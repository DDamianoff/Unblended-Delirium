using System.Drawing;

namespace Serk.Core.Interfaces;

public interface IColor
{
     ConsoleColor BaseColor
     { get; }
     
     KnownColor? DetailedColor
     { get; }
}