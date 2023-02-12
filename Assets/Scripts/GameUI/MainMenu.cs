using Mirror;
using TMPro;
using UnityEngine;

namespace GameUI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nicknameInput;
        [SerializeField] private TMP_InputField hostInput;

        private GameManager _gameManager;

        public void Exit()
        {
            Application.Quit();
        }

        public void Connect()
        {
            _gameManager ??= FindObjectOfType<GameManager>();
            
            PlayerPrefs.SetString("Nickname", nicknameInput.text);

            _gameManager.Connect(hostInput.text);
        }

        public void CreateRoom()
        {
            _gameManager ??= FindObjectOfType<GameManager>();

            PlayerPrefs.SetString("Nickname", nicknameInput.text);

            _gameManager.Host();
        }

        public void Refresh()
        {
        }

        private void Start()
        {
            string savedNick = PlayerPrefs.GetString("Nickname");

            if (savedNick == "")
                savedNick = "Player";

            nicknameInput.text = savedNick;

            hostInput.text = "localhost";
        }
    }
}