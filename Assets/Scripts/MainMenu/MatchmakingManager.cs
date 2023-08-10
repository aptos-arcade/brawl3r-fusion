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
            if(NetworkRunnerManager.Instance.NetworkRunner.IsServer)
                ListAllPlayers();
        }

        private void OnEnable()
        {
            MatchManager.PlayerListChanged += ListAllPlayers;
            MatchManager.OnMatchCreateError += OnMatchCreateError;
        }
        
        private void OnDisable()
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
                AddPlayer(player);
            }

            SetWaitingText();
        }

        private void AddPlayer(PlayerInfo playerInfo)
        {
            var roomPlayer = Instantiate(roomPlayerPrefab, playersList.transform);
            roomPlayer.SetPlayerInfo(playerInfo.Name.ToString(),
                characterImages.GetCharacterSprite((int)playerInfo.Character));
            allPlayers.Add(roomPlayer);
            SetWaitingText();
        }

        private void OnMatchCreateError()
        {
            waitingText.text = "Error creating match";
        }

        private void SetWaitingText()
        {
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
        }
    }
}
