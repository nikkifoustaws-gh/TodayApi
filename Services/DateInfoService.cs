using TodayApi.Data;
using TodayApi.Models;

namespace TodayApi.Services;

/// <summary>
/// Service responsible for determining what's special about today's date.
/// </summary>
public interface IDateInfoService
{
    /// <summary>
    /// Gets information about today's date in Eastern Time.
    /// </summary>
    TodayResponse GetTodayInfo();
}

public sealed class DateInfoService : IDateInfoService
{
    private readonly IEventDataSource _eventDataSource;
    private readonly TimeZoneInfo _easternTimeZone;

    // Timezone identifier for Eastern Time
    // "America/New_York" is the IANA identifier (cross-platform: Linux, macOS, Windows with ICU)
    // "Eastern Standard Time" is the Windows identifier (Windows only)
    private const string IanaTimezoneId = "America/New_York";
    private const string WindowsTimezoneId = "Eastern Standard Time";

    public DateInfoService(IEventDataSource eventDataSource)
    {
        _eventDataSource = eventDataSource;
        _easternTimeZone = GetEasternTimeZone();
    }

    /// <summary>
    /// Gets the Eastern Time TimeZoneInfo in a cross-platform manner.
    ///
    /// TIMEZONE HANDLING EXPLANATION:
    ///
    /// .NET uses different timezone identifiers on different platforms:
    /// - Windows uses Windows timezone IDs (e.g., "Eastern Standard Time")
    /// - Linux/macOS use IANA timezone IDs (e.g., "America/New_York")
    ///
    /// Starting with .NET 6+, TimeZoneInfo.FindSystemTimeZoneById can automatically
    /// convert between formats when ICU (International Components for Unicode) is available.
    /// However, for maximum compatibility, we try IANA first (more portable), then Windows.
    ///
    /// DAYLIGHT SAVING TIME:
    /// TimeZoneInfo automatically handles DST transitions. When we convert UTC to Eastern,
    /// it uses the correct offset based on whether DST is in effect:
    /// - EST (Eastern Standard Time): UTC-5 (November - March)
    /// - EDT (Eastern Daylight Time): UTC-4 (March - November)
    ///
    /// The conversion is based on the official US DST rules, which .NET keeps updated.
    /// </summary>
    private static TimeZoneInfo GetEasternTimeZone()
    {
        // Try IANA identifier first (cross-platform, preferred)
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(IanaTimezoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            // Fall back to Windows identifier
            return TimeZoneInfo.FindSystemTimeZoneById(WindowsTimezoneId);
        }
    }

    public TodayResponse GetTodayInfo()
    {
        // Step 1: Get current UTC time
        // We start with UTC to ensure consistency regardless of server location.
        var utcNow = DateTime.UtcNow;

        // Step 2: Convert UTC to Eastern Time
        // TimeZoneInfo.ConvertTimeFromUtc handles DST automatically.
        // If it's March 10th at 2:30 AM (during DST transition), it correctly
        // "springs forward" to 3:30 AM EDT.
        var easternNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _easternTimeZone);

        // Step 3: Extract the date (ignoring time component)
        // This gives us "today" in Eastern Time, which may differ from UTC date
        // around midnight. For example:
        // - UTC: 2024-01-16 03:00:00 (3 AM UTC)
        // - Eastern: 2024-01-15 22:00:00 (10 PM previous day)
        var easternDate = DateOnly.FromDateTime(easternNow);

        // Step 4: Check if DST is currently in effect
        var isDst = _easternTimeZone.IsDaylightSavingTime(easternNow);

        // Step 5: Gather all events for this date
        var events = new List<SpecialEvent>();

        // Get fixed-date events (holidays on specific month/day)
        events.AddRange(_eventDataSource.GetEventsForDate(easternDate.Month, easternDate.Day));

        // Get floating holidays (depend on year, e.g., "4th Thursday in November")
        events.AddRange(_eventDataSource.GetFloatingHolidaysForDate(easternDate));

        // Step 6: Build the response message
        var message = BuildMessage(easternDate, events);

        return new TodayResponse
        {
            Date = easternDate.ToString("yyyy-MM-dd"),
            DayOfWeek = easternDate.DayOfWeek.ToString(),
            Timezone = isDst ? "America/New_York (EDT, UTC-4)" : "America/New_York (EST, UTC-5)",
            IsDaylightSavingTime = isDst,
            Events = events,
            Message = message
        };
    }

    private static string BuildMessage(DateOnly date, List<SpecialEvent> events)
    {
        if (events.Count == 0)
        {
            return $"Today is {date:MMMM d, yyyy}. While there are no widely recognized holidays or observances, " +
                   "every day is an opportunity to make something special happen!";
        }

        var holidays = events.Where(e => e.Type == "PublicHoliday").ToList();
        var observances = events.Where(e => e.Type is "Observance" or "InternationalDay").ToList();
        var facts = events.Where(e => e.Type == "HistoricalFact").ToList();

        var parts = new List<string>();

        if (holidays.Count > 0)
        {
            var names = string.Join(" and ", holidays.Select(h => h.Name));
            parts.Add($"Today is {names}!");
        }

        if (observances.Count > 0)
        {
            var names = string.Join(", ", observances.Select(o => o.Name));
            parts.Add(holidays.Count > 0
                ? $"It's also {names}."
                : $"Today marks {names}.");
        }

        if (facts.Count > 0)
        {
            var factDesc = string.Join(" Also, ", facts.Select(f => $"on this day: {f.Name}"));
            parts.Add($"Historical note - {factDesc}.");
        }

        return string.Join(" ", parts);
    }
}
