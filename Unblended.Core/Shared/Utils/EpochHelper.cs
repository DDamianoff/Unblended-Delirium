namespace Unblended.Core.Shared.Utils;

public static class EpochHelper
{
    private static DateTime InitialPosixTime => new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    
    public static string Now => $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"
        .PadRight(10, '0');

    public static string Salt => $"{DateTime.Now.ToUniversalTime().Millisecond}".PadRight(3, '0');

    public static string SaltedNow => string.Concat(Now, Salt);
    
    public static DateTime ToDateTime(this string posixTimeStamp)
    {
        if (int.TryParse(posixTimeStamp, out int parsedTimeStamp))
            return InitialPosixTime
                .AddSeconds(parsedTimeStamp)
                .ToLocalTime();

        throw new ArgumentException("Provided string is not a Unix Timestamp valid value.");
    }
}