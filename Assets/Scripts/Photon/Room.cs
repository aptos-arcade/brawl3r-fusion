using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace Photon
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private byte numberOfPlayers;
        [SerializeField] private Global.GameModes gameMode;
        [SerializeField] private int numTeams;

        private Button joinButton;
        
        private void Start()
        {
            joinButton = GetComponent<Button>();
            joinButton.onClick.AddListener(JoinRoom);
        }

        private void JoinRoom()
        {
            NetworkRunnerManager.Instance.JoinRoom(HandleJoin, GameMode.Shared, gameMode, numTeams,
                numberOfPlayers);
        }
        
        private static void HandleJoin(bool success)
        {
            if (success) return;
            Debug.LogError("Failed to join room");
        }
    }
}
