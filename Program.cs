using Newtonsoft.Json.Serialization;
using OsuApi;
using OsuApi.Converters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<OsuCalculator>();
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new TypeJsonConverter());
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
    });
var app = builder.Build();

app.MapControllers();

if (args.Length == 0 || args[0] != "test")
{
    app.Run();
}

public partial class Program
{
}