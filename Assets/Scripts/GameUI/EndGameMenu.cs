using Components;
using TMPro;
using UnityEngine;

namespace GameUI
{
    public class EndGameMenu : MonoBehaviour
    {
        [SerializeField] private GameObject menu;
        [SerializeField] private TMP_Text winnerText;

        public void EndGame(uint winnerNetId)
        {
            foreach (var player in FindObjectsOfType<PlayerTable>())
            {
                if (player.netId == winnerNetId)
                {
                    ShowMenu(player.Nickname);
                    break;
                }
            }
        }

        public void HideMenu()
        {
            menu.SetActive(false);
        }

        private void Start()
        {
            menu.SetActive(false);
        }

        private void ShowMenu(string winner)
        {
            winnerText.text = winner;
            menu.SetActive(true);
        }
    }
}