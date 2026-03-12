using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

public class DuplicateFile
{
    public string FullPath { get; set; } = "";
    public string Name { get; set; } = "";
    public long SizeBytes { get; set; }
    public string Sha256 { get; set; } = "";
}

public static class DuplicateScanner
{
    public static List<List<DuplicateFile>> FindDuplicates(
        string rootDir,
        List<string>? extensions = null)
    {
        var result = new List<List<DuplicateFile>>();

        if (!Directory.Exists(rootDir))
        {
            Console.WriteLine($"[ERREUR] Dossier introuvable : {rootDir}");
            return result;
        }

        var allowedExtensions = extensions?
            .Select(NormalizeExtension)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Console.WriteLine("[INFO] Scan des fichiers...");
        var allFiles = Directory
            .GetFiles(rootDir, "*", SearchOption.AllDirectories)
            .Where(f => allowedExtensions == null || allowedExtensions.Contains(Path.GetExtension(f)))
            .ToList();

        Console.WriteLine($"[INFO] {allFiles.Count} fichier(s) trouvé(s).");

        var fileInfos = allFiles
            .Select(path =>
            {
                try
                {
                    var fi = new FileInfo(path);
                    return fi.Exists ? fi : null;
                }
                catch
                {
                    return null;
                }
            })
            .Where(fi => fi != null)
            .Cast<FileInfo>()
            .ToList();

        var sizeGroups = fileInfos
            .GroupBy(f => f.Length)
            .Where(g => g.Count() > 1)
            .ToList();

        Console.WriteLine($"[INFO] {sizeGroups.Count} groupe(s) suspects par taille.");

        int processed = 0;
        int totalToHash = sizeGroups.Sum(g => g.Count());

        foreach (var sizeGroup in sizeGroups)
        {
            var hashedFiles = new List<DuplicateFile>();

            foreach (var file in sizeGroup)
            {
                try
                {
                    hashedFiles.Add(new DuplicateFile
                    {
                        FullPath = file.FullName,
                        Name = file.Name,
                        SizeBytes = file.Length,
                        Sha256 = ComputeSha256(file.FullName)
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine($"[WARN] Impossible de hasher : {file.FullName}");
                    Console.WriteLine($" {ex.Message}");
                }

                processed++;
                DisplayProgress(processed, totalToHash, file.Name);
            }

            var duplicateGroups = hashedFiles
                .GroupBy(f => f.Sha256)
                .Where(g => g.Count() > 1)
                .Select(g => g.ToList());

            result.AddRange(duplicateGroups);
        }

        Console.WriteLine();
        Console.WriteLine("[OK] Analyse terminée.");
        return result;
    }

    public static void PrintReport(List<List<DuplicateFile>> duplicates)
    {
        if (duplicates.Count == 0)
        {
            Console.WriteLine("[INFO] Aucun doublon trouvé.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("========== RAPPORT DOUBLONS ==========");

        long wastedBytes = 0;
        int index = 1;

        foreach (var group in duplicates)
        {
            Console.WriteLine();
            Console.WriteLine($"Groupe #{index}");
            Console.WriteLine($"Taille : {FormatSize(group[0].SizeBytes)}");
            Console.WriteLine($"Hash : {group[0].Sha256}");
            Console.WriteLine($"Copies : {group.Count}");

            foreach (var file in group)
            {
                Console.WriteLine($" - {file.FullPath}");
            }

            wastedBytes += group[0].SizeBytes * (group.Count - 1);
            index++;
        }

        Console.WriteLine();
        Console.WriteLine($"Nombre de groupes de doublons : {duplicates.Count}");
        Console.WriteLine($"Espace potentiellement récupérable : {FormatSize(wastedBytes)}");
    }

    public static string ComputeSha256(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha256.ComputeHash(stream);
        return Convert.ToHexString(hash);
    }

    public static string FormatSize(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        double size = bytes;
        int unit = 0;

        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        return $"{size:0.##} {units[unit]}";
    }

    private static string NormalizeExtension(string ext)
    {
        if (string.IsNullOrWhiteSpace(ext))
            return "";

        return ext.StartsWith('.') ? ext : "." + ext;
    }

    private static void DisplayProgress(int current, int total, string fileName)
    {
        const int barWidth = 30;
        double ratio = total == 0 ? 1 : (double)current / total;
        int filled = (int)(ratio * barWidth);

        string bar = "[" + new string('#', filled) + new string('-', barWidth - filled) + "]";
        Console.Write($"\r{bar} {current}/{total} {fileName,-40}");

        if (current == total)
            Console.WriteLine();
    }
}
