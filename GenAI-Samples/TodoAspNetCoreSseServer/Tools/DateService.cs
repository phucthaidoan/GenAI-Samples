using ModelContextProtocol.Server;
using System.ComponentModel;

namespace TodoAspNetCoreSseServer.Tools;

[McpServerToolType]
public sealed class DateService
{
    [McpServerTool(Name = "getToday"), Description("Gets today's date with optional time")]
    public static DateTime GetToday(
        [Description("Time of day (e.g. '09:00', '23:59', or 'eod' for end of day). Defaults to end of day.")] 
        string timeOfDay = "eod")
    {
        var today = DateTime.Today;

        if (string.Equals(timeOfDay, "eod", StringComparison.OrdinalIgnoreCase))
        {
            return today.AddHours(23).AddMinutes(59);
        }

        if (TimeSpan.TryParse(timeOfDay, out TimeSpan time))
        {
            return today.Add(time);
        }

        // Default to end of day if time parsing fails
        return today.AddHours(23).AddMinutes(59);
    }

    [McpServerTool(Name = "parseFutureDate"), Description("Parses a natural language date expression into a future DateTime")]
    public static DateTime ParseFutureDate(
        [Description("Natural language date expression (e.g. 'today', 'tomorrow', 'next week', or specific date)")] 
        string dateExpression,
        [Description("Time of day (e.g. '09:00', '23:59', or 'eod' for end of day). Defaults to end of day.")] 
        string timeOfDay = "eod")
    {
        DateTime result;

        // Handle common expressions
        result = dateExpression.ToLower() switch
        {
            "today" => DateTime.Today,
            "tomorrow" => DateTime.Today.AddDays(1),
            "next week" => DateTime.Today.AddDays(7),
            _ => DateTime.Today // Default case, will be overridden if parsing succeeds
        };

        // Try to parse as specific date if not a known expression
        if (!new[] { "today", "tomorrow", "next week" }.Contains(dateExpression.ToLower()))
        {
            if (!DateTime.TryParse(dateExpression, out DateTime parsedDate))
            {
                throw new ArgumentException("Could not understand the date. Try using 'today', 'tomorrow', a specific date, or 'next week'", nameof(dateExpression));
            }
            result = parsedDate.Date;
        }

        // Add time component
        if (string.Equals(timeOfDay, "eod", StringComparison.OrdinalIgnoreCase))
        {
            return result.AddHours(23).AddMinutes(59);
        }

        if (TimeSpan.TryParse(timeOfDay, out TimeSpan time))
        {
            return result.Add(time);
        }

        // Default to end of day if time parsing fails
        return result.AddHours(23).AddMinutes(59);
    }
}