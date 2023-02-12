using UnityEngine;

namespace GameUI
{
    public class GameMenu : MonoBehaviour
    {
        [SerializeField] private GameObject menu;

        private GameManager _gameManager;

        public void LoadMainMenu()
        {
            _gameManager ??= FindObjectOfType<GameManager>();
            
            _gameManager.Disconnect();
        }

        public void Continue()
        {
            GameManager.CanInput = true;
            menu.SetActive(false);
            Cursor.visible = false;
        }

        private void Start()
        {
            Cursor.visible = false;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                bool newState = !menu.activeSelf;

                GameManager.CanInput = !newState;
                menu.SetActive(newState);
                Cursor.visible = newState;
            }
        }
    }
}