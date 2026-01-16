using TodayApi.Models;

namespace TodayApi.Data;

/// <summary>
/// Static data source for holidays and observances.
///
/// DATA SOURCE EXPLANATION:
/// This is an in-memory, hardcoded data source for demonstration purposes.
/// It includes:
/// - US Federal Holidays (fixed dates and calculated floating holidays)
/// - International UN-recognized observances
/// - Notable historical facts
///
/// HOW TO EXTEND OR SWAP:
/// 1. Database: Replace with Entity Framework Core queries to a holidays table
/// 2. External API: Use services like Calendarific, Abstract API, or Nager.Date
/// 3. JSON File: Load from a JSON configuration file for easy updates
/// 4. Hybrid: Combine multiple sources with a fallback strategy
///
/// To swap the data source:
/// 1. Create a new class implementing IEventDataSource
/// 2. Register it in Program.cs instead of this implementation
/// </summary>
public interface IEventDataSource
{
    /// <summary>
    /// Gets all events for a specific month and day.
    /// </summary>
    IEnumerable<SpecialEvent> GetEventsForDate(int month, int day);

    /// <summary>
    /// Gets floating holidays that depend on year calculations (e.g., Thanksgiving).
    /// </summary>
    IEnumerable<SpecialEvent> GetFloatingHolidaysForDate(DateOnly date);
}

public sealed class InMemoryEventDataSource : IEventDataSource
{
    // Fixed-date events keyed by (Month, Day)
    private static readonly Dictionary<(int Month, int Day), List<SpecialEvent>> FixedDateEvents = new()
    {
        // January
        [(1, 1)] = [
            new() { Name = "New Year's Day", Type = "PublicHoliday", Region = "US", Description = "Federal holiday celebrating the first day of the year" },
            new() { Name = "World Day of Peace", Type = "InternationalDay", Description = "UN-recognized day promoting peace worldwide" }
        ],
        [(1, 15)] = [
            new() { Name = "Martin Luther King Jr. Day (observed)", Type = "PublicHoliday", Region = "US", Description = "Federal holiday honoring Dr. Martin Luther King Jr. (actual date varies)" }
        ],
        [(1, 16)] = [
            new() { Name = "National Nothing Day", Type = "Observance", Region = "US", Description = "A day to just sit without celebrating anything" },
            new() { Name = "Religious Freedom Day", Type = "Observance", Region = "US", Description = "Commemorates the Virginia Statute for Religious Freedom (1786)" }
        ],
        [(1, 27)] = [
            new() { Name = "International Holocaust Remembrance Day", Type = "InternationalDay", Description = "UN-designated day commemorating the victims of the Holocaust" }
        ],

        // February
        [(2, 2)] = [
            new() { Name = "Groundhog Day", Type = "Observance", Region = "US", Description = "Traditional day for predicting the arrival of spring" }
        ],
        [(2, 14)] = [
            new() { Name = "Valentine's Day", Type = "Observance", Description = "Day celebrating love and affection" }
        ],

        // March
        [(3, 8)] = [
            new() { Name = "International Women's Day", Type = "InternationalDay", Description = "UN-recognized day celebrating women's achievements" }
        ],
        [(3, 14)] = [
            new() { Name = "Pi Day", Type = "Observance", Description = "Celebration of the mathematical constant pi (3.14)" }
        ],
        [(3, 17)] = [
            new() { Name = "St. Patrick's Day", Type = "Observance", Description = "Cultural and religious celebration of Irish heritage" }
        ],

        // April
        [(4, 1)] = [
            new() { Name = "April Fools' Day", Type = "Observance", Description = "Day for playing practical jokes and hoaxes" }
        ],
        [(4, 22)] = [
            new() { Name = "Earth Day", Type = "InternationalDay", Description = "Annual event demonstrating support for environmental protection" }
        ],

        // May
        [(5, 1)] = [
            new() { Name = "International Workers' Day", Type = "InternationalDay", Description = "Celebration of laborers and the working class" }
        ],
        [(5, 5)] = [
            new() { Name = "Cinco de Mayo", Type = "Observance", Region = "US", Description = "Celebration of Mexican heritage and pride" }
        ],

        // June
        [(6, 14)] = [
            new() { Name = "Flag Day", Type = "Observance", Region = "US", Description = "Commemorates the adoption of the US flag" }
        ],
        [(6, 19)] = [
            new() { Name = "Juneteenth", Type = "PublicHoliday", Region = "US", Description = "Federal holiday commemorating the end of slavery in the US" }
        ],
        [(6, 21)] = [
            new() { Name = "International Day of Yoga", Type = "InternationalDay", Description = "UN-recognized day promoting yoga worldwide" }
        ],

        // July
        [(7, 4)] = [
            new() { Name = "Independence Day", Type = "PublicHoliday", Region = "US", Description = "Federal holiday celebrating the Declaration of Independence" }
        ],

        // August
        [(8, 19)] = [
            new() { Name = "World Humanitarian Day", Type = "InternationalDay", Description = "UN-designated day honoring humanitarian workers" }
        ],

        // September
        [(9, 11)] = [
            new() { Name = "Patriot Day", Type = "Observance", Region = "US", Description = "Day of remembrance for the September 11, 2001 attacks" }
        ],
        [(9, 21)] = [
            new() { Name = "International Day of Peace", Type = "InternationalDay", Description = "UN-designated day devoted to peace" }
        ],

        // October
        [(10, 31)] = [
            new() { Name = "Halloween", Type = "Observance", Description = "Traditional celebration with costumes and trick-or-treating" }
        ],

        // November
        [(11, 11)] = [
            new() { Name = "Veterans Day", Type = "PublicHoliday", Region = "US", Description = "Federal holiday honoring military veterans" }
        ],

        // December
        [(12, 25)] = [
            new() { Name = "Christmas Day", Type = "PublicHoliday", Region = "US", Description = "Federal holiday celebrating Christmas" }
        ],
        [(12, 31)] = [
            new() { Name = "New Year's Eve", Type = "Observance", Description = "Celebration of the last day of the year" }
        ]
    };

