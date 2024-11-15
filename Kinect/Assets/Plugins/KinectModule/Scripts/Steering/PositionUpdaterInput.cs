using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Plugins.KinectModule.Steering {

    /// <summary>
    /// Klasa bazowa, reprezentująca wejście do aktualizacji pozycji obiektu.
    /// </summary>
    [RequireComponent(typeof(PositionUpdater))]
    public abstract class PositionUpdaterInput : MonoBehaviour {

        /// <summary>
        /// Rzutowanie pozycji wejściowej wybranej osi na oś x. 
        /// </summary>
        [SerializeField]
        CastedAxis positionX = CastedAxis.X;
        /// <summary>
        /// Rzutowanie pozycji wejściowej wybranej osi na oś y. 
        /// </summary>
        [SerializeField]
        CastedAxis positionY = CastedAxis.Y;
        /// <summary>
        /// Rzutowanie pozycji wejściowej wybranej osi na oś z. 
        /// </summary>
        [SerializeField]
        CastedAxis positionZ = CastedAxis.Z;

        /// <summary>
        /// Referencja do PositionUpdater.
        /// </summary>
        protected PositionUpdater positionUpdater;

        void Awake() {
            positionUpdater = GetComponent<PositionUpdater>();
        }

        /// <summary>
        /// Zamiana wartości zmiennej typu Vector3 na inne osie.
        /// </summary>
        /// <param name="position">Wartość wejściowa.</param>
        /// <returns>Wartość wyjściowa.</returns>
        protected Vector3 CastPositionFromSkeletonOnPanel(Vector3 position) {
            return new Vector3 {
                x = ChooseAxisFromVector(position, positionX),
                y = ChooseAxisFromVector(position, positionY),
                z = ChooseAxisFromVector(position, positionZ)
            };
        }

        /// <summary>
        /// Zwraca wartość danej osi określonej w castedAxis.
        /// </summary>
        /// <param name="vector">Vector3 wejściowy.</param>
        /// <param name="castedAxis">Określna, którą oś zwrócić.</param>
        /// <returns>Zwracana wartość wybranej osi.</returns>
        protected float ChooseAxisFromVector(Vector3 vector, CastedAxis castedAxis) {
            switch (castedAxis) {
                case CastedAxis.X:
                    return vector.x;
                case CastedAxis.Y:
                    return vector.y;
                case CastedAxis.Z:
                    return vector.z;
                case CastedAxis.inverseX:
                    return 1f - vector.x;
                case CastedAxis.inverseY:
                    return 1f - vector.y;
                case CastedAxis.inverseZ:
                    return 1f - vector.z;
            }
            return 0f;
        }

        /// <summary>
        /// Typ wyliczeniowy określający rzutowanie na daną oś.
        /// </summary>
        public enum CastedAxis {
            X,
            Y,
            Z,
            inverseX,
            inverseY,
            inverseZ,
            None
        }

    }
}
