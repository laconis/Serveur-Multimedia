using FFMpegCore;

public class ThumbnailService
{
    public async Task<string> GenerateThumbnailAsync(string videoPath, string outputPath)
    {
        // Exemple : capture à 5 secondes, 320x180
        await FFMpegArguments
            .FromFileInput(videoPath)
            .OutputToFile(outputPath, overwrite: true, options => options
                .Seek(TimeSpan.FromSeconds(5))
                .WithFrameOutputCount(1)
                .WithVideoFilters(f => f.Scale(320, 180))
            )
            .ProcessAsynchronously();

        return outputPath;
    }
}
