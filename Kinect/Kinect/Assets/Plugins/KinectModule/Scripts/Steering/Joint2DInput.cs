using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Plugins.KinectModule.Data;
using Plugins.KinectModule.SaveData;


namespace Plugins.KinectModule.Steering {

    /// <summary>
    /// Klasa s³u¿¹ca do sterowania obiektem za pomoc¹ urz¹dzenia Azure Kinect.
    /// </summary>
    public class Joint2DInput : PositionUpdaterInput {

        /// <summary>
        /// Referencja do BodyTracker.
        /// </summary>
        [SerializeField]
        BodyTracker bodyTracker;

        [Space()]

        /// <summary>
        /// Staw sterujacy, który s³u¿y do sterowania obiektem.
        /// </summary>
        [SerializeField]
        JointId steeringJointId;

        [SerializeField] RectTransform image;

        /// <summary>
        /// Pozycja stawu steruj¹cego.
        /// </summary>
        Vector2 steeringJointPosition;

        Vector3[] bounds;
        Rect rectImage;

        private void Start() {
            
            
        }

        /// <summary>
        /// Co klatke aktualizuje pozycje obiektu. 
        /// </summary>
        void Update() {
            bounds = positionUpdater.GetBoundsWorldPositions();
            if (bodyTracker.CanGetUpdateData()) {
                steeringJointPosition = bodyTracker.GetDataByJoint(steeringJointId);
                steeringJointPosition = (steeringJointPosition + Vector2.one) / 2f;
                steeringJointPosition.x = 1f - steeringJointPosition.x;
                
                Vector3[] vector3s = new Vector3[4];
                image.GetWorldCorners(vector3s);
                Vector3 leftBottom = vector3s[0];
                Vector3 rightTop = vector3s[2];

                rectImage = new Rect(leftBottom.x, Screen.height - rightTop.y, rightTop.x - leftBottom.x, rightTop.y - leftBottom.y);

                steeringJointPosition.x *= (rectImage.width / Screen.width);
                steeringJointPosition.y *= (rectImage.height / Screen.height);

                steeringJointPosition += new Vector2(rectImage.x / Screen.width, rectImage.y / Screen.height);
            }

            Vector3 leftTopB = Camera.main.WorldToViewportPoint(bounds[0]);
            Vector3 rightTopB = Camera.main.WorldToViewportPoint(bounds[1]);
            Vector3 rightBottomB = Camera.main.WorldToViewportPoint(bounds[2]);
            Vector3 leftBottomB = Camera.main.WorldToViewportPoint(bounds[3]);

            float widthPanelTop = rightTopB.x - leftTopB.x;
            float widthPanelBottom = rightBottomB.x - leftBottomB.x;
            float heightPanel = rightTopB.y - rightBottomB.y;
            
            Vector3 tValues = new Vector3(steeringJointPosition.x, steeringJointPosition.y, 0);

            tValues.y = Mathf.Clamp01((steeringJointPosition.y - rightBottomB.y) / heightPanel);
            tValues.x = Mathf.Lerp(
                ((steeringJointPosition.x - leftTopB.x) / widthPanelTop),
                ((steeringJointPosition.x - leftBottomB.x) / widthPanelBottom) ,
                tValues.y
                );

            tValues = CastPositionFromSkeletonOnPanel(tValues);

            positionUpdater.Update(tValues);
        }

        /// <summary>
        /// Zapisuje informacje o po³o¿eniu i rotacji stawów do pliku.
        /// </summary>
        public void SaveInformationAboutPosition() {
            if (!enabled) {
                return;
            }

            CollectData.Instance.CollectRow(new string[] { "Steering Position", steeringJointId.ToString() });
            CollectData.Instance.AddToCurrentRow(new Vector3[] { steeringJointPosition });
        }

        private void OnGUI() {
            ///GUI.Box(new Rect(Camera.main.ViewportToScreenPoint(steeringJointPosition), new Vector2(50, 50)), "X");
        }

    }

}
