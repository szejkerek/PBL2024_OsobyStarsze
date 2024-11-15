using UnityEngine;

namespace UI
{
    public class PausePanel : MonoBehaviour
    {
        public void Enable()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        public void Disable()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
