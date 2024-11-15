using System.Collections;
using System.Collections.Generic;
using Plugins.KinectModule.Data;
using UnityEngine;

namespace Plugins.KinectModule.UI {

    /// <summary>
    /// Klasa służąca do pokazania statusu wykrycia postaci ludzkiej przez Azure Kinect.
    /// </summary>
    public class ShowBodyStatus : TextSetter {

        /// <summary>
        /// Referencja do SettingKinect.
        /// </summary>
        [SerializeField]
        SettingKinect settingKinect;

        /// <summary>
        /// W zależności od stanu, wyświetla odpowiedni opis na ekranie co klatkę.
        /// </summary>
        void Update() {
            switch (settingKinect.playerDistanceStatus) {
                case PlayerDistanceStatus.TooClose:
                    SetText("Body is too close");
                    SetColor(Color.yellow);
                    break;
                case PlayerDistanceStatus.TooFar:
                    SetText("Body is too far");
                    SetColor(Color.yellow);
                    break;
                case PlayerDistanceStatus.Perfect:
                    SetText("Body is in optimal position");
                    SetColor(Color.green);
                    break;
                default:
                    SetText("Body is undetected");
                    SetColor(Color.red);
                    break;
            }
        }

    }
}
