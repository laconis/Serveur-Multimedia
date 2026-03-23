using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newtube.Models
{
    public class Video
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [MaxLength(64)]
        public string? HashSha256 { get; set; }

        [MaxLength(500)]
        public string? CoverPath { get; set; }

        public long SizeBytes { get; set; }

        public int DurationSec { get; set; }

        [MaxLength(50)]
        public string? Resolution { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsPublished { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public long Views { get; set; } = 0;

        // Clé étrangère catégorie
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Optionnel : utilisateur qui a uploadé
        [MaxLength(100)]
        public string? UploadedBy { get; set; }

        [NotMapped]
        public string DurationFormatted
        {
            get
            {
                return TimeSpan.FromSeconds(DurationSec).ToString(@"hh\:mm\:ss");
            }
        }


        //Propriété calculé à la création de la vidéo
        [NotMapped]
        public string ThunbnailUrl => string.isNullOrWhiteSpace(CoverPath) ? "/images/thumbDefault" : Coverpath;


        
        [NotMapped]
        public string SizeFormatted
        {
            get
            {
                if (SizeBytes < 1024)
                    return $"{SizeBytes} B";

                if (SizeBytes < 1024 * 1024)
                    return $"{SizeBytes / 1024.0:F2} KB";

                if (SizeBytes < 1024 * 1024 * 1024)
                    return $"{SizeBytes / 1024.0 / 1024.0:F2} MB";

                return $"{SizeBytes / 1024.0 / 1024.0 / 1024.0:F2} GB";
            }
        }
    }

    public class VideoMetadataService
{
    public (int durationSec, string resolution) GetMetadata(string videoPath)
    {
        var ffProbe = new FFProbe();
        var info = ffProbe.GetMediaInfo(videoPath);

        int durationSec = (int)info.Duration.TotalSeconds;

        var videoStream = info.Streams.First(s => s.CodecType == "video");
        int width = videoStream.Width.GetValueOrDefault();
        int height = videoStream.Height.GetValueOrDefault();
        string resolution = $"{width}x{height}";

        return (durationSec, resolution);
    }
}


public class ThumbnailService
{
    //retourne String pour la création du Thumb // 
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
}
