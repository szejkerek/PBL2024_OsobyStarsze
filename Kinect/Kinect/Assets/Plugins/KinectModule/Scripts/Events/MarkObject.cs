using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.KinectModule.Events {

    /// <summary>
    /// Klasa, służąca do chwilowego zmiany koloru obiektu.
    /// </summary>
    public class MarkObject : MonoBehaviour {

        /// <summary>
        /// Czas potrzebny, aby wrócić do domyślnego koloru.
        /// </summary>
        [SerializeField]
        float timeToBackDefault = 2f;

        /// <summary>
        /// Kolor, który jest zamieniany z domyślnym kolorem obiektu na określony czas.
        /// </summary>
        [SerializeField]
        Color markColor = Color.red;

        /// <summary>
        /// Lista domyślnych kolorów w obiekcie.
        /// </summary>
        Color[] defaultColors;
        /// <summary>
        /// Lista wszystkich obiektów klasy MeshRenderer w obiekcie i jego dzieciach.
        /// </summary>
        MeshRenderer[] renderers;

        void Awake() {
            renderers = GetComponentsInChildren<MeshRenderer>();
            defaultColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++) {
                defaultColors[i] = renderers[i].material.color;
            }
        }

        /// <summary>
        /// Zmień kolor na tymczasowy.
        /// </summary>
        public void Mark() {
            StopAllCoroutines();
            StartCoroutine(FlashModel());
        }

        /// <summary>
        /// Zmienia kolor na tymczasowy, a następnie po czasie timeToBackDefault, wraca do domyślnego koloru.
        /// </summary>
        /// <returns>IEnumerator</returns>
        IEnumerator FlashModel() {
            if (renderers == null) {
                yield break;
            }

            for (int i = 0; i < renderers.Length; i++) {
                renderers[i].material.color = markColor;
            }

            yield return new WaitForSeconds(timeToBackDefault);

            for (int i = 0; i < renderers.Length; i++) {
                renderers[i].material.color = defaultColors[i];
            }
        }


    }
}
