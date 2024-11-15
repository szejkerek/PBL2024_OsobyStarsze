using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using Plugins.KinectModule.Data;

namespace Plugins.KinectModule.UI {

    /// <summary>
    /// Klasa służąca do pokazywania na ekranie wizualizacji użytkownika.
    /// </summary>
    public class ShowImage : MonoBehaviour {

        /// <summary>
        /// Referencja do RawImage, w którym ma się wyswietlać postać.
        /// </summary>
        [SerializeField]
        RawImage rawImage;

        /// <summary>
        /// Referencja do BodyTracker.
        /// </summary>
        [SerializeField]
        BodyTracker bodyTracker;

        /// <summary>
        /// Referencja do tekstury.
        /// </summary>
        Texture2D texture;
        /// <summary>
        /// Kolor domyślny obrazu.
        /// </summary>
        Color startColor;
        /// <summary>
        /// Przechowuje informacje czy dane są pobierane pierwszy raz.
        /// </summary>
        bool firstTime = true;

        /// <summary>
        /// Ustawia wartości domyślne na starcie żywotności obietku.
        /// </summary>
        void Start() {
            texture = null;
            firstTime = true;
            startColor = rawImage.color;
            rawImage.color = new Color(0, 0, 0, 0);
        }

        /// <summary>
        /// Aktualizuje obraz co klatke.
        /// </summary>
        void Update() {
            if (bodyTracker.CanGetUpdateData()) {
                bodyTracker.GetFrameTexture(ref texture);
                rawImage.texture = texture;
                if (firstTime && texture != null) {
                    rawImage.color = startColor;
                    firstTime = false;
                    GetComponent<AspectRatioFitter>().aspectRatio = (float)rawImage.texture.width / rawImage.texture.height;
                }
            }
        }

        /// <summary>
        /// Na niszczeniu obiektu usuwa teksture z pamięci.
        /// </summary>
        void OnDestroy() {
            if (texture != null) {
                DestroyImmediate(texture);
            }
        }

    }
}
