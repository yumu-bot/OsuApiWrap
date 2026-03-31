using System.Reflection;
using System.Runtime.Serialization;
using osu.Game.Rulesets.Scoring;

namespace OsuApi.Helpers;

public static class HitResultParser
{
    public static bool TryParseHitResult(string value, out HitResult result)
    {
        if (Enum.TryParse(value, true, out result))
            return true;

        foreach (var field in typeof(HitResult).GetFields())
        {
            var attr = field.GetCustomAttribute<EnumMemberAttribute>();
            if (attr?.Value != null && attr.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                result = (HitResult)field.GetValue(null)!;
                return true;
            }
        }

        return false;
    }
}
