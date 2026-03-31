using Microsoft.AspNetCore.Mvc;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using OsuApi.Models;
using OsuApi.Helpers;

namespace OsuApi.Controllers;

[ApiController]
[Route("calculate")]
public class CalculateController : ControllerBase
{
    private readonly OsuCalculator _calculator;

    public CalculateController(OsuCalculator calculator)
    {
        _calculator = calculator;
    }

    [HttpPost]
    public IActionResult Calculate([FromBody] CalculateRequest request)
    {
        if (!System.IO.File.Exists(request.File))
        {
            return NotFound(new { message = "Beatmap file not found" });
        }

        try
        {
            using var stream = System.IO.File.OpenRead(request.File);
            var map = _calculator.GetBeatmap(stream);
            var ruleset = OsuCalculator.GetRuleset(request.Mode);
            var (_, beatmap) = _calculator.CalculateDifficulty(map, ruleset);

            var scoreInfo = new ScoreInfo(beatmap: beatmap.BeatmapInfo, ruleset: ruleset.RulesetInfo);
            if (request.Score != null)
            {
                scoreInfo.Accuracy = request.Score.Accuracy;
                scoreInfo.MaxCombo = request.Score.MaxCombo;
                scoreInfo.Statistics = new Dictionary<HitResult, int>();

                if (request.Score.Statistics != null)
                {
                    foreach (var (key, value) in request.Score.Statistics)
                    {
                        if (HitResultParser.TryParseHitResult(key, out var result))
                        {
                            scoreInfo.Statistics[result] = value;
                        }
                    }
                }

                if (request.Score.Mods != null)
                {
                    var selectedMods = new List<Mod>();

                    foreach (var modReq in request.Score.Mods)
                    {
                        var newMod = modReq.ToMod(ruleset);
                        selectedMods.Add(newMod);
                    }
                    
                    scoreInfo.Mods = selectedMods.ToArray();
                }
            }

            var (detailedAttr, detailedBeatmap) = _calculator.CalculateDifficulty(map, ruleset, scoreInfo.Mods);

            var response = new {
                difficulty = detailedAttr,
                performance = (object?)null
            };

            if (request.Score != null)
            {
                var perfAttributes = ruleset.CreatePerformanceCalculator()?.Calculate(scoreInfo, detailedAttr);
                if (perfAttributes != null)
                {
                    return Ok(new
                    {
                        difficulty = detailedAttr,
                        performance = perfAttributes
                    });
                }
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
