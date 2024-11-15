using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
//using NaughtyAttributes;


namespace Plugins.KinectModule.Steering {

    /// <summary>
    /// Klasa służąca do aktualizacji pozycji obiektu.
    /// </summary>
    public class PositionUpdater : MonoBehaviour {

        /// <summary>
        /// Gładkość aktualizacji pozycji.
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        float smoothTime = 0.5f;

        /// <summary>
        /// Maksymalna prędkość gładkiej aktualizacji pozycji.
        /// </summary>
        [SerializeField]
        float maxSpeed = 20f;

        /// <summary>
        /// Przesunięcie pozycji obiektu.
        /// </summary>
        [SerializeField]
        Vector3 offsetPosition;

        /// <summary>
        /// Granice poruszania się obiektu.
        /// </summary>
        [SerializeField]
        Vector3 panelBounds;

        /// <summary>
        /// Zmienna do przechowywania aktualnej wartości pozycji. 
        /// </summary>
        Vector3 refSmoothPosition;

        void Update() { }

        /// <summary>
        /// Metoda rysuje granice poruszania się obiektu po panelu.
        /// </summary>
        void OnDrawGizmosSelected() {
            Gizmos.DrawWireCube(transform.TransformPoint(Vector3.zero + offsetPosition), panelBounds * 2);
        }

        /// <summary>
        /// Aktualizuje pozycje na podstawie zmiennej tValues. Zmienna ta zawiera wartości procentowe określające stosunek do wartości granicznych określanych w zmiennej panelBounds.
        /// </summary>
        /// <param name="tValues">Wartości procentowe zmiennej panelBounds</param>
        public void Update(Vector3 tValues) {

            Vector3 newPosition = new Vector3 {
                x = Mathf.Lerp(-panelBounds.x, panelBounds.x, tValues.x),
                y = Mathf.Lerp(-panelBounds.y, panelBounds.y, tValues.y),
                z = Mathf.Lerp(-panelBounds.z, panelBounds.z, tValues.z)
            };
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, newPosition + offsetPosition, ref refSmoothPosition, smoothTime, maxSpeed);

        }

        public Vector3[] GetBoundsWorldPositions() {
            Vector3[] bounds = new Vector3[8];

            bounds[0] = LocalToWorld(new Vector3(-panelBounds.x, panelBounds.y, panelBounds.z));
            bounds[1] = LocalToWorld(new Vector3(panelBounds.x, panelBounds.y, panelBounds.z));
            bounds[2] = LocalToWorld(new Vector3(panelBounds.x, panelBounds.y, -panelBounds.z));
            bounds[3] = LocalToWorld(new Vector3(-panelBounds.x, panelBounds.y, -panelBounds.z));

            bounds[4] = LocalToWorld(new Vector3(-panelBounds.x, -panelBounds.y, panelBounds.z));
            bounds[5] = LocalToWorld(new Vector3(panelBounds.x, -panelBounds.y, panelBounds.z));
            bounds[6] = LocalToWorld(new Vector3(panelBounds.x, -panelBounds.y, -panelBounds.z));
            bounds[7] = LocalToWorld(new Vector3(-panelBounds.x, -panelBounds.y, -panelBounds.z));

            return bounds;
        }

        Vector3 LocalToWorld(Vector3 pos) {
            if (transform.parent) {
                return transform.parent.TransformPoint(pos + offsetPosition);
            }
            return pos + offsetPosition;
        }
    }

}

