using osu.Game.Beatmaps;
namespace OsuApi.Models;

public class CalculateWorkingBeatmap(IBeatmap beatmap) : WorkingBeatmap(beatmap.BeatmapInfo, null)
{
    protected override IBeatmap GetBeatmap() => beatmap;
    public override osu.Framework.Graphics.Textures.Texture GetBackground() => null!;
    protected override osu.Framework.Audio.Track.Track GetBeatmapTrack() => null!;
    protected override osu.Game.Skinning.ISkin GetSkin() => null!;
    public override Stream GetStream(string storagePath) => File.Create(storagePath);
}