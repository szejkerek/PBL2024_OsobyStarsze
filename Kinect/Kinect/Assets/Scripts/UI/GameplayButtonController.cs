using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameplayButtonController : MonoBehaviour
    {
        private const string SceneName = "MainMenu";

        public void BackToMenuButton()
        {
            SceneManager.LoadScene(SceneName);
            Time.timeScale = 1;
        }
    
    }
}
