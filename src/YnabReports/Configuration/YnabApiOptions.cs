namespace YnabReports.Configuration;

public class YnabApiOptions
{
    public const string SectionName = "YnabApi";

    public string BaseUrl { get; set; } = "https://api.ynab.com/v1";
    public string AccessToken { get; set; } = string.Empty;
}
