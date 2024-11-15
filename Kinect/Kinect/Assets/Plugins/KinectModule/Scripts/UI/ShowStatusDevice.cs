using System.Collections;
using System.Collections.Generic;
using Plugins.KinectModule.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.KinectModule.UI {

    /// <summary>
    /// Klasa służąca do pokazywania stanu urządzenia na ekranie.
    /// </summary>
    public class ShowStatusDevice : TextSetter {

        /// <summary>
        /// Referencja do KinectConnection.
        /// </summary>
        [SerializeField]
        KinectConnection connection;

        /// <summary>
        /// W zależności od stanu, wyświetla odpowiedni opis na ekranie co klatke.
        /// </summary>
        void Update() {
            switch (connection.status) {
                case KinectStatus.Starting:
                    SetText("Device is starting...");
                    SetColor(Color.yellow);
                    break;
                case KinectStatus.Running:
                    SetText("Device is running...");
                    SetColor(Color.green);
                    break;
                default:
                    SetText("Device is turned off");
                    SetColor(Color.red);
                    break;
            }
        }
    }

}
