using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using NaughtyAttributes;

namespace Plugins.KinectModule.Gesture {

    /// <summary>
    /// Klasa reprezentująca pojedynczy gest.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Gesture", fileName = "Gesture")]
    public class Gesture : ScriptableObject {

        /// <summary>
        /// Lista warunków do spełnienia, aby dany gest został rozpoznany.
        /// </summary>
        [SerializeField]
        List<GestureCondition> _gestureConditions = new List<GestureCondition>();
        /// <summary>
        /// Getter do zwracania danych gestów.
        /// </summary>
        public List<GestureCondition> gestureConditions => new List<GestureCondition>(_gestureConditions);

        /// <summary>
        /// Metoda pomocnicza, która zmienia stronę stawów np. lewą kostke (AnkleRight) na prawą kostke (AnkleLeft).
        /// </summary>
        //[Button("Turn side")]
        public void TurnSideForJointId() {
            foreach (GestureCondition gestureCondition in gestureConditions) {
                gestureCondition.TurnSide();
            }
        }

    }

}
