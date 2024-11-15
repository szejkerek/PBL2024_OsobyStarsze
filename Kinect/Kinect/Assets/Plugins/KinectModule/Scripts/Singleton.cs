using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.KinectModule {

    /// <summary>
    /// Klasa reprezentująca wzorzec Singletonu.
    /// </summary>
    /// <typeparam name="T">Typ MonoBehaviour.</typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

        /// <summary>
        /// Prywatna zmienna przechowująca referncje do instancji tej klasy.
        /// </summary>
        static T instance = null;

        /// <summary>
        /// Właściwość do zwracania danej instancji singletonu. Jeżeli nieistnieje to towrzy nową instację na scenie.
        /// </summary>
        public static T Instance {
            get {
                if (instance == null) {
                    instance = FindObjectOfType<T>();
                    if (instance == null) {
                        instance = (new GameObject(typeof(T).Name)).AddComponent<T>();
                    }
                }
                return instance;
            }
        }


    }

}