    // Historical facts keyed by (Month, Day)
    private static readonly Dictionary<(int Month, int Day), List<SpecialEvent>> HistoricalFacts = new()
    {
        [(1, 16)] = [
            new() { Name = "Prohibition began (1920)", Type = "HistoricalFact", Description = "The 18th Amendment went into effect, banning alcohol in the US" },
            new() { Name = "First Gulf War began (1991)", Type = "HistoricalFact", Description = "Operation Desert Storm launched against Iraq" }
        ],
        [(7, 20)] = [
            new() { Name = "Moon Landing (1969)", Type = "HistoricalFact", Description = "Apollo 11 astronauts became the first humans to walk on the Moon" }
        ],
        [(11, 9)] = [
            new() { Name = "Fall of the Berlin Wall (1989)", Type = "HistoricalFact", Description = "The Berlin Wall was opened, leading to German reunification" }
        ],
        [(12, 17)] = [
            new() { Name = "First Powered Flight (1903)", Type = "HistoricalFact", Description = "The Wright Brothers achieved the first sustained powered flight" }
        ]
    };

    public IEnumerable<SpecialEvent> GetEventsForDate(int month, int day)
    {
        var events = new List<SpecialEvent>();

        if (FixedDateEvents.TryGetValue((month, day), out var fixedEvents))
        {
            events.AddRange(fixedEvents);
        }

        if (HistoricalFacts.TryGetValue((month, day), out var facts))
        {
            events.AddRange(facts);
        }

        return events;
    }

