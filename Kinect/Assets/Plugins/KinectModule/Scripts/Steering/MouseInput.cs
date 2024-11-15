using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Plugins.KinectModule.Steering {

    /// <summary>
    /// Klasa służąca do sterowania obiektem za pomocą myszki.
    /// </summary>
    public class MouseInput : PositionUpdaterInput {

        /// <summary>
        /// Referencja do głównej kamery z sceny.
        /// </summary>
        [SerializeField]
        new Camera camera;

        /// <summary>
        /// Maksymalne odchylenie myszki na ekranie.  
        /// </summary>
        [SerializeField]
        Vector2 maxViewport;

        /// <summary>
        /// Przycisk, który włącza i wyłącza tryb sterowania myszką. 
        /// </summary>
        [SerializeField]
        KeyCode enterExitMode;

        /// <summary>
        /// Referencja do metody, która wykonuje się co klatke na sekundę.
        /// </summary>
        System.Action update = null;

        /// <summary>
        /// W zależności czy jest zainicjowana zmienna update jest ona wywoływana. W zmiennej update jest referencja do metody UpdatePositionByMouse.
        /// </summary>
        void Update() {
            if (Input.GetKeyDown(enterExitMode)) {
                if (update == null) {
                    update = new System.Action(UpdatePositionByMouse);
                } else {
                    update = null;
                }
            }

            update?.Invoke();
        }

        /// <summary>
        /// Aktualizuje pozycje obiektu za pomocą pozycji kursora na ekranie.
        /// </summary>
        void UpdatePositionByMouse() {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = camera.nearClipPlane;
            Vector3 viewport = camera.ScreenToViewportPoint(mousePosition);

            Vector3 tValues = new Vector3 {
                x = Mathf.Clamp(viewport.x / maxViewport.x, 0f, 1f),
                y = Mathf.Clamp(viewport.y / maxViewport.y, 0f, 1f),
                z = Mathf.Clamp(viewport.z, 0f, 1f)
            };

            tValues = CastPositionFromSkeletonOnPanel(tValues);

            positionUpdater.Update(tValues);
        }

    }

}
