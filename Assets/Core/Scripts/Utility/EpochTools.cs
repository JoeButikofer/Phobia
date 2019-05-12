using System;

public class EpochTools
{
    /// <summary>
    /// Return a convenient string representation for folder/file name,
    /// Format : yyyy_MM_dd-HH_mm_ss
    /// </summary>
    /// <param name="epoch">epoch date (number milliseconds since 1.1.1970)</param>
    /// <returns>a sortable representation of the date, to be used for a folder/file name</returns>
    public static string ConvertEpochToSortableDateTimeString(long epoch)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(epoch);
        return String.Format("{0:yyyy_MM_dd-HH_mm_ss}", dateTime);
    }

    /// <summary>
    /// Return a convenient string representation for displaying a date to users,
    /// Format : dddd dd.MM.yyyy
    /// </summary>
    /// <param name="epoch">epoch date (number milliseconds since 1.1.1970)</param>
    /// <returns>a human readable representation of the date (not the time)</returns>
    public static string ConvertEpochToHumanReadableDate(long epoch)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(epoch);
        return String.Format(new System.Globalization.CultureInfo("fr-FR"), "{0:dddd dd.MM.yyyy}", dateTime);
    }

    /// <summary>
    /// Return a convenient string representation for displaying a time to users,
    /// Format : HH:mm
    /// </summary>
    /// <param name="epoch">epoch date (number milliseconds since 1.1.1970)</param>
    /// <returns>a human readable representation of the time</returns>
    public static string ConvertEpochToHumanReadableTime(long epoch, bool localTime = false)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(epoch);

        if (localTime)
            dateTime = TimeZone.CurrentTimeZone.ToLocalTime(dateTime);

        return String.Format("{0:HH:mm}", dateTime);
    }

    /// <summary>
    /// Return a convenient string representation for displaying a duration to users,
    /// Format : xh xm
    /// </summary>
    /// <param name="ms">duration in milliseconds</param>
    /// <returns>a human readable representation of a duration</returns>
    public static string ConvertDurationToHumanReadableString(long ms)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(ms);

        return string.Format("{0:D2}h {1:D2}m",
                        t.Hours,
                        t.Minutes);
    }
}

