using System.Drawing;
using System.Text.RegularExpressions;
using Serk.Core.Interfaces;

namespace Serk.Core.Models;

public class HexColor : IColor
{
    public HexColor(int blue, int green, int red)
    {
        Blue = blue;
        Green = green;
        Red = red;
    }

    public HexColor(string stringValue)
    {
        if (!IsValidHexCode(stringValue))
            throw new ArgumentException("provided string is not a valid HEX color code");
        
        stringValue = stringValue.PadRight(7, '0');
        
        // # A A A 0 0 0
        // 0 1 2 3 4 5 6
        
        Blue = int.Parse(stringValue[1..3]);
        Green = int.Parse(stringValue[3..5]);
        Red = int.Parse(stringValue[5..]);
    }
    
    // TODO: decide what to do with this
    public ConsoleColor BaseColor { get; }
    public KnownColor? DetailedColor { get; }
    public int Red { get; }
    public int Green { get; }
    public int Blue { get; }

    public override string ToString()
    {
        return "#" + 
               Red.ToString("X")
                   .PadRight(2, '0') +
               Green.ToString("X")
                   .PadRight(2, '0') +
               Blue.ToString("X")
                   .PadRight(2, '0');
    }
    
    public static bool IsValidHexCode (string hexColor) 
        => Regex.IsMatch(hexColor, @"^#(?:[0-9a-fA-F]{1,6})$");

    public static implicit operator string (HexColor hexColor) 
        => hexColor.ToString();

    public static explicit operator HexColor (string colorCode) 
        => new (colorCode);
}