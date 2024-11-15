using Plugins.KinectModule.Events;
using Plugins.KinectModule.Scripts.Events;
using UnityEngine;
using UnityEngine.UI;


namespace Plugins.KinectModule.Scripts.UI {

    /// <summary>
    /// Klasa służąca do pokazania czasu do wywołania zdarzenia z interaktywnym obiektem. 
    /// </summary>
    public class ShowTriggerProgress : MonoBehaviour {

        /// <summary>
        /// Referencja do CheckCollision. 
        /// </summary>
        [SerializeField]
        private CheckCollision checkCollision;

        /// <summary>
        /// Zmienna przechowująca obiekt klasy Text, w którym pokazywany jest czas na ekranie.
        /// </summary>
        [SerializeField]
        private Text timeText;

        /// <summary>
        /// Określa obiekt, który powinien zmienić pozycje względem obiektu interaktywnego.
        /// </summary>
        [SerializeField]
        private GameObject timer;

        /// <summary>
        /// Obrazek, reprezentujący postęp czasu do wywołania zdarzenia.
        /// </summary>
        [SerializeField]
        private Image timeImage;

        /// <summary>
        /// Przesunięcie względem pozycji obiektu interaktywnego.
        /// </summary>
        [SerializeField]
        private Vector3 offsetPosition;


        [SerializeField]
        private Rigidbody rb;

        /// <summary>
        /// Na starcie działania obiektu dodaje metodę Show do zdarznia OnHover obiektu checkCollision.
        /// </summary>
        private void Start() {

            if (checkCollision != null) {

                checkCollision.onHover.AddListener(ShowTimer);
            }
        }

        /// <summary>
        /// Na starcie działania obiektu usuwa metodę Show z zdarznia OnHover obiektu checkCollision.
        /// </summary>
        private void OnDestroy() {
            checkCollision.onHover.RemoveListener(ShowTimer);
        }

        /// <summary>
        /// Pokazuje czas i postęp, który określa ile zostało do wywołania zdarzenia w checkCollision.
        /// </summary>
        /// 


        /// ZMIANY TUTAJ!!!!
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 

        private void ShowTimer() {

            if (!checkCollision.IsHover || checkCollision.wasFull || checkCollision.colliderInteracts == false)
            {
                timer.SetActive(false);
                return;
            }

            if (checkCollision.colliderInteracts == true)
            {
                timer.SetActive(true);
                timer.transform.localPosition = checkCollision.PositionTrigger + offsetPosition;
                timeText.text = Mathf.Ceil(checkCollision.TimeToTrigger).ToString("0");
                timeImage.fillAmount = checkCollision.TimeToTriggerInPercent;

                checkCollision.colliderInteracts = false;
            }

        }

    }
}
