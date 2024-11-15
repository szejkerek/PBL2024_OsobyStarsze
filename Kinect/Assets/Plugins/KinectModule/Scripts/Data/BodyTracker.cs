using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

namespace Plugins.KinectModule.Data{

    /// <summary>
    /// Klasa służąca do przechwytywania danych o wykrytej postaci ludzkiej.
    /// </summary>
    public class BodyTracker : MonoBehaviour{

        /// <summary>
        /// Referencja do KinectConnection.
        /// </summary>
        private KinectConnection connection;

        /// <summary>
        /// Maksymalna odległość z kamey do ciała wykrywanej postaci ludzkiej.
        /// </summary>
        [SerializeField]
        private float maxDistance = 5000.0f;

        /// <summary>
        /// Dane o aktualnej ramce danych.
        /// </summary>
        private BackgroundData backgroundData = new BackgroundData();
        /// <summary>
        /// 
        /// </summary>
        private Quaternion Y_180_FLIP = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

        /// <summary>
        /// Słownik przechowujący bazową rotacje danego stawu.
        /// </summary>
        private Dictionary<JointId, Quaternion> basisJointMap;

        /// <summary>
        /// Czy był błąd podczas pobierania danych.
        /// </summary>
        private bool wasErrorBackgroundData;
        /// <summary>
        /// Czy w aktualnej klatce zostały zaktualizowane dane z urządzenia.
        /// </summary>
        private bool wasUpdateFrame;
        /// <summary>
        /// Czy postać ludzka została wykryta przez urządzenie.
        /// </summary>
        private bool isDetected;
        
        /// <summary>
        /// Inicjuje, na starcie działania obiektu, domyśle wartości.
        /// </summary>
        private void Start(){

            connection = GetComponent<KinectConnection>();
            Vector3 zPositive = Vector3.forward;
            Vector3 xPositive = Vector3.right;
            Vector3 yPositive = Vector3.up;

            Quaternion leftHipBasis = Quaternion.LookRotation(xPositive, -zPositive);
            Quaternion spineHipBasis = Quaternion.LookRotation(xPositive, -zPositive);
            Quaternion rightHipBasis = Quaternion.LookRotation(xPositive, zPositive);

            Quaternion leftArmBasis = Quaternion.LookRotation(yPositive, -zPositive);
            Quaternion rightArmBasis = Quaternion.LookRotation(-yPositive, zPositive);
            Quaternion leftHandBasis = Quaternion.LookRotation(-zPositive, -yPositive);
            Quaternion rightHandBasis = Quaternion.identity;
            Quaternion leftFootBasis = Quaternion.LookRotation(xPositive, yPositive);
            Quaternion rightFootBasis = Quaternion.LookRotation(xPositive, -yPositive);

            basisJointMap = new Dictionary<JointId, Quaternion>();

            basisJointMap[JointId.Pelvis] = spineHipBasis;
            basisJointMap[JointId.SpineNavel] = spineHipBasis;
            basisJointMap[JointId.SpineChest] = spineHipBasis;
            basisJointMap[JointId.Neck] = spineHipBasis;
            basisJointMap[JointId.ClavicleLeft] = leftArmBasis;
            basisJointMap[JointId.ShoulderLeft] = leftArmBasis;
            basisJointMap[JointId.ElbowLeft] = leftArmBasis;
            basisJointMap[JointId.WristLeft] = leftHandBasis;
            basisJointMap[JointId.HandLeft] = leftHandBasis;
            basisJointMap[JointId.HandTipLeft] = leftHandBasis;
            basisJointMap[JointId.ThumbLeft] = leftArmBasis;
            basisJointMap[JointId.ClavicleRight] = rightArmBasis;
            basisJointMap[JointId.ShoulderRight] = rightArmBasis;
            basisJointMap[JointId.ElbowRight] = rightArmBasis;
            basisJointMap[JointId.WristRight] = rightHandBasis;
            basisJointMap[JointId.HandRight] = rightHandBasis;
            basisJointMap[JointId.HandTipRight] = rightHandBasis;
            basisJointMap[JointId.ThumbRight] = rightArmBasis;
            basisJointMap[JointId.HipLeft] = leftHipBasis;
            basisJointMap[JointId.KneeLeft] = leftHipBasis;
            basisJointMap[JointId.AnkleLeft] = leftHipBasis;
            basisJointMap[JointId.FootLeft] = leftFootBasis;
            basisJointMap[JointId.HipRight] = rightHipBasis;
            basisJointMap[JointId.KneeRight] = rightHipBasis;
            basisJointMap[JointId.AnkleRight] = rightHipBasis;
            basisJointMap[JointId.FootRight] = rightFootBasis;
            basisJointMap[JointId.Head] = spineHipBasis;
            basisJointMap[JointId.Nose] = spineHipBasis;
            basisJointMap[JointId.EyeLeft] = spineHipBasis;
            basisJointMap[JointId.EarLeft] = spineHipBasis;
            basisJointMap[JointId.EyeRight] = spineHipBasis;
            basisJointMap[JointId.EarRight] = spineHipBasis;
        }

        /// <summary>
        /// Na końcu każdej klatki ustawia wartość wasUpdateFrame na false.
        /// </summary>
        private void LateUpdate() {
            wasUpdateFrame = wasErrorBackgroundData = false;
        }

        /// <summary>
        /// Przy niszczeniu obiektu, rozłącza się z urzędzeniem Azure Kinect.
        /// </summary>
        private void OnDestroy() {
            if (connection != null)  {
                connection.Dispose();
            }
        }

