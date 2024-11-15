using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField]
        private List<TutorialFrame> frames;
        [SerializeField]
        private Button leftArrowButton;
        [SerializeField]
        private Button rightArrowButton;
        [SerializeField]
        private Button backToMenuButton;

        private TutorialFrame currentFrame;
        private int currentFrameIndex;
        public void Enable()
        {
            gameObject.SetActive(true);
            ActivateFirstFrame();
        }

        private void Awake()
        {
            GetReferences();
            leftArrowButton.onClick.AddListener(ToggleToPreviousFrame);
            rightArrowButton.onClick.AddListener(ToggleToNextFrame);
            backToMenuButton.onClick.AddListener(FinishTutorial);
        }

        private void Update()
        {
            ToggleButtons();
        }

        private void ActivateFirstFrame()
        {
            foreach (TutorialFrame frame in frames)
            {
                frame.Disable();
            }
            ToggleFrame(0);
            currentFrame = frames[0];
        }

        private void FinishTutorial()
        {
            Disable();
        }

        private void ToggleToNextFrame()
        {
            ToggleFrame(currentFrameIndex + 1);
        }

        private void ToggleToPreviousFrame()
        {
            ToggleFrame(currentFrameIndex - 1);
        }

        private void ToggleFrame(int index)
        {
            currentFrameIndex = Mathf.Clamp(index, 0, frames.Count - 1);
            currentFrame?.Disable();
            currentFrame = frames[currentFrameIndex];
            currentFrame.Enable();

        }
        private void GetReferences()
        {
            frames = new List<TutorialFrame>(GetComponentsInChildren<TutorialFrame>());
        }

        private void Disable()
        {
            currentFrame?.Disable();
            gameObject.SetActive(false);
        }
        private void ToggleButtons()
        {
            leftArrowButton.interactable = currentFrameIndex > 0;
            rightArrowButton.interactable = currentFrameIndex < frames.Count - 1;
        }

    }
}
