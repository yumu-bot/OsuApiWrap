using System.Reflection;
using System.Runtime.Serialization;
using osu.Game.Rulesets.Scoring;

namespace OsuApi.Helpers;

public static class HitResultParser
{
    private static readonly Dictionary<string, HitResult> HitResultMap = Build();
    private static Dictionary<string, HitResult> Build()
    {
        var type = typeof(HitResult);

        return type
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => new
            {
                Field = f,
                Attr = f.GetCustomAttribute<EnumMemberAttribute>()
            })
            .Where(x => x.Attr?.Value != null)
            .ToDictionary(
                x => x.Attr!.Value!,
                x => (HitResult)x.Field.GetValue(null)!
            );
    }
    
    public static bool TryParseHitResult(string value, out HitResult result)
    {
        return HitResultMap.TryGetValue( value, out  result);
    }
}
