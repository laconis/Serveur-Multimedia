

class FileLaconis{

   /*
   *Retourne le nombre des dossiers d'un repertoire. 
   */
   public static function getCountFolder($dir){
      return count(glob($dir),GLOB_ONLYDIR);
   }


  
}
