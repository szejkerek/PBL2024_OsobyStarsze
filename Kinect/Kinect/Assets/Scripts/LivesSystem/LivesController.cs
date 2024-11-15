using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace LivesSystem
{
    public class LivesController : MonoBehaviour
    {
        [SerializeField] private List<Heart> hearts;

        public int lives;

        private void Awake()
        {
            GetReferences();
        }

        private void GetReferences()
        {
            hearts = new List<Heart>(GetComponentsInChildren<Heart>(true));
        }

        private void Start()
        {
            foreach (Heart heart in hearts)
            {
                heart.Enable();
            }
            lives = hearts.Count;
        }

        public void ToggleHearts(int hits)
        {

            for (int i=0; i < hits && i < hearts.Count; i++) {
                Heart heart = hearts[i];
                heart.transform.DOScale(Vector3.one * 0.001f, 0.5f).OnComplete(() => { heart.Disable(); });
            }
            lives--;
        }
    
    }
}
