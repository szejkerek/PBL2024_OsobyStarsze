using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.IO;
using System.Text;
//using NaughtyAttributes;
using Plugins.KinectModule;


namespace Plugins.KinectModule.SaveData {

    /// <summary>
    /// Klasa służąca do zapisu danych do pliku.
    /// </summary>
    public class SavingData : Singleton<SavingData> {

        /// <summary>
        /// Zdarzenie wywoływane po zapisie pliku.
        /// </summary>
        public System.Action onSaveFile;

        /// <summary>
        /// Rozpoczyna zapisywanie pliku.
        /// </summary>
        /// <param name="data">Dane do zapisania.</param>
        public void Save(string[,] data) {
            SaveFile(SavingDataConfig.pathFile, data).Forget();
        }

        /// <summary>
        /// Asynchronicznie tworzy plik i zapisuje w nim dane.
        /// </summary>
        /// <param name="path">Ścieżka do pliku.</param>
        /// <param name="data">Dane do zapisania.</param>
        /// <returns>UniTaskVoid</returns>
        async UniTaskVoid SaveFile(string path, string[,] data) {
            int nRows = data.GetLength(0);
            int nCells = data.GetLength(1);

            if (nRows == 0 || nCells == 0) {
                return;
            }

            StringBuilder output = new StringBuilder();

            for (int row = 0; row < nRows; row++) {
                for (int cell = 0; cell < nCells; cell++) {
                    output.Append("\"");
                    output.Append(data[row, cell]);
                    output.Append("\"");
                    output.Append(SavingDataConfig.separator);
                }
                output.Length -= SavingDataConfig.separator.Length;
                output.Append("\n");
            }

            FileStream file = File.Create(path);

            byte[] bytes = Encoding.ASCII.GetBytes(output.ToString());
            await file.WriteAsync(Encoding.ASCII.GetBytes(output.ToString()), 0, bytes.Length);

            onSaveFile?.Invoke();
        }
    }
}
