using Microsoft.Extensions.Caching.Memory;
using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;
using Nager.PublicSuffix.RuleProviders.CacheProviders;
using Nager.PublicSuffix.WebApi.GitHub;
using System.Text.Json.Serialization;
using System.Web;

var corsPolicyName = "ApiPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ICacheProvider, LocalFileSystemCacheProvider>();
builder.Services.AddSingleton<IRuleProvider, CachedHttpRuleProvider>();
builder.Services.AddSingleton<IDomainParser, DomainParser>();
builder.Services.AddScoped<GitHubClient>();

builder.Services.AddMemoryCache();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(configuration =>
    configuration.AddPolicy(corsPolicyName, builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    })
);

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(configuration =>
{
    configuration.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
}).CreateLogger("Program");

var app = builder.Build();

var ruleProvider = app.Services.GetService<IRuleProvider>();
if (ruleProvider != null)
{
    await ruleProvider.BuildAsync();
}

app.UseCors(corsPolicyName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/DomainInfo/{domain}", (string domain, IDomainParser domainParser) =>
{
    domain = HttpUtility.UrlEncode(domain);

    logger.LogInformation($"Get DomainInfo for {domain}");

    var domainInfo = domainParser.Parse(domain);
    return domainInfo;
})
.WithName("DomainInfo")
.WithOpenApi();

app.MapPost("/CheckLastCommit", async (GitHubClient gitHubClient, IMemoryCache memoryCache, CancellationToken cancellationToken) =>
{
    memoryCache.TryGetValue<DateTime>("lastCheckTime", out var lastCheckTime);
    if (lastCheckTime > DateTime.UtcNow.AddMinutes(-1))
    {
        return Results.StatusCode(StatusCodes.Status429TooManyRequests);
    }

    memoryCache.Set("lastCheckTime", DateTime.UtcNow);

    var lastGitHubCommit = await gitHubClient.GetCommitAsync("publicsuffix", "list", "master", cancellationToken);

    return Results.Content(lastGitHubCommit?.Commit?.Committer?.Date?.ToString());
})
.WithName("CheckLastCommit")
.WithOpenApi();

app.MapPost("/UpdateRules", async (IRuleProvider ruleProvider, IMemoryCache memoryCache) =>
{
    memoryCache.TryGetValue<DateTime>("lastUpdateTime", out var lastUpdateTime);
    if (lastUpdateTime > DateTime.UtcNow.AddMinutes(-10))
    {
        return Results.StatusCode(StatusCodes.Status429TooManyRequests);
    }

    memoryCache.Set("lastUpdateTime", DateTime.UtcNow);

    await ruleProvider.BuildAsync(ignoreCache: true);
    return Results.NoContent();
})
.WithName("UpdateRules")
.WithOpenApi();

app.Run();

