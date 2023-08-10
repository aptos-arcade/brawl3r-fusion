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
        [SerializeField] private RespawnManager respawnManager;
        [SerializeField] private OutOfLivesManager outOfLivesManager;
        [SerializeField] private FeedManager feedManager;
        [SerializeField] private KillTextManager killTextManager;
        [SerializeField] private ConnectedPlayersManager connectedPlayersManager;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private EnergyManager energyManager;

        [Header("UI")]
        [SerializeField] private GameObject energyUI;
        [SerializeField] private GameObject outOfLivesUI;
        [SerializeField] private GameObject winUI;
        [SerializeField] private GameObject loseUI;

        // handle logic of match manager player infos change
        private void UpdateLeaderboard()
        {
            var playerInfos = MatchManager.Instance.SessionPlayers.ToList();
            playerInfos.Sort((a, b) =>
            {
                var eliminationsComparison = b.Eliminations.CompareTo(a.Eliminations);
                return eliminationsComparison != 0 ? eliminationsComparison : b.Lives.CompareTo(a.Lives);
            });
            connectedPlayersManager.ListAllPlayers(playerInfos);
        }

        private void PlayerDeathReceive(IReadOnlyList<object> data)
        {
            // var actorDeath = (int)data[0];
            // var actorKill = (int)data[1];
            //
            // DeathFeedMessage(actorDeath, actorKill);
            //
            // var killPlayerIndex = PlayerInfos.FindIndex(x => x.ActorNumber == actorKill);
            // if (killPlayerIndex >= 0 && killPlayerIndex < PlayerInfos.Count) PlayerInfos[killPlayerIndex].Eliminations++;
            // if (actorKill == PhotonNetwork.LocalPlayer.ActorNumber) killTextManager.OnKill();
            //
            // var deathPlayerIndex = PlayerInfos.FindIndex(x => x.ActorNumber == actorDeath);
            // if (deathPlayerIndex >= 0 && deathPlayerIndex < PlayerInfos.Count) PlayerInfos[deathPlayerIndex].Lives--;
            // if (actorDeath == PhotonNetwork.LocalPlayer.ActorNumber)
            // {
            //     if(PlayerInfos[deathPlayerIndex].Lives > 0)
            //         OnPlayerDeath();
            //     else
            //         OnPlayerOutOfLives();
            // }
            // ScoreCheck();
            // ListPlayersSend();
        }

        public void SpawnPlayer(PlayerController player)
        {
            StartCoroutine(spawnManager.SpawnCoroutine(player));
        }

        private void DeathFeedMessage(PlayerRef actorDeath, PlayerRef actorKill)
        {
            var deathPlayerName = MatchManager.Instance.SessionPlayers[actorDeath].Name.ToString();
            if (actorKill == 0)
            {
                feedManager.WriteMessage(deathPlayerName + " died!", 5f);
            }
            else
            {
                var killPlayerName = MatchManager.Instance.SessionPlayers[actorDeath].Name.ToString();
                feedManager.WriteMessage(killPlayerName + " killed " + deathPlayerName + "!", 5f);
            }
        }

        private void OnPlayerDeath()
        {
            respawnManager.StartRespawn();
        }

        private void OnPlayerOutOfLives()
        {
            if(NetworkRunnerManager.Instance.MatchManager.GameState == GameState.MatchOver) return;
            outOfLivesUI.SetActive(true);
        }
        
        public void SetEnergyUIActive(bool active)
        {
            energyUI.SetActive(active);
        }

        public void NoEnergy(EnergyManager.EnergyType energyType)
        {
            energyManager.NoEnergy(energyType);
        }

        private void EndGame()
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
    }
}