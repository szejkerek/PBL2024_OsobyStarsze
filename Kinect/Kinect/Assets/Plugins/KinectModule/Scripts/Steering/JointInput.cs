using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Plugins.KinectModule.Data;
using Plugins.KinectModule.SaveData;


namespace Plugins.KinectModule.Steering {

    /// <summary>
    /// Klasa służąca do sterowania obiektem za pomocą urządzenia Azure Kinect.
    /// </summary>
    public class JointInput : PositionUpdaterInput {

        /// <summary>
        /// Referencja do BodyTracker.
        /// </summary>
        [SerializeField]
        BodyTracker bodyTracker;

        [Space()]

        /// <summary>
        /// Staw sterujacy, który służy do sterowania obiektem.
        /// </summary>
        [SerializeField]
        JointId steeringJointId;

        /// <summary>
        /// Staw bazowy.
        /// </summary>
        [SerializeField]
        JointId baseJointId;

        /// <summary>
        /// Maksymalne odchylenie się stawu bazowego od stawu sterującego w metrach.
        /// </summary>
        [SerializeField]
        Vector3 maxDeviation;

        /// <summary>
        /// Pozycja stawu sterującego.
        /// </summary>
        Vector3 steeringJointPosition;
        /// <summary>
        /// Pozycja stawu bazowego.
        /// </summary>
        Vector3 baseJointPosition;
        /// <summary>
        /// Rotacja stawu sterującego.
        /// </summary>
        Quaternion steeringRotation;
        /// <summary>
        /// Rotacja stawu bazowego.
        /// </summary>
        Quaternion baseJointRotation;
        /// <summary>
        /// Czy postać ludzka została wykryta przez urządzenie Azure Kinect.
        /// </summary>
        bool isDetected;

        /// <summary>
        /// Co klatke aktualizuje pozycje obiektu. 
        /// </summary>
        void Update() {
            if (bodyTracker.CanGetUpdateData()) {
                (steeringJointPosition, steeringRotation) = bodyTracker.GetDataByJoint(steeringJointId, ref isDetected);
                (baseJointPosition, baseJointRotation) = bodyTracker.GetDataByJoint(baseJointId, ref isDetected);
            }

            Vector3 tValues = new Vector3 {
                x = Clamp(steeringJointPosition.x, baseJointPosition.x, maxDeviation.x),
                y = Clamp(steeringJointPosition.y, baseJointPosition.y, maxDeviation.y),
                z = Clamp(steeringJointPosition.z, baseJointPosition.z, maxDeviation.z)
            };

            tValues = CastPositionFromSkeletonOnPanel(tValues);

            positionUpdater.Update(tValues);

        }

        /// <summary>
        /// Zapisuje informacje o położeniu i rotacji stawów do pliku.
        /// </summary>
        public void SaveInformationAboutPosition() {
            if (!enabled) {
                return;
            }
            CollectData.Instance.CollectRow(new string[] { "Base Position", baseJointId.ToString() });
            CollectData.Instance.AddToCurrentRow(new Vector3[] { baseJointPosition, baseJointRotation.eulerAngles });

            CollectData.Instance.CollectRow(new string[] { "Steering Position", steeringJointId.ToString() });
            CollectData.Instance.AddToCurrentRow(new Vector3[] { steeringJointPosition, steeringRotation.eulerAngles });
        }

        /// <summary>
        /// Wylicza wartość procentową pomiędzy wartościami steeringValue i baseValue. Maksymalna wartość jest określana przez maxDeviation.
        /// </summary>
        /// <param name="steeringValue">Pierwsza wartość.</param>
        /// <param name="baseValue">Druga wartość.</param>
        /// <param name="maxDeviation">Maksymalne odchylenie.</param>
        /// <returns></returns>
        float Clamp(float steeringValue, float baseValue, float maxDeviation) {
            if (maxDeviation != 0) {
                return (Mathf.Clamp((steeringValue - baseValue) / maxDeviation, -1f, 1f) + 1) / 2;
            } else {
                return 0f;
            }
        }
    }

}
