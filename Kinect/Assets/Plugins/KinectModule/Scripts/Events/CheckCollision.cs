using UnityEngine;
using UnityEngine.Events;

namespace Plugins.KinectModule.Scripts.Events
{
    /// <summary>
    ///     Klasa wywłująca zdarzenie, jeśli znalazła kolizję.
    /// </summary>
    public class CheckCollision : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        ///     Zasięg wykrywania kolizji.
        /// </summary>
        [SerializeField]
        private float radiusHit = 1f;

        /// <summary>
        ///     Czas jaki jest potrzebny do wywołania zdarzenia przy stałej kolizji.
        /// </summary>
        [SerializeField]
        private float timeToTrigger = 3f;

        #endregion

        #region Private Fields

        /// <summary>
        ///     Aktualny interaktywny obiekt w stałej kolizji.
        /// </summary>
        private TriggerKinectObject currentHoverTrigger;
        //public string currentObject = "";

        /// <summary>
        ///     Zmienna przechowująca aktualny czas.
        /// </summary>
        private float timer;

        #endregion

        /// <summary>
        ///     Zdarzenie wywoływane, przy stałej kolizji z obiektem interaktywnym po czasie wyznaczonym w zmiennej timeToTrigger.
        /// </summary>
        public UnityEvent onCall;

        /// <summary>
        ///     Zdarzenie wywoływane, przy stałej kolizji z obiektem interaktywnym.
        /// </summary>
        public UnityEvent onHover;
        public bool wasFull;

        #region Public Properties

        /// <summary>
        ///     Czy jest w aktualnej kolizji z jakimś interaktywnym obiektem.
        /// </summary>
        public bool IsHover => currentHoverTrigger != null;
        /// <summary>
        ///     Czas do wywołania zdarzenia.
        /// </summary>
        public float TimeToTrigger => timeToTrigger > 0 ? timeToTrigger - timer : 0f;
        /// <summary>
        ///     Czas do wywołania zdarzenia w procentach.
        /// </summary>
        public float TimeToTriggerInPercent => timeToTrigger > 0 ? timer / timeToTrigger : 1f;
        /// <summary>
        ///     Pozycja aktualnego interaktywnego obiektu.
        /// </summary>
        public Vector3 PositionTrigger => currentHoverTrigger != null ? currentHoverTrigger.transform.position : Vector3.zero;

        /////////////////////////////
        /// <summary>
        ///     Parametr zmieniający stan licznika: if true - pokazuje licznik podczas zbierania.
        /// </summary>
        public bool colliderInteracts = false;
        ///

        #endregion

        #region Unity Callbacks

        /// <summary>
        ///     Jeżeli checkOnUpdate jest ustawiony na true, to co klatke jest wywoływana metoda Check.
        /// </summary>
        private void Update()
        {
            Check();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Ustawia czas do wywołania zdarzenia.
        /// </summary>
        /// <param name="time">Nowy czas w sekundach.</param>
        public void SetTriggerTime(float time) {
            timeToTrigger = time;
        }

        /// <summary>
        ///     Sprawdza czy nie występuje kolizja w wyznaczonym zasięgu z najbliższym interaktywnym obiektem.
        /// </summary>
        public void Check()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radiusHit);
            float minDistance = float.MaxValue;
            TriggerKinectObject trigger = null;

            foreach (Collider collider in colliders)
            {
                if (collider.isTrigger)
                {
                    colliderInteracts = true;
                }
                else
                {
                    colliderInteracts = false;
                }

                TriggerKinectObject currentTrigger = collider.GetComponent<TriggerKinectObject>();

                if (currentTrigger)
                {
                    float currentDistance = Vector3.Distance(collider.transform.position, transform.position);

                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        trigger = currentTrigger;
                    }
                }
            }
            
            Hover(trigger);
            
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Metoda, która odlicza czas potrzebny do wywołania zdarzenia i wywołuje odpowiednie zdarzenia.
        /// </summary>
        /// <param name="trigger">Aktualnie interaktywny obiekt</param>
        private void Hover(TriggerKinectObject trigger)
        {

            if (trigger != currentHoverTrigger)
            {
                currentHoverTrigger = trigger;
                timer = 0f;
                wasFull = false;
            }
            else if (currentHoverTrigger != null)
            {
                timer += Time.deltaTime;

                if (timer >= timeToTrigger)
                {
                    onCall?.Invoke();
                    currentHoverTrigger.Call();
                    wasFull = true;
                    currentHoverTrigger = null;
                }
            }
            
            onHover?.Invoke();
                
        }

        #endregion
    }
}