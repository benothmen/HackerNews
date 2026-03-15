namespace Infrastructure.Models;

public class HackerNewsItem
{
    public string By { get; set; } = string.Empty;
    public int Descendants { get; set; }
    public long Id { get; set; }
    public int Score { get; set; }
    public long Time { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}