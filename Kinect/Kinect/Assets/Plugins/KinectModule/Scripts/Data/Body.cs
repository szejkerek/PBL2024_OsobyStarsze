using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System;
using UnityEngine;
using System.Runtime.Serialization;

namespace Plugins.KinectModule.Data {

    /// <summary>
    /// Klasa przechowująca dane na temat podjedynczego wykrytego ciała.
    /// </summary>
    [Serializable]
    public struct Body {
        /// <summary>
        /// Tablica pozycji stawów.
        /// </summary>
        public Vector3[] JointPositions3D;

        public Vector3[] JointPositions2D;

        /// <summary>
        /// Tablica rotacji stawów .
        /// </summary>
        public Quaternion[] JointRotations;

        /// <summary>
        /// Tablica poziomów pewności wykrycia stawów.
        /// </summary>
        public JointConfidenceLevel[] JointPrecisions;

        /// <summary>
        /// Ilość dostępnych stawów.
        /// </summary>
        public int Length;

        /// <summary>
        /// Id wykrytego ciała.
        /// </summary>
        public uint Id;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="maxJointsLength">Maksymalna ilość stawów.</param>
        public Body(int maxJointsLength) {
            JointPositions3D = new Vector3[maxJointsLength];
            JointPositions2D = new Vector3[maxJointsLength];
            JointRotations = new Quaternion[maxJointsLength];
            JointPrecisions = new JointConfidenceLevel[maxJointsLength];

            Length = 0;
            Id = 0;
        }

        /// <summary>
        /// Metoda zajmująca się kopiowaniem danych z Microsoft.Azure.Kinect.BodyTracking.Body do tej klasy.
        /// </summary>
        /// <param name="body"></param>
        public void CopyFromBodyTrackingSdk(Microsoft.Azure.Kinect.BodyTracking.Body body, Calibration sensorCalibration) {
            Id = body.Id;
            Length = Skeleton.JointCount;

            for (int bodyPoint = 0; bodyPoint < Length; bodyPoint++) {
                JointPositions3D[bodyPoint] = (body.Skeleton.GetJoint(bodyPoint).Position / 1000.0f).ConvertToUnityEngine();
                JointRotations[bodyPoint] = body.Skeleton.GetJoint(bodyPoint).Quaternion.ConvertToUnityEngine();
                JointPrecisions[bodyPoint] = body.Skeleton.GetJoint(bodyPoint).ConfidenceLevel;

                System.Numerics.Vector2? position2d = sensorCalibration.TransformTo2D(
                (body.Skeleton.GetJoint(bodyPoint).Position / 1000.0f),
                CalibrationDeviceType.Depth,
                CalibrationDeviceType.Depth);

                if (position2d != null) {
                    JointPositions2D[bodyPoint] = position2d.Value.ConvertToUnityEngine();
                } else {
                    JointPositions2D[bodyPoint].x = 0f;
                    JointPositions2D[bodyPoint].y = 0f;
                }

            }
        }
    }
}

