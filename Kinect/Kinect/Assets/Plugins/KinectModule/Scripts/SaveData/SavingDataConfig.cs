using UnityEngine;
using System.IO;


namespace Plugins.KinectModule.SaveData {

    /// <summary>
    /// Klasa statyczna przechowująca informacje konfigurujące do zapisu pliku.
    /// </summary>
    public class SavingDataConfig {

        /// <summary>
        /// Domyślna nazwa pliku.
        /// </summary>
        public static string fileName = "data";
        /// <summary>
        /// Rozszerzenie pliku.
        /// </summary>
        public static string fileExt = "csv";
        /// <summary>
        /// Separator w formacie csv.
        /// </summary>
        public static string separator = ";";
        /// <summary>
        /// Ścieżka bezwzględna do pliku.
        /// </summary>
        public static string pathFile => Path.Combine(Application.persistentDataPath, fileName + (int)(Time.realtimeSinceStartup * 1000) + "." + fileExt);

    }

}
