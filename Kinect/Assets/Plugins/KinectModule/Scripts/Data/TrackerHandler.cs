using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

namespace Plugins.KinectModule.Data {

    /// <summary>
    /// Klasa służąca do zmiany pozycji szkieletu 3D.
    /// </summary>
    public class TrackerHandler : MonoBehaviour {

        /// <summary>
        /// Referencja do BodyTracker.
        /// </summary>
        [SerializeField]
        BodyTracker bodyTracker;

        /// <summary>
        /// Czy generować i aktualizować pozycję szkieletu.
        /// </summary>
        [SerializeField]
        bool drawSkeletons = true;

        /// <summary>
        /// Zależności pomiędzy stawami.
        /// </summary>
        Dictionary<JointId, JointId> parentJointMap;

        /// <summary>
        /// Indeks postaci.
        /// </summary>
        const int bodyIndex = 0;
        /// <summary>
        /// Indeks kości.
        /// </summary>
        const int boneChildNum = 0;

        /// <summary>
        /// Na starcie działania obiektu definiuje domyślne wartości.
        /// </summary>
        void Awake() {
            parentJointMap = new Dictionary<JointId, JointId>();

            parentJointMap[JointId.Pelvis] = JointId.Count;
            parentJointMap[JointId.SpineNavel] = JointId.Pelvis;
            parentJointMap[JointId.SpineChest] = JointId.SpineNavel;
            parentJointMap[JointId.Neck] = JointId.SpineChest;
            parentJointMap[JointId.ClavicleLeft] = JointId.SpineChest;
            parentJointMap[JointId.ShoulderLeft] = JointId.ClavicleLeft;
            parentJointMap[JointId.ElbowLeft] = JointId.ShoulderLeft;
            parentJointMap[JointId.WristLeft] = JointId.ElbowLeft;
            parentJointMap[JointId.HandLeft] = JointId.WristLeft;
            parentJointMap[JointId.HandTipLeft] = JointId.HandLeft;
            parentJointMap[JointId.ThumbLeft] = JointId.HandLeft;
            parentJointMap[JointId.ClavicleRight] = JointId.SpineChest;
            parentJointMap[JointId.ShoulderRight] = JointId.ClavicleRight;
            parentJointMap[JointId.ElbowRight] = JointId.ShoulderRight;
            parentJointMap[JointId.WristRight] = JointId.ElbowRight;
            parentJointMap[JointId.HandRight] = JointId.WristRight;
            parentJointMap[JointId.HandTipRight] = JointId.HandRight;
            parentJointMap[JointId.ThumbRight] = JointId.HandRight;
            parentJointMap[JointId.HipLeft] = JointId.SpineNavel;
            parentJointMap[JointId.KneeLeft] = JointId.HipLeft;
            parentJointMap[JointId.AnkleLeft] = JointId.KneeLeft;
            parentJointMap[JointId.FootLeft] = JointId.AnkleLeft;
            parentJointMap[JointId.HipRight] = JointId.SpineNavel;
            parentJointMap[JointId.KneeRight] = JointId.HipRight;
            parentJointMap[JointId.AnkleRight] = JointId.KneeRight;
            parentJointMap[JointId.FootRight] = JointId.AnkleRight;
            parentJointMap[JointId.Head] = JointId.Pelvis;
            parentJointMap[JointId.Nose] = JointId.Head;
            parentJointMap[JointId.EyeLeft] = JointId.Head;
            parentJointMap[JointId.EarLeft] = JointId.Head;
            parentJointMap[JointId.EyeRight] = JointId.Head;
            parentJointMap[JointId.EarRight] = JointId.Head;
        }

        /// <summary>
        /// Co klatkę sprawdza czy może zaktualizować dane, jeśli tak to aktualizuje pozycje stawów w szkielecie.
        /// </summary>
        void Update() {
            if (drawSkeletons && bodyTracker != null && bodyTracker.CanGetUpdateData()) {
                RenderSkeleton();
            }
        }

        /// <summary>
        /// Wyłącza szkielet 3D.
        /// </summary>
        public void TurnOnOffSkeletons() {
            drawSkeletons = !drawSkeletons;
            const int bodyRenderedNum = 0;
            for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++) {
                transform.GetChild(bodyRenderedNum).GetChild(jointNum).gameObject.GetComponent<MeshRenderer>().enabled = drawSkeletons;
                transform.GetChild(bodyRenderedNum).GetChild(jointNum).GetChild(0).GetComponent<MeshRenderer>().enabled = drawSkeletons;
            }
        }

        /// <summary>
        /// Generuje i aktualizuje szkielet 3D.
        /// </summary>
        void RenderSkeleton() {

            Vector3 jointPosition;
            Quaternion jointRotation;
            Vector3 parentTrackerSpacePosition;
            bool isDetected = true;

            for (int jointNum = 0; jointNum < (int)JointId.Count; jointNum++) {
                (jointPosition, jointRotation) = bodyTracker.GetDataByJoint((JointId)jointNum, ref isDetected);

                if (!isDetected) {
                    return;
                }

                transform.GetChild(bodyIndex).GetChild(jointNum).localPosition = jointPosition;
                transform.GetChild(bodyIndex).GetChild(jointNum).localRotation = jointRotation;

                if (parentJointMap[(JointId)jointNum] != JointId.Head && parentJointMap[(JointId)jointNum] != JointId.Count) {
                    (parentTrackerSpacePosition, jointRotation) = bodyTracker.GetDataByJoint(parentJointMap[(JointId)jointNum], ref isDetected);

                    Vector3 boneDirectionTrackerSpace = jointPosition - parentTrackerSpacePosition;
                    Vector3 boneDirectionWorldSpace = transform.rotation * boneDirectionTrackerSpace;
                    Vector3 boneDirectionLocalSpace = Quaternion.Inverse(transform.GetChild(bodyIndex).GetChild(jointNum).rotation) * Vector3.Normalize(boneDirectionWorldSpace);
                    transform.GetChild(bodyIndex).GetChild(jointNum).GetChild(boneChildNum).localScale = new Vector3(1, 20.0f * 0.5f * boneDirectionWorldSpace.magnitude, 1);
                    transform.GetChild(bodyIndex).GetChild(jointNum).GetChild(boneChildNum).localRotation = Quaternion.FromToRotation(Vector3.up, boneDirectionLocalSpace);
                    transform.GetChild(bodyIndex).GetChild(jointNum).GetChild(boneChildNum).position = transform.GetChild(bodyIndex).GetChild(jointNum).position - 0.5f * boneDirectionWorldSpace;
                } else {
                    transform.GetChild(bodyIndex).GetChild(jointNum).GetChild(boneChildNum).gameObject.SetActive(false);
                }
            }
        }
    }

}
