using System.Collections.Generic;
using Plugins.KinectModule.Data;
using Plugins.KinectModule.Gesture;
using Plugins.KinectModule.SaveData;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.KinectModule.Scripts.Gesture {

    /// <summary>
    /// Klasa odpowiedzialna za rozpoznawanie gestów.
    /// </summary>
    public class GestureRecognizer : MonoBehaviour {

        /// <summary>
        /// Zdarzenie wywoływane podczas ywkrycia gestu.
        /// </summary>
        public UnityEvent OnGestureRecognize;
        /// <summary>
        /// Czy zapisać dane do pliku podczas wykrycia gestu.
        /// </summary>
        public bool saveData;

        /// <summary>
        /// Referencja do wykrywanej gestu.
        /// </summary>
        [SerializeField]
        private KinectModule.Gesture.Gesture gesture;

        /// <summary>
        /// Referencja do BodyTracker.
        /// </summary>
        [SerializeField]
        private BodyTracker bodyTracker;

        [Space()]

        /// <summary>
        /// Referencja do BodyTracker.
        /// </summary>
        [SerializeField]
        private KeyCode keyToSimulateOnGestureRecognize;

        /// <summary>
        /// Aktualny czas warunku przeznaczony do sprawdzania wygaśnięcia.
        /// </summary>
        private float currentTime;
        /// <summary>
        /// Index sprawdzanego warunku.
        /// </summary>
        private int indexCondition = 0;
        /// <summary>
        /// Lista warunków do spełnienia, aby gest został wykryty.
        /// </summary>
        private List<GestureCondition> conditions = new List<GestureCondition>();
        /// <summary>
        /// Pozycja i skala pierwszego stawu w warunku.
        /// </summary>
        private Transform firstJointIdTransform;
        /// <summary>
        /// Pozycja i skala drugiego stawu w warunku.
        /// </summary>
        private Transform secondJointIdTransform;

        //List<Vector3> firstPositionsToSave;
        //List<Quaternion> firstRotationToSave;
        //List<Vector3> secPositionsToSave;
        //List<Quaternion> secRotationToSave;

        bool repeatCollectingData;

        private void Start() {
            if (gesture != null) {
                conditions = gesture.gestureConditions;
                firstJointIdTransform = (new GameObject("FirstJointIdTransform")).transform;
                secondJointIdTransform = (new GameObject("SecondJointIdTransform")).transform;

                firstJointIdTransform.SetParent(transform);
                secondJointIdTransform.SetParent(transform);
            }

            repeatCollectingData = false;
            InvokeRepeating("switchRepeatCollectingData", 10.0f, 0.3f);
        }

        void switchRepeatCollectingData()
        {
            repeatCollectingData = !repeatCollectingData;
        }
        /// <summary>
        /// Sprawdza czy dany gest nastąpił za pomocą listy conditions.
        /// </summary>
        private void Update() {
            if (Input.GetKeyDown(keyToSimulateOnGestureRecognize)) {
                OnGestureRecognize.Invoke();
                ResetValues();
                return;
            }


            if (conditions.Count == 0) {
                return;
            }

            if (conditions.Count <= indexCondition) {
                ResetValues();
            }

            currentTime += Time.deltaTime;
            int previousIndexCondition = indexCondition - 1;

            //Sprawdz czy minal czas waznosci
            if (previousIndexCondition >= 0) {
                if (conditions[previousIndexCondition].expireTime <= currentTime) {
                    ResetValues();
                }
            }

            if (bodyTracker.CanGetUpdateData()) {
                bool isDetected = true;
                (firstJointIdTransform.position, firstJointIdTransform.rotation) = bodyTracker.GetDataByJoint(conditions[indexCondition].firstJointId, ref isDetected);
                (secondJointIdTransform.position, secondJointIdTransform.rotation) = bodyTracker.GetDataByJoint(conditions[indexCondition].secondJointId, ref isDetected);

                if(repeatCollectingData == true)
                {
                    CollectData.Instance.firstPositionsToSave = firstJointIdTransform.position;
                    CollectData.Instance.firstRotationToSave=firstJointIdTransform.rotation;
                    CollectData.Instance.secPositionsToSave=secondJointIdTransform.position;
                    CollectData.Instance.secRotationToSave=secondJointIdTransform.rotation;

                    CollectData.Instance.processNewData();
                }
               // Debug.Log("ID" + conditions[indexCondition].firstJointId.ToString());
              //  Debug.Log("Position:" + firstJointIdTransform.position);//
               // Debug.Log("Rotation" + firstJointIdTransform.rotation);
            }

            //Sprawdz czy zostal sprawdzony warunek
            if (conditions[indexCondition].IsFullfiled(firstJointIdTransform, secondJointIdTransform)) {
                if (saveData && indexCondition > 0) {
                    CollectData.Instance.CollectRow(new string[] { "Gesture First Joint", conditions[indexCondition].firstJointId.ToString() });
                    //Debug.Log(conditions[indexCondition].firstJointId.ToString());
                    CollectData.Instance.AddToCurrentRow(new Vector3[] { firstJointIdTransform.position, firstJointIdTransform.rotation.eulerAngles });

                    CollectData.Instance.AddToCurrentRow(new string[] { "Gesture  Second Joint", conditions[indexCondition].secondJointId.ToString() });
                    CollectData.Instance.AddToCurrentRow(new Vector3[] { secondJointIdTransform.position, secondJointIdTransform.rotation.eulerAngles });
                }

                currentTime = 0f;
                indexCondition++;
            }

            //Check if gesture is recognized
            if (indexCondition == conditions.Count) {
                OnGestureRecognize.Invoke();
                ResetValues();
            }

        }

        /// <summary>
        /// Resetuje wartości do domyślnych.
        /// </summary>
        private void ResetValues() {
            indexCondition = 0;
            currentTime = 0f;
        }
    }

}
