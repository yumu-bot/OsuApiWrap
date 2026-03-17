using osu.Game.Beatmaps;
namespace OsuApi;

public class CalculateWorkingBeatmap : WorkingBeatmap
{
    private readonly IBeatmap beatmap;
    public CalculateWorkingBeatmap(IBeatmap beatmap) : base(beatmap.BeatmapInfo, null) => this.beatmap = beatmap;
    protected override IBeatmap GetBeatmap() => beatmap;
    public override osu.Framework.Graphics.Textures.Texture GetBackground() => null!;
    protected override osu.Framework.Audio.Track.Track GetBeatmapTrack() => null!;
    protected override osu.Game.Skinning.ISkin GetSkin() => null!;
    public override Stream GetStream(string storagePath) => File.Create(storagePath);
}