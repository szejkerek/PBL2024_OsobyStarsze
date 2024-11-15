using UnityEngine;

namespace UI
{
    public class GameOverPanel : MonoBehaviour
    {
        public void Enable()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    
        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
