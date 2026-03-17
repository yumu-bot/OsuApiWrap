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

app.Run();
