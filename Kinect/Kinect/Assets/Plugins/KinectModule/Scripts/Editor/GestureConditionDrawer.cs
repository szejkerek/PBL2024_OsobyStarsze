using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Plugins.KinectModule.Gesture {

    /// <summary>
    /// Klasa odpowiedzialna za odpowiednie wyświetlanie parametrów w oknie Inspector w klasie GestureCondition
    /// </summary>
    [CustomPropertyDrawer(typeof(GestureCondition))]
    public class GestureConditionDrawer : PropertyDrawer {

        /// <summary>
        /// Wysokość parametru w oknie
        /// </summary>
        const float heightPerParamter = 20;
        /// <summary>
        /// Odległość pomiędzy parametrami
        /// </summary>
        const float space = 5;

        /// <summary>
        /// Zwraca wysokość jaką zajmuje GestureCondition
        /// </summary>
        /// <param name="prop">Argument typu SerializedProperty</param>
        /// <param name="label">Argument typu GUIContent</param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
            SerializedProperty condition = prop.FindPropertyRelative("_condition");
            int condValue = condition.enumValueIndex;
            return (condValue > 5 ? heightPerParamter * 3 : heightPerParamter * 2) + space;
        }

        /// <summary>
        /// Domyślna metoda odpowiedzalan za wyświetlanie obiektu klasy GestureCondition
        /// </summary>
        /// <param name="position">Pozycja w oknie Inspector</param>
        /// <param name="property">Serializowana zmienna, która odnosi się do obiektu klasy GestureCondition</param>
        /// <param name="label">Argument typu GUIContent</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            float widthPerParametr = position.width / 3;

            Rect rectFirstJointId = new Rect(position.x, position.y, widthPerParametr, heightPerParamter);
            Rect rectCondition = new Rect(rectFirstJointId.x + widthPerParametr, rectFirstJointId.y, widthPerParametr, heightPerParamter);
            Rect rectSecondJointId = new Rect(rectCondition.x + widthPerParametr, rectCondition.y, widthPerParametr, heightPerParamter);

            SerializedProperty firstJointId = property.FindPropertyRelative("_firstJointId");
            SerializedProperty condition = property.FindPropertyRelative("_condition");
            SerializedProperty secondJointId = property.FindPropertyRelative("_secondJointId");

            EditorGUI.PropertyField(rectFirstJointId, firstJointId, GUIContent.none);
            EditorGUI.PropertyField(rectCondition, condition, GUIContent.none);
            EditorGUI.PropertyField(rectSecondJointId, secondJointId, GUIContent.none);

            int condValue = condition.enumValueIndex;

            Rect rectExpireTime = new Rect(position.x, rectSecondJointId.y, position.width, heightPerParamter);

            if (condValue > 5 && condValue < 8) {
                Rect rectAngleValue = new Rect(position.x, rectSecondJointId.y + heightPerParamter, position.width, heightPerParamter);
                SerializedProperty angleValue = property.FindPropertyRelative("_angleValue");
                EditorGUI.PropertyField(rectAngleValue, angleValue, new GUIContent("Angle"));
                rectExpireTime.y = rectAngleValue.y;

            } else if (condValue >= 8) {
                Rect rectDistanceValue = new Rect(position.x, rectSecondJointId.y + heightPerParamter, position.width, heightPerParamter);
                SerializedProperty distanceValue = property.FindPropertyRelative("_distanceValue");
                EditorGUI.PropertyField(rectDistanceValue, distanceValue, new GUIContent("Distance"));
                rectExpireTime.y = rectDistanceValue.y;
            }

            rectExpireTime.y += heightPerParamter;

            SerializedProperty expireTime = property.FindPropertyRelative("_expireTime");
            EditorGUI.PropertyField(rectExpireTime, expireTime);

            EditorGUI.EndProperty();
        }


    }

}
