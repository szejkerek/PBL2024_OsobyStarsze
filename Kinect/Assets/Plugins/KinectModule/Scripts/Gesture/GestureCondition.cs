using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;


namespace Plugins.KinectModule.Gesture {
    
    /// <summary>
    /// Klasa reprezentująca pojedynczy warunek do spełnienia, aby wykryć gest.
    /// </summary>
    [System.Serializable]
    public class GestureCondition {

        /// <summary>
        /// Pierwszy staw w warunku.
        /// </summary>
        [SerializeField]
        protected JointId _firstJointId;
        public JointId firstJointId => _firstJointId;

        /// <summary>
        /// Rodzaj warunku.
        /// </summary>
        [SerializeField]
        protected JointCondition _condition;
        public JointCondition condition => _condition;

        /// <summary>
        /// Drugi staw w warunku.
        /// </summary>
        [SerializeField]
        protected JointId _secondJointId;
        public JointId secondJointId => _secondJointId;

        /// <summary>
        /// Czas jaki ma użytkownik do spełnienia warunku.
        /// </summary>
        [SerializeField]
        protected float _expireTime;
        public float expireTime => _expireTime;

        /// <summary>
        /// Wartość kątowa w warunku, wykorzystywana w niektórych rodzajach warunku.
        /// </summary>
        [SerializeField]
        protected float _angleValue;
        public float angleValue => _angleValue;

        /// <summary>
        /// Wartość pomocnicza w warunku, wykorzystywana w niektórych rodzajach warunku.
        /// </summary>
        [SerializeField]
        protected float _distanceValue;
        public float distanceValue => _distanceValue;

        /// <summary>
        /// Sprawdza czy dany warunek został spełniony.
        /// </summary>
        /// <param name="firstTransform">Pozycja i roatacja pierwszego stawu</param>
        /// <param name="secondTransform">Pozycja i roatacja drugiego stawu.</param>
        /// <returns>Zwraca TRUE jeśli warunek został spełniony, w przeciwnym razie FALSE.</returns>
        public bool IsFullfiled(Transform firstTransform, Transform secondTransform) {
            switch (_condition) {
                case JointCondition.GreaterPositionX:
                    return firstTransform.position.x > secondTransform.position.x;
                case JointCondition.GreaterPositionY:
                    return firstTransform.position.y > secondTransform.position.y;
                case JointCondition.GreaterPositionZ:
                    return firstTransform.position.z > secondTransform.position.z;

                case JointCondition.LesserPositionX:
                    return firstTransform.position.x < secondTransform.position.x;
                case JointCondition.LesserPositionY:
                    return firstTransform.position.y < secondTransform.position.y;
                case JointCondition.LesserPositionZ:
                    return firstTransform.position.z < secondTransform.position.z;

                case JointCondition.GreaterAngle:
                    return Quaternion.Angle(firstTransform.rotation, secondTransform.rotation) > _angleValue;
                case JointCondition.LeserAngle:
                    return Quaternion.Angle(firstTransform.rotation, secondTransform.rotation) < _angleValue;

                case JointCondition.GreaterDistance:
                    return Vector3.Distance(firstTransform.position, secondTransform.position) > _angleValue;
                case JointCondition.LeserDistance:
                    return Vector3.Distance(firstTransform.position, secondTransform.position) < _angleValue;

                case JointCondition.GreaterDistanceX:
                    return Mathf.Abs(firstTransform.position.x - secondTransform.position.x) > distanceValue;
                case JointCondition.GreaterDistanceY:
                    return Mathf.Abs(firstTransform.position.y - secondTransform.position.y) > distanceValue;
                case JointCondition.GreaterDistanceZ:
                    return Mathf.Abs(firstTransform.position.z - secondTransform.position.z) > distanceValue;

                case JointCondition.LesserDistanceX:
                    return Mathf.Abs(firstTransform.position.x - secondTransform.position.x) < distanceValue;
                case JointCondition.LesserDistanceY:
                    return Mathf.Abs(firstTransform.position.y - secondTransform.position.y) < distanceValue;
                case JointCondition.LesserDistanceZ:
                    return Mathf.Abs(firstTransform.position.z - secondTransform.position.z) < distanceValue;
            }
            return false;
        }

        /// <summary>
        /// Zmień stronę stawów.
        /// </summary>
        public void TurnSide() {
            _firstJointId = TurnSide(_firstJointId);
            _secondJointId = TurnSide(_secondJointId);
        }

        /// <summary>
        /// Zmienia stronę stawów.
        /// </summary>
        /// <param name="jointId">Stary staw.</param>
        /// <returns>Obrócony staw.</returns>
        JointId TurnSide(JointId jointId) {
            switch (jointId) {
                case JointId.AnkleLeft:
                    return JointId.AnkleRight;
                case JointId.AnkleRight:
                    return JointId.AnkleLeft;

                case JointId.ClavicleLeft:
                    return JointId.ClavicleRight;
                case JointId.ClavicleRight:
                    return JointId.ClavicleLeft;

                case JointId.EarLeft:
                    return JointId.EarRight;
                case JointId.EarRight:
                    return JointId.EarLeft;

                case JointId.ElbowLeft:
                    return JointId.ElbowRight;
                case JointId.ElbowRight:
                    return JointId.ElbowLeft;
                case JointId.FootLeft:
                    return JointId.FootRight;
                case JointId.FootRight:
                    return JointId.FootLeft;

                case JointId.HandLeft:
                    return JointId.HandRight;
                case JointId.HandRight:
                    return JointId.HandLeft;

                case JointId.HandTipLeft:
                    return JointId.HandTipRight;
                case JointId.HandTipRight:
                    return JointId.HandTipLeft;

                case JointId.HipLeft:
                    return JointId.HipRight;
                case JointId.HipRight:
                    return JointId.HipLeft;

                case JointId.KneeLeft:
                    return JointId.KneeRight;
                case JointId.KneeRight:
                    return JointId.KneeLeft;

                case JointId.ShoulderLeft:
                    return JointId.ShoulderRight;
                case JointId.ShoulderRight:
                    return JointId.ShoulderLeft;

                case JointId.ThumbLeft:
                    return JointId.ThumbRight;
                case JointId.ThumbRight:
                    return JointId.ThumbLeft;

                case JointId.WristLeft:
                    return JointId.WristRight;
                case JointId.WristRight:
                    return JointId.WristLeft;
            }
            return jointId;
        }

    }

    /// <summary>
    /// Typ wyliczeniowy określający rodzaj warunku.
    /// </summary>
    public enum JointCondition {
        GreaterPositionX = 0,
        GreaterPositionY = 1,
        GreaterPositionZ,
        LesserPositionX,
        LesserPositionY,
        LesserPositionZ,
        GreaterAngle,
        LeserAngle,
        GreaterDistance,
        LeserDistance,
        GreaterDistanceX,
        GreaterDistanceY,
        GreaterDistanceZ,
        LesserDistanceX,
        LesserDistanceY,
        LesserDistanceZ,
    }

}