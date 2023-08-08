using System;
using System.Collections.Generic;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon
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
            roomName.text = RoomTitle(NetworkRunnerManager.Instance.SessionNumTeams,
                NetworkRunnerManager.Instance.SessionMaxPlayers);
            backButton.onClick.AddListener(NetworkRunnerManager.Instance.LeaveRoom);
        }

        private void OnEnable()
        {
            MatchManager.PlayerListChanged += ListAllPlayers;
            NetworkRunnerManager.Instance.OnMatchCreateError += OnMatchCreateError;
        }
        
        private void OnDisable()
        {
            MatchManager.PlayerListChanged -= ListAllPlayers;
            NetworkRunnerManager.Instance.OnMatchCreateError -= OnMatchCreateError;
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
            foreach (var roomPlayer in allPlayers)
            {
                Destroy(roomPlayer.gameObject);
            }
            allPlayers.Clear();
            
            for(var i = 0; i < NetworkRunnerManager.Instance.SessionMaxPlayers; i++)
            {
                var player = NetworkRunnerManager.Instance.SessionPlayers[i];
                if(player.Name.ToString() == string.Empty) continue;
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
            var remainingPlayers = NetworkRunnerManager.Instance.SessionMaxPlayers -
                                   NetworkRunnerManager.Instance.SessionPlayerCount;
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
