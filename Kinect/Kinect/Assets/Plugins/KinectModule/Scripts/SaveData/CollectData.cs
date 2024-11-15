using System.Collections.Generic;
using System.Linq;
using System.IO;
//using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Plugins.KinectModule.SaveData{
    /// <summary>
    /// Klasa służąca do przechowywania danych, póżniej zapisywanych do pliku.
    /// </summary>
    public class CollectData : Singleton<CollectData> {

        /// <summary>
        /// Maksymalna ilość linijek w pojedynczym pliku.
        /// </summary>
        private const int MaxRowsInData = 400;
        /// <summary>
        /// Lista wierszy danych do zapisania.
        /// </summary>
        private List<string[]> data = new List<string[]>();
        /// <summary>
        /// Maksymalna ilość kolumn.
        /// </summary>
        private int maxCells;


        public Vector3 firstPositionsToSave;
        public Quaternion firstRotationToSave;
        public Vector3 secPositionsToSave;
        public Quaternion secRotationToSave;

        public List<string> newdataFirstJoint = new List<string>();
        public List<string> newdataSecondJoint = new List<string>();

        private void OnDestroy() {
            if (data.Count > 0) {
                SaveNewData();
                SaveNewData2();
                Save();
            }
        }

        public void processNewData()
        {
            var lineFJoint = string.Format("{0};{1};{2};{3};{4};{5}", firstPositionsToSave.x, firstPositionsToSave.y, firstPositionsToSave.z, firstRotationToSave.x, firstRotationToSave.y, firstRotationToSave.z);
            var lineSecJoint = string.Format("{0},{1},{2},{3},{4},{5}", secPositionsToSave.x, secPositionsToSave.y, secPositionsToSave.z, secRotationToSave.x, secRotationToSave.y, secRotationToSave.z);
            newdataFirstJoint.Add(lineFJoint);
            newdataSecondJoint.Add(lineSecJoint);

            //DODAC CZAS
            // HH:MM:SS
        }

        void SaveNewData()
        {
            string directory = Application.dataPath + "/UserData/";
            string filename = String.Format("hipright.csv");
            string path = Path.Combine(directory, filename);

            using (StreamWriter sw = File.CreateText(path))
            {
                for(int i =0; i < newdataFirstJoint.Count;i++)
                {
                    sw.WriteLine(newdataFirstJoint[i]);
                }
            }         
        }

        void SaveNewData2()
        {
            string directory = Application.dataPath + "/UserData/";
            string filename2 = String.Format("footright.csv");
            string path2 = Path.Combine(directory, filename2);

            using (StreamWriter sw = File.CreateText(path2))
            {
                for (int i = 0; i < newdataSecondJoint.Count; i++)
                {
                    sw.WriteLine(newdataSecondJoint[i]);
                }
            }
        }
        /// <summary>
        /// Dodaje nowy wiersz z danymi.
        /// </summary>
        /// <param name="cells">Pojedynczy element string tablicy określa pojdynczą komórke danych w pliku.</param>
        public void CollectRow(string [] cells) {
            CreateIfDontExist();

            if (maxCells < cells.Length) {
                maxCells = cells.Length;
            }

            data.Add(cells);

            if (data.Count > MaxRowsInData) {
                Save();
            }
        }

        /// <summary>
        /// Dodaje nowy wiersz z danymi.
        /// </summary>
        /// <param name="cells">Pojedynczy element string tablicy określa pojdynczą komórke danych w pliku.</param>
        public void CollectRow(Vector3[] cells) {
            CollectRow(ConvertVector3ToString(cells));
        }

        /// <summary>
        /// Dodaje komórki danych do ostatniego wiersza.
        /// </summary>
        /// <param name="cells">Pojedynczy element Vector3 tablicy określa trzy komórki danych w pliku.</param>
        public void AddToCurrentRow(string [] cells) {
            CreateIfDontExist();
            int lastIndex = data.Count - 1;
            data[lastIndex] = data[lastIndex].Concat(cells).ToArray();
            if (data[lastIndex].Length > maxCells) {
                maxCells = data[lastIndex].Length;
            }
        }

        /// <summary>
        /// Dodaje komórki danych do ostatniego wiersza.
        /// </summary>
        /// <param name="cells">Pojedynczy element Vector3 tablicy określa trzy komórki danych w pliku.</param>
        public void AddToCurrentRow(Vector3[] cells) {
            AddToCurrentRow(ConvertVector3ToString(cells));
        }

        /// <summary>
        /// Zapisuje przechywane dane do pliku.
        /// </summary>
        //[Button]
        private void Save() {
            if (maxCells == 0) {
                return;
            }

            string[,] dataArray = new string[data.Count, maxCells];
            for (int row = 0; row < data.Count; row++) {
                int cell = 0;
                for (; cell < data[row].Length; cell++) {
                    dataArray[row, cell] = data[row][cell];
                }
                for (; cell < maxCells; cell++) {
                    dataArray[row, cell] = "";
                }
            }

            SavingData.Instance.Save(dataArray);
            Clear();
        }

        /// <summary>
        /// Konwertuje Vector3 na tablice 3 elementową typu string.
        /// </summary>
        /// <param name="array">Dana Vector3.</param>
        /// <returns>Tablice 3 elementowa typu string.</returns>
        private string[] ConvertVector3ToString(Vector3 [] array) {
            string[] stringCells = new string[array.Length * 3];
            int x = 0;
            for (int i = 0; i < stringCells.Length; i += 3) {
                stringCells[i] = array[x].x.ToString("F5");
                stringCells[i + 1] = array[x].y.ToString("F5");
                stringCells[i + 2] = array[x].z.ToString("F5");
                x++;
            }
            return stringCells;
        }

        /// <summary>
        /// Inicjuje listę jeśli nie istnieje i resetuje wartość maxCell. 
        /// </summary>
        private void CreateIfDontExist(){
            if (data != null)
            {
                return;
            }
            data = new List<string[]>();
            maxCells = 0;
        }

        /// <summary>
        /// Resetuje wartości.
        /// </summary>
        private void Clear() {
            data.Clear();
            maxCells = 0;
        }

    }
}
