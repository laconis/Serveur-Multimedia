

class FileLaconis{

   /*
   *Retourne le nombre des dossiers d'un repertoire. 
   */
   public static function getCountFolder($dir){
      return count(glob($dir),GLOB_ONLYDIR);
   }

   public static void getFileRecurive(){

   }

   /*
   * Retourne une liste de fichiers selon le pattern des extensions
   */
   public static void getFileRecurive(List<string> patternExtension){
      
   }

   /*
   * Retourne les 16 premiers bytes du fichier
   */
   public static byte[] GetFileHeader(string filePath, int headerSize = 16)
   {
       using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
       {
           byte[] header = new byte[headerSize];
           fs.Read(header, 0, headerSize);
           return header;
       }
   }

   /*
   * Retourne les 5 (default) premieres lignes du fichier
   */
      public static string GetTextHeader(string filePath, int lines = 5)
   {
       return string.Join("\n", File.ReadLines(filePath).Take(lines));
   }
      

   
   

  
}
