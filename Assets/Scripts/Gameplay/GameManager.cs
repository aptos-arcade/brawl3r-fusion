using System;
using System.Collections.Generic;
using System.Linq;
using Com.LuisPedroFonseca.ProCamera2D;
using Fusion;
using Photon;
using Player;
using UnityEngine;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        // singleton pattern
        public static GameManager Instance;
        
        private void Awake()
        {
            Instance = this;
        }
        
        public PlayerController Player { get; set; }
        
        [Header("Game Objects")]
        [SerializeField] private ProCamera2D sceneCamera;
        public ProCamera2D SceneCamera => sceneCamera;
        
        [Header("Managers")]
        [SerializeField] private OutOfLivesManager outOfLivesManager;
        [SerializeField] private FeedManager feedManager;
        [SerializeField] private ConnectedPlayersManager connectedPlayersManager;
        [SerializeField] private KillTextManager killTextManager;

        [Header("UI")]
        [SerializeField] private GameObject outOfLivesUI;
        [SerializeField] private GameObject winUI;
        [SerializeField] private GameObject loseUI;
        
        public void OnEnable()
        {
            UpdateLeaderboard();
            MatchManager.PlayerListChanged += UpdateLeaderboard;
        }
        
        public void OnDisable()
        {
            MatchManager.PlayerListChanged -= UpdateLeaderboard;
        }
        
        public void DeathFeedMessage(PlayerRef actorDeath, PlayerRef actorKill)
        {
            var deathPlayerName = MatchManager.Instance.SessionPlayers[actorDeath].Name.ToString();
            if (actorKill == -1)
            {
                feedManager.WriteMessage(deathPlayerName + " died!", 5f);
            }
            else
            {
                var killPlayerName = MatchManager.Instance.SessionPlayers[actorKill].Name.ToString();
                feedManager.WriteMessage(killPlayerName + " killed " + deathPlayerName + "!", 5f);
            }
        }

        public void OnKill()
        {
            killTextManager.OnKill();
        }

        // handle logic of match manager player infos change
        private void UpdateLeaderboard()
        {
            var playerInfos = MatchManager.Instance.SessionPlayers.Where(a => a.IsActive == true).ToList();
            playerInfos.Sort((a, b) => {
                var eliminationsComparison = b.Eliminations.CompareTo(a.Eliminations);
                return eliminationsComparison != 0 ? eliminationsComparison : b.Lives.CompareTo(a.Lives);
            });
            connectedPlayersManager.ListAllPlayers(playerInfos);
        }

        public void OnPlayerOutOfLives()
        {
            if(NetworkRunnerManager.Instance.MatchManager.GameState == GameState.MatchOver) return;
            outOfLivesUI.SetActive(true);
        }

        public void EndGame()
        {
            if(NetworkRunnerManager.Instance.MatchManager.WinnerIndex == MatchManager.Instance.LocalPlayerInfo.Team)
                OnWin();
            else
                OnLose();
        }
        
        private void OnWin()
        {
            winUI.SetActive(true);
            outOfLivesManager.SetOutOfLivesUI(false);
        }
        
        private void OnLose()
        {
            loseUI.SetActive(true);
            outOfLivesManager.SetOutOfLivesUI(false);
        }

        public static void LeaveGame()
        {
            NetworkRunnerManager.Instance.LeaveRoom();
        }
    }
}