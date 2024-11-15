using UnityEngine;
using UnityEngine.Events;

namespace Plugins.KinectModule.Scripts.Events {

    /// <summary>
    /// Klasa reprezentująca interaktywny obiekt.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TriggerKinectObject : MonoBehaviour {

        /// <summary>
        /// Zdarzenie do wywołania po wywołaniu metody Call.
        /// </summary>
        public UnityEvent onTrigger;

        /// <summary>
        /// Metoda, która wywołuje zdarzenie.
        /// </summary>
        public void Call() {
            onTrigger?.Invoke();
        }

    }

}
