using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Plugins.KinectModule.Steering;

namespace Plugins.KinectModule.UI {

    /// <summary>
    /// Klasa służąca do wyświetlania opcji na ekranie do przełączania sterowania pomiędzy Azure Kinect, a myszką.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class ToggleKinect : MonoBehaviour {

        /// <summary>
        /// List JointInput aktywynych podczas sterowania Azure Kinect.
        /// </summary>
        [SerializeField] List<JointInput> jointInputs = new List<JointInput>();
        /// <summary>
        /// List MouseInput aktywynych podczas sterowania myszką.
        /// </summary>
        [SerializeField] List<MouseInput> mouseInputs = new List<MouseInput>();
        /// <summary>
        /// List elementów UI aktywynych podczas sterowania Azure Kinect.
        /// </summary>
        [SerializeField] List<RectTransform> UIKinect = new List<RectTransform>();

        /// <summary>
        /// Referencja do przełącznika na ekranie.
        /// </summary>
        Toggle toggle;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        void Awake() {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(ChangeKinectByValue);
            ChangeKinectByToggle();
        }

        /// <summary>
        /// Destruktor.
        /// </summary>
        void OnDestroy() {
            if (toggle != null && this != null) {
                toggle.onValueChanged.RemoveListener(ChangeKinectByValue);
            }
        }

        /// <summary>
        /// Zmienia tryb sterowania na podstawie wartości przełącznika.
        /// </summary>
        public void ChangeKinectByToggle() {
            ChangeKinectByValue(toggle.isOn);
        }

        /// <summary>
        /// Zmienia tryb sterowania na podstawie parametru.
        /// </summary>
        /// <param name="value">Jeśli true to sterowanie Azure Kinect, jeśli false to sterowanie myszką.</param>
        void ChangeKinectByValue(bool value) {
            jointInputs.ForEach(x => x.enabled = value);
            mouseInputs.ForEach(x => x.enabled = !value);
            UIKinect.ForEach(x => x.gameObject.SetActive(value));
        }

    }

}
