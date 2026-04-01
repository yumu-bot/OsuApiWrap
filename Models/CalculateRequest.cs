using Newtonsoft.Json;
using osu.Game.Online.API;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Catch.Mods;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Mania.Mods;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Taiko;
using osu.Game.Rulesets.Taiko.Mods;
using osu.Game.Scoring;
using OsuApi.Helpers;

namespace OsuApi.Models;

public record CalculateRequest(
    [property: JsonProperty("file")] string File,
    [property: JsonProperty("mode")] string Mode,
    [property: JsonProperty("score")] ScoreRequest? Score
)
{
    public void Apply(ScoreInfo scoreInfo, Ruleset ruleset)
    {
        if (Score == null) return;
        Score.Apply(scoreInfo);
        Score.ApplyMods(scoreInfo, ruleset);
        Score.ApplyStatistics(scoreInfo);
        Score.ApplyLegacyTotalScore(scoreInfo);
    }
}

public record ScoreRequest(
    [property: JsonProperty("accuracy")] double Accuracy,
    [property: JsonProperty("max_combo")] int MaxCombo,
    [property: JsonProperty("legacy_total_score")]
    long? LegacyTotalScore,
    [property: JsonProperty("statistics")] Dictionary<string, int>? Statistics,
    [property: JsonProperty("mods")] List<APIMod>? Mods
)
{
    public void Apply(ScoreInfo scoreInfo)
    {
        scoreInfo.Accuracy = Accuracy;
        scoreInfo.MaxCombo = MaxCombo;
    }
    public void ApplyStatistics(ScoreInfo scoreInfo)
    {
        if (Statistics == null) return;
        foreach (var (key, value) in Statistics)
        {
            if (HitResultParser.TryParseHitResult(key, out var result))
            {
                scoreInfo.Statistics[result] = value;
            }
        }
    }
    public void ApplyMods(ScoreInfo scoreInfo, Ruleset ruleset)
    {
        var mods = Mods ?? [];
        
        var selectedMods = new Mod[mods.Count];
        var isNoClassic = true;

        for (var i = 0; i < mods.Count; i++)
        {
            var mod = mods[i].ToMod(ruleset);
            selectedMods[i] = mod;

            if (mod is ModClassic)
                isNoClassic = false;
        }

        if (LegacyTotalScore != null && isNoClassic)
        {
            Mod classic = ruleset switch
            {
                OsuRuleset => new OsuModClassic(),
                TaikoRuleset => new TaikoModClassic(),
                CatchRuleset => new CatchModClassic(),
                ManiaRuleset => new ManiaModClassic(),
                _ => throw new NotSupportedException()
            };
            selectedMods = [..selectedMods, classic];
        }
        scoreInfo.Mods = selectedMods;
    }
    public void ApplyLegacyTotalScore(ScoreInfo scoreInfo)
    {
        if (LegacyTotalScore == null) return;
        scoreInfo.TotalScore = LegacyTotalScore.Value;
        scoreInfo.IsLegacyScore = true;
    }
}
