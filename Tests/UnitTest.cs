using System.Text;
using Newtonsoft.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using osu.Framework.Extensions.ObjectExtensions;
using Xunit.Abstractions;

namespace OsuApi.Tests;

public class UnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    // nm hr dt dt*1.2 dt+hr
    private readonly List<JArray> _mods = new()
    {
        JArray.Parse("[]"),
        JArray.Parse(""" [{"acronym": "HR"}] """),
        JArray.Parse(""" [{"acronym": "DT"}] """),
        JArray.Parse(""" [{"acronym": "DT", "settings": {"speed_change": 1.2}}] """),
        JArray.Parse(""" [{"acronym": "DT"}, {"acronym": "HR"}] """),
    };

    // nm hr dt dt*1.2 dt+hr
    private readonly Dictionary<string, List<double>> _testResult =
        new()
        {
            ["osu"] = new List<double> { 5.95, 6.38, 9.19, 7.05, 10.22 },
            ["mania.4k"] = new List<double> { 3.03, 3.03, 4.10, 3.44, 4.10 },
            ["mania.7k"] = new List<double> { 4.88, 4.88, 6.78, 5.64, 6.78 },
            ["taiko"] = new List<double> { 3.82, 3.90, 5.30, 4.43, 5.97 },
            ["catch"] = new List<double> { 3.65, 4.71, 5.13, 4.25, 6.44 },
        };

    public UnitTest(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        Directory.SetCurrentDirectory("../../../");
        _output = output;
        _client = factory.WithWebHostBuilder(b => b.UseContentRoot("./")).CreateClient();
    }

    private async Task<JObject> Reqeust(string point, JObject body)
    {
        var response = await _client.PostAsync(
            "/calculate",
            new StringContent(body.ToString(), Encoding.UTF8, "application/json")
        );
        var text = await response.Content.ReadAsStringAsync();
        return JObject.Parse(text);
    }

    private bool check(double d1, double d2)
    {
        return Math.Abs(d1 - d2) < 0.01;
    }

    private async Task testOne(string file)
    {
        var path = $"{Directory.GetCurrentDirectory()}/file/{file}.osu";
        _output.WriteLine(path);
        var mode = file.Split('.').First();
        var json = JObject.Parse("{}");
        json["file"] = path;
        json["mode"] = mode;
        var score = new JObject();
        for (int i = 0; i < _mods.Count; i++)
        {
            score["mods"] = _mods[i];
            json["score"] = score;
            var result = await Reqeust("calculate", json);
            var star = (double)result["difficulty"]["star_rating"];
            var isOk = check(star, _testResult[file][i]);
            _output.WriteLine($"t{i} ==> {isOk}");
            if (!isOk)
            {
                _output.WriteLine($" #{star} != {_testResult[file][i]}");
            }
        }
    }

    [Fact]
    public async Task TestCalculate()
    {
        foreach (var (key, _) in _testResult)
        {
            await testOne(key);
        }
    }
}