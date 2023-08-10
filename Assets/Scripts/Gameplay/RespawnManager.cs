using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class RespawnManager : MonoBehaviour
    {
        // references
        [SerializeField] private GameObject respawnPanel;
        [SerializeField] private TMP_Text respawnTimer;

        public void Update()
        {
            var respawnTime = GameManager.Instance.Player.PlayerRespawnController.RespawnTimer;
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
