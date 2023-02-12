using Components;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class PlayerScoreManager : NetworkBehaviour
    {
        public UnityEvent<uint> OnWin = new UnityEvent<uint>();
        public UnityEvent<uint, int> OnScoreChange = new UnityEvent<uint, int>();
        [SerializeField] private int winScore = 3;

        private readonly SyncDictionary<uint, int> _scores = new SyncDictionary<uint, int>();
        private bool _isEnd = false;

        public override void OnStartClient()
        {
            _scores.Callback += (op, key, item) =>
            {
                if (op is SyncIDictionary<uint, int>.Operation.OP_CLEAR ||
                    op is SyncIDictionary<uint, int>.Operation.OP_REMOVE)
                    return;

                if (item >= winScore)
                    OnWin.Invoke(key);

                OnScoreChange.Invoke(key, _scores[key]);
            };
        }

        [Server]
        public void AddPlayer(Damagable damagable)
        {
            uint netId = damagable.netId;

            if (_scores.ContainsKey(netId))
                return;
            
            _scores[netId] = 0;
            damagable.OnDamage.AddListener(IncreaseScore);
        }
        
        [Server]
        private void IncreaseScore(uint netId)
        {
            if (_isEnd)
                return;
            
            if (++_scores[netId] >= winScore)
            {
                _isEnd = true;
                OnWin.Invoke(netId);
            }
        }
    }
}