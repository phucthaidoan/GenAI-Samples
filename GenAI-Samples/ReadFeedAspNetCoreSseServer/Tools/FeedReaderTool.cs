using System.Text.Json.Serialization;
using System.ComponentModel;
using CodeHollow.FeedReader;
using ModelContextProtocol.Server;

namespace ReadFeedAspNetCoreSseServer.Tools;

[McpServerToolType]
public class FeedReaderTool
{
    private readonly ILogger<FeedReaderTool> _logger;

    public FeedReaderTool(ILogger<FeedReaderTool> logger)
    {
        _logger = logger;
    }

    [McpServerTool]
    [Description("Reads a feed from a given URL and returns the most recent entries")]
    public async Task<object> ReadFeed(string url, int maxEntries = 10)
    {
        try
        {
            _logger.LogInformation("Reading feed from {Url}", url);
            
            // Load the feed using CodeHollow.FeedReader
            var feed = await FeedReader.ReadAsync(url);
            
            // Take only the requested number of entries
            var entries = feed.Items
                .Take(maxEntries)
                .Select(item => new FeedEntry
                {
                    Title = item.Title,
                    Summary = item.Description ?? string.Empty,
                    PublishedDate = item.PublishingDate ?? DateTime.UtcNow,
                    Link = item.Link ?? string.Empty,
                    Author = item.Author ?? string.Empty
                })
                .ToList();

            _logger.LogInformation("Retrieved {Count} entries from feed", entries.Count);
            
            return new { Entries = entries };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading feed from {Url}", url);
            throw new Exception($"Failed to read feed: {ex.Message}");
        }
    }

    public class FeedEntry
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;
        
        [JsonPropertyName("publishedDate")]
        public DateTime PublishedDate { get; set; }
        
        [JsonPropertyName("link")]
        public string Link { get; set; } = string.Empty;
        
        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;
    }
}
