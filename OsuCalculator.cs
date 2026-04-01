using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Taiko;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Scoring;
using OsuApi.Models;

namespace OsuApi;

public class OsuCalculator
{
    public Beatmap GetBeatmap(Stream input)
    {
        var reader = new LineBufferedReader(input);
        var decoded = Decoder.GetDecoder<Beatmap>(reader).Decode(reader);
        return decoded;
    }

    private static void ConvertBeatmap(ref IBeatmap beatmap, Ruleset mode)
    {
        if (beatmap.BeatmapInfo.Ruleset.ShortName == mode.ShortName) 
            return;
        
        if (beatmap.BeatmapInfo.Ruleset.ShortName != OsuRuleset.SHORT_NAME) 
            throw new ArgumentException("Invalid mode");
        
        beatmap = mode.CreateBeatmapConverter(beatmap).Convert();
    }


    public (DifficultyAttributes Attributes, IBeatmap Beatmap) CalculateDifficulty(IBeatmap beatmap, Ruleset ruleset, IEnumerable<osu.Game.Rulesets.Mods.Mod>? mods = null)
    {
        ConvertBeatmap(ref beatmap, ruleset);
        var workingBeatmap = new CalculateWorkingBeatmap(beatmap);

        // 4. 计算难度
        var diffCalc = ruleset.CreateDifficultyCalculator(workingBeatmap);
        var attributes = diffCalc.Calculate(mods ?? []);

        return (attributes, beatmap);
    }

    public PerformanceAttributes? Calculate(DifficultyAttributes attributes, ScoreInfo score,  Ruleset ruleset)
    {
        var performance = ruleset.CreatePerformanceCalculator();
        return performance?.Calculate(score, attributes);
    }
    
    public static Ruleset GetRuleset(string mode) => mode.ToLower() switch
    {
        "osu" or "o"=>  new OsuRuleset(),
        "taiko" or "t" =>  new TaikoRuleset(),
        "catch" or "fruits" or "c" or "f" =>  new CatchRuleset(),
        "mania" or "m" =>  new ManiaRuleset(),
        _ => throw new ArgumentException("Invalid mode")
    };
}