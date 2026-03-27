using YnabReports.Configuration;
using YnabReports.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// YNAB API configuration
var ynabSection = builder.Configuration.GetSection(YnabApiOptions.SectionName);
builder.Services.Configure<YnabApiOptions>(ynabSection);

var ynabOptions = ynabSection.Get<YnabApiOptions>();
if (string.IsNullOrWhiteSpace(ynabOptions?.AccessToken))
{
    throw new InvalidOperationException(
        "YNAB API access token is not configured. Set 'YnabApi:AccessToken' in appsettings.json or user secrets.");
}

// YNAB API client with typed HttpClient
builder.Services.AddTransient<YnabAuthHandler>();
builder.Services.AddHttpClient<IYnabApiClient, YnabApiClient>(client =>
{
    client.BaseAddress = new Uri(ynabOptions.BaseUrl.TrimEnd('/') + "/");
}).AddHttpMessageHandler<YnabAuthHandler>();

builder.Services.AddSingleton<TargetProportionCalculator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
