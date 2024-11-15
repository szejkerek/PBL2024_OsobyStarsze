using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Plugins.KinectModule {
    /// <summary>
    /// Klasa przechowujacą statyczne metody do ogólnego przeznaczenia rozszerzając funkcjonalność działania modułu.
    /// </summary>
    public static class Utility {

        /// <summary>
        /// Metoda konwertująca System.Numerics.Vector3 na UnityEngine.Vector3.
        /// </summary>
        /// <param name="vector3">System.Numerics.Vector3 do konwersji.</param>
        /// <returns>Zwraca UnityEngine.Vector3.</returns>
        public static Vector3 ConvertToUnityEngine(this System.Numerics.Vector3 vector3) {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        public static Vector2 ConvertToUnityEngine(this System.Numerics.Vector2 vector2) {
            return new Vector2(vector2.X, vector2.Y);
        }

        /// <summary>
        /// Metoda konwertująca System.Numerics.Quaternion na UnityEngine.Quaternion.
        /// </summary>
        /// <param name="quaternion">System.Numerics.Quaternion do konwersji.</param>
        /// <returns>Zwraca UnityEngine.Quaternion.</returns>
        public static Quaternion ConvertToUnityEngine(this System.Numerics.Quaternion quaternion) {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

    }
}
