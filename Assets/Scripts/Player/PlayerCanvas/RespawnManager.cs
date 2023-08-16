using Fusion;
using Player;
using TMPro;
using UnityEngine;
using Utilities;

namespace Gameplay
{
    public class RespawnManager : SimulationBehaviour
    {
        // references
        [SerializeField] private PlayerController player;
        [SerializeField] private GameObject respawnPanel;
        [SerializeField] private TMP_Text respawnTimer;

        public override void FixedUpdateNetwork()
        {
            if (!FusionUtils.IsLocalPlayer(Object)) return;
            var respawnTime = player.PlayerRespawnController.RespawnTimer;
            if (!respawnTime.IsRunning)
            {
                respawnPanel.gameObject.SetActive(false);
                return;
            }
            respawnPanel.SetActive(true);
            respawnTimer.text =
                $"Spawn in: {GameManager.Instance.Player.PlayerRespawnController.RespawnTimerRemaining:F0}";
        }
    }
}
