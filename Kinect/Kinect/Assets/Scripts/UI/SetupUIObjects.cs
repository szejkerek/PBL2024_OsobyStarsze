using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UI {

    public class SetupUIObjects : MonoBehaviour 
    {

        [SerializeField] private List<RectTransform> objectsToEnableOnStart = new List<RectTransform>();
        [SerializeField] private List<RectTransform> objectsToDisableOnStart = new List<RectTransform>();

        void Start() 
        {
            objectsToEnableOnStart.ForEach(x => x.gameObject.SetActive(true));
            objectsToDisableOnStart.ForEach(x => x.gameObject.SetActive(false));
        }

    }
}
