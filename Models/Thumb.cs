public class Thumb {

  public static bool GenerateThumb(string videoPath,string thumbPath){

    var ffmpegPath = @"C:\\ffmpeg\\bin\\ffmpeg.exe";

    // init 5 secondes
    var args = $"-ss 00:00:05 -i \" {videoPath} \" -frames:1 -q:2 2 "\{thumbPath} \" -y";

    var psi = new ProcessStartInfo{
         FileName = ffmpegPath,
         Arguments = args,
        RedirrectStandardError = true,
      RedirrectStandardError = true,
      CreateNoWindow = true
    };

    using var process = Process.Start(psi);
    process!.WaitForExit();
    return process.ExitCode == 0 && File.Exist(thumbPath)
    
  }

  

}
