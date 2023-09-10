using System.Collections.Generic;
using Photon;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MatchmakingManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomName;
        [SerializeField] private TMP_Text waitingText;
        [SerializeField] private CharacterImages characterImages;
        [SerializeField] private GameObject playersList;
        [SerializeField] private RoomPlayer roomPlayerPrefab;
        [SerializeField] private Button backButton;
    
        private readonly List<RoomPlayer> allPlayers = new();

        private void Start()
        {
            backButton.onClick.AddListener(NetworkRunnerManager.Instance.LeaveRoom);
            MatchManager.PlayerListChanged += ListAllPlayers;
            MatchManager.OnMatchCreateError += OnMatchCreateError;
        }

        private void OnDestroy()
        {
            MatchManager.PlayerListChanged -= ListAllPlayers;
            MatchManager.OnMatchCreateError -= OnMatchCreateError;
        }

        private static string RoomTitle(int numTeams, int numPlayers)
        {
            var roomTitle = "";
            for(var i = 0; i < numTeams; i++)
            {
                roomTitle += $"{numPlayers / numPlayers}";
                if(i < numTeams - 1)
                {
                    roomTitle += "v";
                }
            }
            return roomTitle;
        }

        private void ListAllPlayers()
        {
            roomName.text = RoomTitle(MatchManager.Instance.SessionNumTeams,
                MatchManager.Instance.SessionMaxPlayers);
            
            foreach (var roomPlayer in allPlayers)
            {
                Destroy(roomPlayer.gameObject);
            }
            allPlayers.Clear();
            
            for(var i = 0; i < MatchManager.Instance.SessionMaxPlayers; i++)
            {
                var player = MatchManager.Instance.SessionPlayers[i];
                if(!player.IsActive) continue;
                Debug.Log(player.Name);
                var roomPlayer = Instantiate(roomPlayerPrefab, playersList.transform);
                roomPlayer.SetPlayerInfo(player.Name.ToString(),
                    characterImages.GetCharacterSprite((int)player.Character));
                allPlayers.Add(roomPlayer);
            }
            SetWaitingText();
        }

        private void OnMatchCreateError()
        {
            waitingText.text = "Error creating match";
            Debug.Log(waitingText.text);
            backButton.interactable = true;
            Debug.Log(backButton.interactable);
        }

        private void SetWaitingText()
        {
            Debug.Log("Set waiting text");
            var remainingPlayers = MatchManager.Instance.SessionMaxPlayers -
                                   MatchManager.Instance.SessionPlayerCount;
            if (remainingPlayers == 0)
            {
                waitingText.text = "Loading the game...";
                backButton.interactable = false;
            }
            else
            {
                var suffix = remainingPlayers == 1 ? "" : "s";
                waitingText.text =
                    $"Waiting for {remainingPlayers} player{suffix}...";
            }
            Debug.Log("Finished waiting text");
        }
    }
}