        /// <summary>
        /// Metoda, która pobiera dane z KinectConnection.
        /// </summary>
        /// <returns>Zwraca czy udało się pobrać dane.</returns>
        public bool CanGetUpdateData() {
            if (connection == null || connection.status != KinectStatus.Running || (wasUpdateFrame && wasErrorBackgroundData)) {
                return false;
            }

            if (!wasUpdateFrame) {
                wasErrorBackgroundData = !connection.GetCurrentFrameData(ref backgroundData);
                isDetected = backgroundData != null && backgroundData.Bodies != null && backgroundData.Bodies.Length > 0;
                wasUpdateFrame = true;
            }

            return !wasErrorBackgroundData;
        }

        /// <summary>
        /// Metoda, która zwraca pozycje i rotacje danego stawu.
        /// </summary>
        /// <param name="jointId">Rodzaj stawu.</param>
        /// <param name="isDetected">Referncja do zmiennej logicznej, która zwraca czy dane ciało zosta�o wykryte.</param>
        /// <returns>Zwraca pozycje i rotacje.</returns>
        public (Vector3, Quaternion) GetDataByJoint(JointId jointId, ref bool isDetected){
            if (!CanGetBodyData()) {
                isDetected = false;
                return (new Vector3(), Quaternion.identity);
            }

            Body skeleton;
            {
                Body? nullableSkeleton = GetCurrentBody();
                if (!nullableSkeleton.HasValue) {
                    isDetected = false;
                    return (new Vector3(), Quaternion.identity);
                } else {
                    skeleton = nullableSkeleton.Value;
                }
            }
            int jointNum = (int)jointId;

            Vector3 jointPos = new Vector3(skeleton.JointPositions3D[jointNum].x, -skeleton.JointPositions3D[jointNum].y, skeleton.JointPositions3D[jointNum].z);
            Quaternion jointRot = Y_180_FLIP * new Quaternion(skeleton.JointRotations[jointNum].x, skeleton.JointRotations[jointNum].y,
                skeleton.JointRotations[jointNum].z, skeleton.JointRotations[jointNum].w) * Quaternion.Inverse(basisJointMap[(JointId)jointNum]);

            isDetected = true;

            return (jointPos, jointRot);
        }

        public Vector2 GetDataByJoint(JointId jointId) {
            if (!CanGetBodyData()) {
                isDetected = false;
                return Vector2.zero;
            }
            Body skeleton;
            {
                Body? nullableSkeleton = GetCurrentBody();
                if (!nullableSkeleton.HasValue) {
                    isDetected = false;
                    return new Vector2();
                } else {
                    skeleton = nullableSkeleton.Value;
                }
            }
            int jointNum = (int)jointId;

            return skeleton.JointPositions3D[jointNum];
        }



        /// <summary>
        /// Zwraca wygenerowany obraz z KinectConnection.
        /// </summary>
        /// <param name="texture">Referencja do zwracanej tekstury.</param>
        public void GetFrameTexture(ref Texture2D texture) {
            if (!CanGetBodyData()) {
                return;
            }

            if (texture == null || texture.width != backgroundData.ColorImageWidth || texture.height != backgroundData.ColorImageHeight) {
                if (texture != null) {
                    DestroyImmediate(texture);
                }
                texture = new Texture2D(backgroundData.ColorImageWidth, backgroundData.ColorImageHeight, TextureFormat.BGRA32, false) {
                    name = "Color image"
                };
            }

            texture.LoadRawTextureData(backgroundData.ColorImage);
            texture.Apply();
        }

        /// <summary>
        /// Zwraca aktualnie wykryte ciało.
        /// </summary>
        /// <returns>Obiekt klasy Body.</returns>
        private Body? GetCurrentBody() {
            if (!CanGetBodyData()) {
                return null;
            }

            int closestTrackedBody = FindClosestTrackedBody();
            if (closestTrackedBody < 0 || closestTrackedBody >= backgroundData.Bodies.Length) {
                return null;
            }

            return backgroundData.Bodies[closestTrackedBody];
        }

        /// <summary>
        /// Sprawdza czy w danej klatce zostało pobrane dane z urządzenia. Jeżli nie, to je pobiera.
        /// </summary>
        /// <returns>Czy udało się zaktualizował dane lub czy dane się już zaktualizowane.</returns>
        private bool CanGetBodyData() {
            if (!wasUpdateFrame) {
                if (!CanGetUpdateData()) {
                    return false;
                }
            }
            return isDetected;
        }

        /// <summary>
        /// Znajduje index najbliższego ciała w stosunku do kamery.
        /// </summary>
        /// <returns>Index najbliższego ciała.</returns>
        private int FindClosestTrackedBody() {
            int closestBody = -1;
            float minDistanceFromKinect = maxDistance;
            for (int i = 0; i < (int)backgroundData.NumOfBodies; i++) {
                var pelvisPosition = backgroundData.Bodies[i].JointPositions3D[(int)JointId.Pelvis];
                Vector3 pelvisPos = new Vector3((float)pelvisPosition.x, (float)pelvisPosition.y, (float)pelvisPosition.z);
                if (pelvisPos.magnitude < minDistanceFromKinect) {
                    closestBody = i;
                    minDistanceFromKinect = pelvisPos.magnitude;
                }
            }
            return closestBody;
        }
    }
}
