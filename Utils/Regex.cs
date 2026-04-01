public Regex{
static string NormalizeTimestamp(string input)
    {
        var match = Regex.Match(input, @"^(\d{1,2}):(\d{1,2}):(\d{1,2})$");

        if (!match.Success)
            return null;

        int h = int.Parse(match.Groups[1].Value);
        int m = int.Parse(match.Groups[2].Value);
        int s = int.Parse(match.Groups[3].Value);

        if (h > 23 || m > 59 || s > 59)
            return null;

        return $"{h:D2}:{m:D2}:{s:D2}";
    }

    public static bool verifRegexVideo(string pattern){

      var pattern = new Regex(@"^(?:[01]\d|2[0-3]):[0-5]\d:[0-5]\d$");
        
        bool match = pattern.IsMatch(input);

        return match;
       
    }

    
/*
*   Fonction decoupant une vidéo en fonction d'une timelapse // 
*   Update => Modifier la taille du Timelapse + bitrate// 
*/
public static bool GetTimelapseVideo(string pattern)
{
    // Exemple : pattern = "01:23:45"
    //string pattern = "01:23:45";

    // Découpage HH:MM:SS
    string[] total = pattern.Split(':');
    int heures = int.Parse(total[0]);
    int minutes = int.Parse(total[1]);
    int secondes = int.Parse(total[2]);

    // Conversion en secondes
    int totalSeconds = (heures * 3600) + (minutes * 60) + secondes;

    // On retire 120 secondes (2 minutes)
    int secondsBeforeThreat = totalSeconds - 120;
    if (secondsBeforeThreat < 0) secondsBeforeThreat = 0;

    // Conversion inverse en HH:MM:SS
    TimeSpan ts = TimeSpan.FromSeconds(secondsBeforeThreat);
    string startTime = ts.ToString(@"hh\:mm\:ss");


    // Construction de la commande FFmpeg // 
    string arguments = $"-i input.mp4 -ss {startTime} -t 00:04:00 -c copy output.mp4";

    // Lancement du processus FFmpeg
    ProcessStartInfo psi = new ProcessStartInfo
    {
        FileName = "ffmpeg",
        Arguments = arguments,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    };

    Process p = Process.Start(psi);
    p.WaitForExit();

    return p.ExitCode == 0;
}


}

