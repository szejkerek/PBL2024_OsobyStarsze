using UnityEngine;

namespace Tutorial
{
    public class TutorialFrame : MonoBehaviour
    {
        public void Enable()
        {
            gameObject.SetActive(true);
        }
    
        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
