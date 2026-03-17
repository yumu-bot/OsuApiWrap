using Newtonsoft.Json;
using osu.Game.Online.API;

namespace OsuApi.Models;

public record CalculateRequest(
    [property: JsonProperty("file")] string File,
    [property: JsonProperty("mode")] string Mode,
    [property: JsonProperty("score")] ScoreRequest? Score
);

public record ScoreRequest(
    [property: JsonProperty("accuracy")] double Accuracy,
    [property: JsonProperty("max_combo")] int MaxCombo,
    [property: JsonProperty("statistics")] Dictionary<string, int>? Statistics,
    [property: JsonProperty("mods")] List<APIMod>? Mods
);
