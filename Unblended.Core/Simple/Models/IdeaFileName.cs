using System.ComponentModel.DataAnnotations;
using Unblended.Core.Simple.Utils;


namespace Unblended.Core.Simple.Models;

internal class IdeaFileName
{
    internal string FileName =>
        $"{CategoryId}" +
        $"{Identifier}" +
        $"{FileExtension}";
    
    [RegularExpression(@"[A-Z]")]
    internal char CategoryId 
    { get; }
    
    [StringLength(10)]
    [RegularExpression(@"^[0-9]*$")]
    internal string EpochTime 
    { get; }

    [StringLength(03)]
    [RegularExpression(@"^[0-9]*$")]
    internal string UtcTicksSalt
    { get; }

    [RegularExpression(@"(\.(md|txt)$)")]
    internal string FileExtension
    { get; }

    internal string Identifier 
        => string.Concat(EpochTime, UtcTicksSalt);

    internal static IdeaFileName GenerateNewInfo() => new ();

    private IdeaFileName()
    {
        CategoryId = Category.Default;
        EpochTime = EpochHelper.Now;
        UtcTicksSalt = EpochHelper.Salt;
        FileExtension = IoHelper.DefaultIdeaFileExtension;
    }
    
    internal static IdeaFileName GenerateNewInfo(char category) => new (category);

    private IdeaFileName(char category)
    {
        CategoryId = category;
        EpochTime = EpochHelper.Now;
        UtcTicksSalt = EpochHelper.Salt;
        FileExtension = IoHelper.DefaultIdeaFileExtension;
    }
    
    private IdeaFileName(char categoryId, string epochTime, string utcTicksSalt, string fileExtension)
    {
        CategoryId = categoryId;
        EpochTime = epochTime;
        UtcTicksSalt = utcTicksSalt;
        FileExtension = fileExtension;
        
        Validate();
    }

    private void Validate()
    {
        var isValid = this.TryValidate(out var results);

        if (isValid) 
            return;
        
        var messages = string.Join("\n", results
            .Select(v => v.ErrorMessage)
            .ToArray());
        
        throw new ArgumentException("Data provided to filename is invalid \n" + messages);
    }
    
    public static implicit operator IdeaFileName(string stringFileName)
    {
        return new IdeaFileName(
            categoryId: stringFileName[0],
            epochTime: stringFileName[1..11],
            utcTicksSalt: stringFileName[11..14],
            fileExtension: stringFileName[14..]);
    }
}

internal static class IdeaFileNameHelper
{
    internal static bool TryValidate(this object obj, out ICollection<ValidationResult> results)  
    {  
        var context = new ValidationContext(obj, serviceProvider: null, items: null);  
        results = new List<ValidationResult>();  
        return Validator.TryValidateObject(  
            obj, context, results,  
            validateAllProperties: true  
        );  
    }  
}