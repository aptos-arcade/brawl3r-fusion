using Fusion;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class RespawnManager : NetworkBehaviour
    {
        // references
        [SerializeField] private GameObject respawnPanel;
        [SerializeField] private TMP_Text respawnTimer;
        [SerializeField] private float respawnDuration = 5;
    
        // state
        [Networked] private NetworkBool IsRespawning { get; set; }
        [Networked] private TickTimer RespawnTimer { get; set; }

        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            if (!IsRespawning || !RespawnTimer.RemainingTime(Runner).HasValue) return;
            respawnTimer.text = "Spawn in: " + RespawnTimer.RemainingTime(Runner)?.ToString("F0");
            if (!RespawnTimer.Expired(Runner)) return;
            Respawn();
        }

        public void StartRespawn()
        {
            respawnPanel.gameObject.SetActive(true);
            RespawnTimer = TickTimer.CreateFromSeconds(Runner, respawnDuration);
            IsRespawning = true;
        }

        private void Respawn()
        {
            respawnPanel.gameObject.SetActive(false);
            IsRespawning = false;
            RespawnTimer = TickTimer.None;
            GameManager.Instance.SpawnPlayer(GameManager.Instance.Player);
        }
    }
}
