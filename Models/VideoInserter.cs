using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Security.Cryptography;

//dotnet add package NReco.VideoInfo
//dotnet add package FFMpegCore
class VideoInserter
{
    static void Main()
    {
        string connectionString = "Server=localhost;Database=nicoflix;Uid=root;Pwd=;";
        string videoPath = @"D:\ServeurMultimédia\www\";   
        string coverPath = @"D:\ServeurMultimédia\www\thumb\"; 
        int categoryId = 1;

        FileInfo fileInfo = new FileInfo(videoPath);

        string fileName = fileInfo.Name;
        long sizeBytes = fileInfo.Length;

        string sha256 = ComputeSHA256(videoPath);

        // Durée fictive ici (à remplacer par extraction réelle)
        int durationSec = 120;  

        string resolution = "1920x1080";

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();

            string query = @"
                INSERT INTO videos 
                (title, description, file_path, file_name, hash_sha256, cover_path, 
                 size_bytes, duration_sec, resolution, updated_at, category_id, uploaded_by)
                VALUES 
                (@title, @description, @file_path, @file_name, @hash_sha256, @cover_path,
                 @size_bytes, @duration_sec, @resolution, NOW(), @category_id, @uploaded_by)
            ";

            MySqlCommand cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@title", "Ma vidéo de test");
            cmd.Parameters.AddWithValue("@description", "Vidéo d'exemple insérée via C#");
            cmd.Parameters.AddWithValue("@file_path", videoPath);
            cmd.Parameters.AddWithValue("@file_name", fileName);
            cmd.Parameters.AddWithValue("@hash_sha256", sha256);
            cmd.Parameters.AddWithValue("@cover_path", coverPath);
            cmd.Parameters.AddWithValue("@size_bytes", sizeBytes);
            cmd.Parameters.AddWithValue("@duration_sec", durationSec);
            cmd.Parameters.AddWithValue("@resolution", resolution);
            cmd.Parameters.AddWithValue("@category_id", categoryId);
            cmd.Parameters.AddWithValue("@uploaded_by", Environment.UserName);

            cmd.ExecuteNonQuery();
        }
    }

    static string ComputeSHA256(string filePath)
    {
        using (var sha = SHA256.Create())
        using (var stream = File.OpenRead(filePath))
        {
            byte[] hash = sha.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
