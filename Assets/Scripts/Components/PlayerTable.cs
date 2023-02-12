using Managers;
using Mirror;
using UnityEngine;

namespace Components
{
    public class PlayerTable : NetworkBehaviour
    {
        [SerializeField] private TextMesh textArea;
        
        [SyncVar(hook = nameof(UpdateNickname))] private string _nickname;
        private Camera _camera;
        private int _score = 0;

        public string Nickname => _nickname;

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                SetNickname(PlayerPrefs.GetString("Nickname"));
                textArea.gameObject.SetActive(false);
            }
            
            _camera = Camera.main;
            
            FindObjectOfType<PlayerScoreManager>().OnScoreChange.AddListener(UpdateScore);
        }
        
        public void UpdateScore(uint netId, int score)
        {
            if (netId != this.netId)
                return;

            _score = score;
            
            textArea.text = $"{_nickname} | {_score}";
        }

        [Client]
        private void Update()
        {
            textArea.transform.LookAt(_camera.transform);
            textArea.transform.Rotate(0, 180, 0);
        }

        [Command]
        private void SetNickname(string nickname)
        {
            if (nickname == "")
                nickname = $"Player {netId}";
            
            _nickname = nickname;
        }

        [Client]
        private void UpdateNickname(string oldValue, string newValue)
        {
            textArea.text = $"{newValue} | {_score}";
        }
    }
}