    public IEnumerable<SpecialEvent> GetFloatingHolidaysForDate(DateOnly date)
    {
        var events = new List<SpecialEvent>();

        // Martin Luther King Jr. Day: Third Monday in January
        if (date == GetNthWeekdayOfMonth(date.Year, 1, DayOfWeek.Monday, 3))
        {
            events.Add(new SpecialEvent
            {
                Name = "Martin Luther King Jr. Day",
                Type = "PublicHoliday",
                Region = "US",
                Description = "Federal holiday honoring Dr. Martin Luther King Jr."
            });
        }

        // Presidents' Day: Third Monday in February
        if (date == GetNthWeekdayOfMonth(date.Year, 2, DayOfWeek.Monday, 3))
        {
            events.Add(new SpecialEvent
            {
                Name = "Presidents' Day",
                Type = "PublicHoliday",
                Region = "US",
                Description = "Federal holiday honoring US presidents"
            });
        }

        // Memorial Day: Last Monday in May
        if (date == GetLastWeekdayOfMonth(date.Year, 5, DayOfWeek.Monday))
        {
            events.Add(new SpecialEvent
            {
                Name = "Memorial Day",
                Type = "PublicHoliday",
                Region = "US",
                Description = "Federal holiday honoring those who died in military service"
            });
        }

        // Labor Day: First Monday in September
        if (date == GetNthWeekdayOfMonth(date.Year, 9, DayOfWeek.Monday, 1))
        {
            events.Add(new SpecialEvent
            {
                Name = "Labor Day",
                Type = "PublicHoliday",
                Region = "US",
                Description = "Federal holiday honoring the American labor movement"
            });
        }

        // Columbus Day / Indigenous Peoples' Day: Second Monday in October
        if (date == GetNthWeekdayOfMonth(date.Year, 10, DayOfWeek.Monday, 2))
        {
            events.Add(new SpecialEvent
            {
                Name = "Columbus Day / Indigenous Peoples' Day",
                Type = "PublicHoliday",
                Region = "US",
                Description = "Federal holiday (observed differently across states)"
            });
        }

        // Thanksgiving: Fourth Thursday in November
        if (date == GetNthWeekdayOfMonth(date.Year, 11, DayOfWeek.Thursday, 4))
        {
            events.Add(new SpecialEvent
            {
                Name = "Thanksgiving Day",
                Type = "PublicHoliday",
                Region = "US",
                Description = "Federal holiday for giving thanks"
            });
        }

        // Mother's Day: Second Sunday in May
        if (date == GetNthWeekdayOfMonth(date.Year, 5, DayOfWeek.Sunday, 2))
        {
            events.Add(new SpecialEvent
            {
                Name = "Mother's Day",
                Type = "Observance",
                Region = "US",
                Description = "Day honoring mothers and motherhood"
            });
        }

        // Father's Day: Third Sunday in June
        if (date == GetNthWeekdayOfMonth(date.Year, 6, DayOfWeek.Sunday, 3))
        {
            events.Add(new SpecialEvent
            {
                Name = "Father's Day",
                Type = "Observance",
                Region = "US",
                Description = "Day honoring fathers and fatherhood"
            });
        }

        return events;
    }

    /// <summary>
    /// Gets the Nth occurrence of a weekday in a given month.
    /// Example: 3rd Monday of January 2024.
    /// </summary>
    private static DateOnly GetNthWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek, int n)
    {
        var firstOfMonth = new DateOnly(year, month, 1);
        var daysUntilTarget = ((int)dayOfWeek - (int)firstOfMonth.DayOfWeek + 7) % 7;
        var firstOccurrence = firstOfMonth.AddDays(daysUntilTarget);
        return firstOccurrence.AddDays(7 * (n - 1));
    }

    /// <summary>
    /// Gets the last occurrence of a weekday in a given month.
    /// Example: Last Monday of May 2024.
    /// </summary>
    private static DateOnly GetLastWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek)
    {
        var lastOfMonth = new DateOnly(year, month, DateTime.DaysInMonth(year, month));
        var daysAfterTarget = ((int)lastOfMonth.DayOfWeek - (int)dayOfWeek + 7) % 7;
        return lastOfMonth.AddDays(-daysAfterTarget);
    }
}
