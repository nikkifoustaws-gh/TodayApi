namespace TodayApi.Models;

/// <summary>
/// Response model for the /today endpoint.
/// Contains information about what makes today special.
/// </summary>
public sealed record TodayResponse
{
    /// <summary>
    /// The current date in Eastern Time (America/New_York), formatted as yyyy-MM-dd.
    /// </summary>
    public required string Date { get; init; }

    /// <summary>
    /// The day of the week (e.g., "Monday", "Tuesday").
    /// </summary>
    public required string DayOfWeek { get; init; }

    /// <summary>
    /// The timezone used for date calculation.
    /// </summary>
    public required string Timezone { get; init; }

    /// <summary>
    /// Whether the timezone is currently observing Daylight Saving Time.
    /// </summary>
    public required bool IsDaylightSavingTime { get; init; }

    /// <summary>
    /// List of special events, holidays, or observances for today.
    /// </summary>
    public required IReadOnlyList<SpecialEvent> Events { get; init; }

    /// <summary>
    /// A friendly message summarizing what's special about today.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Who is asking for today's information.
    /// </summary>
    public required string WhoIsAsking { get; init; }
}

/// <summary>
/// Represents a special event, holiday, or observance.
/// </summary>
public sealed record SpecialEvent
{
    /// <summary>
    /// Name of the event or holiday.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Type of event: "PublicHoliday", "Observance", "InternationalDay", "HistoricalFact".
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Optional description providing more context about the event.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Country or region where this event is observed (null for international events).
    /// </summary>
    public string? Region { get; init; }
}
