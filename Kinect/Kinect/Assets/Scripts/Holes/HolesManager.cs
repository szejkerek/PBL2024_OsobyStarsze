using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Holes
{
    public class DifficultyLevel
    {
        public int Id;
        public int SpawningTime;
        public int GoodCount;
        public int BadCount;
        public float BadElementChance;
        public float TimeInStablePosition;
    }

    [System.Serializable]
    public class ListofObjects
    {
        public string name;
        public List<GameObject> objects;
    }
    public class HolesManager : MonoBehaviour
    {

        /*Objects to catch*/
        [SerializeField]
        private List<ListofObjects> Objects;
        
        [Space]
        public List<GameObject> currentBadElements;
        public List<GameObject> currentGoodElements;

        /*Current difficulty level*/
        public int levelSelected;
        /*The type of objects to catch for a given gameplay*/
        public string selectedTag = "";
       
        public int result;  /*The type of objects to catch for a given gameplay - the index in Objects list*/

        [Space]
        [Range(0, 100)]
        public float badElementChance;// = 20f;

        public float timeInStablePosition;

        private Hole[] holes;

        /*Difficulty levels*/
        public DifficultyLevel[] Difflevels = new DifficultyLevel[]
        {
            new DifficultyLevel { Id = 0, SpawningTime = 8, GoodCount = 1, BadCount = 1, BadElementChance = 18f, TimeInStablePosition = 8.5f },
            new DifficultyLevel { Id = 1, SpawningTime = 6, GoodCount = 3, BadCount = 1, BadElementChance = 20f, TimeInStablePosition = 7.5f },
            new DifficultyLevel { Id = 2, SpawningTime = 5, GoodCount = 4, BadCount = 3, BadElementChance = 33f, TimeInStablePosition = 6.2f },
            new DifficultyLevel { Id = 3, SpawningTime = 4, GoodCount = 5, BadCount = 5, BadElementChance = 45f, TimeInStablePosition = 5.1f },
            new DifficultyLevel { Id = 4, SpawningTime = 3, GoodCount = 5, BadCount = 8, BadElementChance = 55f, TimeInStablePosition = 4.3f }
        };

        /*The number of good objects spawned*/
        private int goodObjectsSpawned;
        public int GoodObjectsSpawned
        {
            get { return goodObjectsSpawned; }
        }

        //private List<GameObject> newListToTest;


        private void Start()
        {
           holes = GetComponentsInChildren<Hole>();
           goodObjectsSpawned = 0;
        }

        public void SetList(int difficulty)
        {
            levelSelected = difficulty; 
           
            //Wybierz losowy Dataset
            System.Random rand = new System.Random();
            result = rand.Next(0, Objects.Count);
            Debug.Log("Wybrane-" + result);
        
            selectedTag = Objects[result].name;

            if (difficulty == 0 && Difflevels[0].GoodCount == 1)
                currentGoodElements.Add(Objects[result].objects[0]);
            else
            {
                int[] numbers = new int[Objects[result].objects.Count];
                for (int i = 0; i < numbers.Length; i++)
                    numbers[i] = i;

                Randomize(numbers);

                for (int i = 0; i < Difflevels[difficulty].GoodCount; i++)
                    currentGoodElements.Add(Objects[result].objects[numbers[i]]);
            }
            while (currentBadElements.Count < Difflevels[difficulty].BadCount)
            {
                int resultBad = result;
                while (resultBad == result)
                    resultBad = rand.Next(0, Objects.Count);
                GameObject badObj = Objects[resultBad].objects[rand.Next(0, Objects[resultBad].objects.Count)];
                if (currentBadElements.Contains(badObj))
                    Debug.Log(badObj.name + "juz jest na liscie BadObjects");
                else
                    currentBadElements.Add(badObj);
            }

            badElementChance = Difflevels[difficulty].BadElementChance;

        }
        public void SpawnEnemy()
        {
            try
            {
                holes[RandomIndex()].SpawnEnemy(RandomPrefab());
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private int RandomIndex()
        {
            List<int> availableHolesIDs = new List<int>();

            for (int i = 0; i < holes.Length; i++)
            {
                if(!holes[i].IsOccupied)
                {
                    availableHolesIDs.Add(i);
                }
            }

            if(availableHolesIDs.Count > 0)
            {
                return availableHolesIDs[UnityEngine.Random.Range(0,availableHolesIDs.Count)];
            }
            throw new NullReferenceException("All holes are occupied");
        }

        private GameObject RandomPrefab()
        {
            float myChance = UnityEngine.Random.Range(0f, 100f);
            GameObject obj = PickRandomFromList(myChance < badElementChance ? currentBadElements : currentGoodElements);
            if(obj.tag == selectedTag)
            {
                goodObjectsSpawned++;
            }
            return obj;
        }
        private GameObject PickRandomFromList(List<GameObject> objectsList)
        {
            return objectsList[UnityEngine.Random.Range(0, objectsList.Count)];
        }

        public void AdjustAllParameters()
        {
            badElementChance = Difflevels[levelSelected].BadElementChance;
            timeInStablePosition = Difflevels[levelSelected].TimeInStablePosition;
        }

        /*
         * Zmienia liczby obiektw w listach : good i bad
         * toselect - do jakiego poziomu trudnosci przechodzi
         */
        public void SetNewElementList(int toselect)
        {
            if (toselect == levelSelected)
                return;
            //zwiekszamy listy
            if (toselect > levelSelected)
            {
                int[] numbers = new int[Objects[result].objects.Count];
                for (int i = 0; i < numbers.Length; i++)
                    numbers[i] = i;

                Randomize(numbers);

                for (int i = 0; currentGoodElements.Count < Difflevels[toselect].GoodCount; i++)
                {
                    if (numbers.Length < i + 1)
                        break;
                    if (!currentGoodElements.Contains(Objects[result].objects[numbers[i]]))
                        currentGoodElements.Add(Objects[result].objects[numbers[i]]);
                }

                System.Random rand = new System.Random();

                while (currentBadElements.Count < Difflevels[toselect].BadCount)
                {

                    int resultBad = result;
                    while (resultBad == result)
                        resultBad = rand.Next(0, Objects.Count);
                    GameObject badObj = Objects[resultBad].objects[rand.Next(0, Objects[resultBad].objects.Count)];
                    if (currentBadElements.Contains(badObj))
                        Debug.Log(badObj.name + "juz jest na liscie BadObjects");
                    else
                        currentBadElements.Add(badObj);
                }

                return;
            }

            if (toselect < levelSelected)
            {
                System.Random rand = new System.Random();

                int indexToRemove;

                // Usuń element o losowym indeksie
              
                while ( currentGoodElements.Count > Difflevels[toselect].GoodCount)
                {
                    indexToRemove = rand.Next(0, currentGoodElements.Count);
                    currentGoodElements.RemoveAt(indexToRemove);
                }

        

                while (currentBadElements.Count > Difflevels[toselect].BadCount)
                {

                    indexToRemove = rand.Next(0, currentBadElements.Count);
                    currentBadElements.RemoveAt(indexToRemove);
                }

                return;
            }

        }

        /*Miesza elementy w tablicy*/
        private void Randomize(int[] arr)
        {
            System.Random rand = new System.Random();
            int n = arr.Length;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                int temp = arr[k];
                arr[k] = arr[n];
                arr[n] = temp;
            }
        }
    }

    


}
