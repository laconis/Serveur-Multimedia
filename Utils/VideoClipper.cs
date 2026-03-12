using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

public static class VideoClipper
{
    public static bool CreateClipAroundTimestamp(
        string inputPath,
        string outputPath,
        string timestamp,
        int minutesBefore = 2,
        int minutesAfter = 2,
        bool reencode = false)
    {
        if (!File.Exists(inputPath))
            throw new FileNotFoundException("Fichier vidéo introuvable.", inputPath);

        timestamp = NormalizeTimestamp(timestamp);
        if (timestamp == null)
            throw new ArgumentException("Timestamp invalide. Format attendu : HH:MM:SS ou H:M:S");

        TimeSpan center = TimeSpan.Parse(timestamp);
        TimeSpan start = center - TimeSpan.FromMinutes(minutesBefore);
        TimeSpan end = center + TimeSpan.FromMinutes(minutesAfter);

        if (start < TimeSpan.Zero)
            start = TimeSpan.Zero;

        TimeSpan duration = end - start;

        string ffmpegPath = "ffmpeg";

        string codecArgs = reencode
            ? "-c:v libx264 -c:a aac"
            : "-c copy";

        string args =
            $"-y -ss {FormatTimeSpan(start)} -i \"{inputPath}\" -t {FormatTimeSpan(duration)} {codecArgs} \"{outputPath}\"";

        var psi = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = args,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        process.Start();

        string ffmpegLog = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Console.WriteLine("Erreur FFmpeg :");
            Console.WriteLine(ffmpegLog);
            return false;
        }

        return File.Exists(outputPath);
    }

    public static string NormalizeTimestamp(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var match = Regex.Match(input.Trim(), @"^(\d{1,2}):(\d{1,2}):(\d{1,2})$");
        if (!match.Success)
            return null;

        int h = int.Parse(match.Groups[1].Value);
        int m = int.Parse(match.Groups[2].Value);
        int s = int.Parse(match.Groups[3].Value);

        if (h > 23 || m > 59 || s > 59)
            return null;

        return $"{h:D2}:{m:D2}:{s:D2}";
    }

    private static string FormatTimeSpan(TimeSpan ts)
    {
        return $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
    }
}
