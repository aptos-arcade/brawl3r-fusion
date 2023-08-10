using System;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class FeedManager : MonoBehaviour
    {
    
        [SerializeField] private TMP_Text feedTextPrefab;

        private void OnEnable()
        {
            
        }

        public void OnPlayerLeftRoom(string playerName)
        {
            WriteMessage(playerName + " has left the game", 3f);
        }
    
        public void WriteMessage(string message, float destroyTime)
        {
            var playerText = Instantiate(feedTextPrefab, transform);
            playerText.text = message;
            Destroy(playerText.gameObject, destroyTime);
        }
    }
}
