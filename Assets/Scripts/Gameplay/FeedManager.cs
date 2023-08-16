using Fusion;
using Photon;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class FeedManager : MonoBehaviour
    {
    
        [SerializeField] private TMP_Text feedTextPrefab;

        public void OnPlayerLeftRoom(string playerName)
        {
            WriteMessage(playerName + " has left the game", 3f);
        }
        
        public void OnDeath(PlayerRef playerDeath, PlayerRef playerKill)
        {
            var deathPlayerName = MatchManager.Instance.SessionPlayers[playerDeath].Name.ToString();
            if (playerKill == -1)
            {
                WriteMessage(deathPlayerName + " died!", 5f);
            }
            else
            {
                var killPlayerName = MatchManager.Instance.SessionPlayers[playerKill].Name.ToString();
                WriteMessage(killPlayerName + " killed " + deathPlayerName + "!", 5f);
            }
        }
    
        public void WriteMessage(string message, float destroyTime)
        {
            var playerText = Instantiate(feedTextPrefab, transform);
            playerText.text = message;
            Destroy(playerText.gameObject, destroyTime);
        }
    }
}
