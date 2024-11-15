using Tutorial;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MenuButtonsController : MonoBehaviour
    {

        [SerializeField]
        private TutorialController tutorial;

        [SerializeField]
        private RectTransform mainMenu;

        [SerializeField]
        private RectTransform optionMenu;

        [SerializeField]
        private RectTransform tutorialMenu;

        private const string SceneName = "NewGameplay";//"GameplayScene";

        public AudioSource AudioSource;

        public void QuitButton()
        {
            Application.Quit();
           // Debug.Log("Wyjscie z gry");
           // for the purpose of editing
            UnityEditor.EditorApplication.isPlaying = false;

        }

        public void TutorialButton()
        {
            tutorial.Enable();
        }
        public void StartButton()
        {
            SceneManager.LoadScene(SceneName);
        }

        public void SettingButton() {
            mainMenu.gameObject.SetActive(false);
            optionMenu.gameObject.SetActive(true);
        }

    }
}
