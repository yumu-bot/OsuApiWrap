using Microsoft.AspNetCore.Mvc;
using osu.Game.Scoring;
using OsuApi.Models;

namespace OsuApi.Controllers;

[ApiController]
[Route("calculate")]
public class CalculateController : ControllerBase
{
    private readonly OsuCalculator _calculator = new ();
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
            
            request.Apply(scoreInfo, ruleset);
            
            var (detailedAttr, _) = _calculator.CalculateDifficulty(map, ruleset, scoreInfo.Mods);

            if (request.Score == null) return Ok(new {
                difficulty = detailedAttr,
                performance = (object?)null
            });
            
            var perfAttributes = _calculator.Calculate(detailedAttr, scoreInfo, ruleset);
           
            return Ok(new
            {
                difficulty = detailedAttr,
                performance = perfAttributes
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
