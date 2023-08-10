using System;
using System.Collections.Generic;
using System.Linq;
using ApiServices;
using ApiServices.Models.CasualMatch;
using ApiServices.Models.RankedMatch;
using Brawler;
using Characters;
using Fusion;
using Global;
using UnityEngine;

namespace Photon
{
    public class MatchManager : NetworkBehaviour
    {
        // singleton
        
        public static MatchManager Instance { get; private set; }
        
        // session properties
        
        public PlayerInfo LocalPlayerInfo => SessionPlayers[Runner.LocalPlayer];

        public int SessionNumTeams => Runner.SessionInfo.Properties[NetworkRunnerManager.NumTeamsPropKey];

        public GameModes SessionGameMode =>
            (GameModes)(int)Runner.SessionInfo.Properties[NetworkRunnerManager.ModePropKey];

        public int SessionMaxPlayers => Runner.SessionInfo.MaxPlayers;

        public int SessionPlayerCount => Runner.SessionInfo.PlayerCount;

        public PlayerInfo[] SessionPlayers => PlayerInfos.ToArray();

        // networked properties

        [Networked(OnChanged = nameof(OnPlayerInfosChanged))]
        [Capacity(8)]
        public NetworkArray<PlayerInfo> PlayerInfos { get; }
        
        [Networked] public string MatchId { get; set; }
        
        [Networked] public GameState GameState { get; set; } = GameState.Matchmaking;
        
        [Networked] public int WinnerIndex { get; set; } = -1;
        
        // events

        public static event Action OnMatchCreateError;

        public static event Action PlayerListChanged;

        public override void Spawned()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            NetworkRunnerManager.Instance.MatchManager = this;
            Runner.SetActiveScene("MatchmakingScene");
            RpcAddPlayer(Runner.LocalPlayer,
                new PlayerInfo(BrawlerManager.Instance.Brawler,
                    Runner.LocalPlayer % SessionNumTeams));
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RpcAddPlayer(PlayerRef playerRef, PlayerInfo playerInfo)
        {
            Debug.Log($"Adding player {playerRef} with name {playerInfo.Name}");
            PlayerInfos.Set(playerRef, playerInfo);
        }
        
        private static void OnPlayerInfosChanged(Changed<MatchManager> changed)
        {
            PlayerListChanged?.Invoke();
            if (changed.Behaviour.GameState == GameState.Matchmaking)
            {
                changed.Behaviour.TryStartGame();
            }
        }
        
        private void TryStartGame()
        {
            if (!Runner.IsServer || SessionMaxPlayers != SessionPlayerCount) return;
            Runner.SessionInfo.IsOpen = false;
            switch (SessionGameMode)
            {
                case GameModes.Casual:
                    CreateCasualMatch();
                    break;
                case GameModes.Ranked:
                    CreateRankedMatch();
                    break;
                case GameModes.Training:
                case GameModes.None:
                default:
                    LoadGame();
                    break;
            }
        }
        
        private List<List<string>> TeamsList()
        {
            List<List<string>> teams = new();
            for(var i = 0; i < SessionNumTeams; i++) teams.Add(new List<string>());
            for (var i = 0; i < SessionMaxPlayers; i++)
            {
                teams[SessionPlayers[i].Team].Add(SessionPlayers[i].Id.ToString());
            }
            return teams;
        }

        private void CreateCasualMatch()
        {
            StartCoroutine(CasualMatchServices.CreateMatch(TeamsList(), OnMatchCreated));
        }

        private void CreateRankedMatch()
        {
            StartCoroutine(RankedMatchServices.CreateMatch(TeamsList(), OnMatchCreated));
        }
        
        private void OnMatchCreated(bool success, string message)
        {
            if (!success)
            {
                Debug.Log("Failed to create match");
                OnMatchCreateError?.Invoke();
                return;
            }
            Debug.Log($"Match created with id {message}");
            MatchId = message;
            LoadGame();
        }

        private void LoadGame()
        {
            Runner.SetActiveScene("GameplayScene");
        }

        

        

        private void ScoreCheck()
        {
            if (SessionMaxPlayers == 1 || !NetworkRunnerManager.Instance.NetworkRunner.IsServer ||
                GameState == GameState.MatchOver) return;
            // check if there is only one team left that has players with more than one life
            var winnerFound = false;
            var curWinningTeam = -1;
            foreach (var playerInfo in SessionPlayers.ToList().Where(playerInfo => playerInfo.Lives > 0 && playerInfo.IsActive))
            {
                if (curWinningTeam == -1 || curWinningTeam == playerInfo.Team)
                {
                    curWinningTeam = playerInfo.Team;
                    winnerFound = true;
                    continue;
                }
                winnerFound = false;
                break;
            }
            if (!winnerFound) return;
            OnWinnerFound(curWinningTeam);
        }
        
        private void OnWinnerFound(int winnerIndex)
        {
            GameState = GameState.MatchOver;
            WinnerIndex = winnerIndex;
            switch (SessionGameMode)
            {
                case GameModes.Casual:
                    var casualTeams = new List<List<CasualMatchPlayer>>();
                    for (var i = 0; i < SessionNumTeams; i++)
                        casualTeams.Add(new List<CasualMatchPlayer>());
                    foreach (var playerInfo in SessionPlayers)
                    {
                        casualTeams[playerInfo.Team]
                            .Add(new CasualMatchPlayer(playerInfo.Id.ToString(), playerInfo.Character, playerInfo.Eliminations));
                    }

                    StartCoroutine(CasualMatchServices.SetMatchResult(MatchId, winnerIndex, casualTeams,
                        OnCasualMatchResultReported));
                    break;
                case GameModes.Ranked:
                    var rankedTeams = new List<List<RankedMatchPlayer>>();
                    for (var i = 0; i < SessionNumTeams; i++)
                        rankedTeams.Add(new List<RankedMatchPlayer>());
                    foreach (var playerInfo in SessionPlayers)
                    {
                        rankedTeams[playerInfo.Team]
                            .Add(new RankedMatchPlayer(playerInfo.Id.ToString(), playerInfo.Character, playerInfo.Eliminations));
                    }
                    StartCoroutine(RankedMatchServices.SetMatchResult(MatchId, winnerIndex, rankedTeams, OnRankedMatchResultReported));
                    break;
                case GameModes.Training:
                case GameModes.None:
                default:
                    break;
            }
        }
        
        private static void OnCasualMatchResultReported(bool success, string message)
        {
            Debug.Log(message);
        }

        private static void OnRankedMatchResultReported(bool success, string message)
        {
            Debug.Log(message);
        }
    }
    
    public struct PlayerInfo: INetworkStruct
    {
        public NetworkString<_16> Name;
        public NetworkString<_64> Id;
        public CharactersEnum Character;
        public Swords Sword;
        public Guns Gun;
        public int Team;
        public int Lives;
        public int Eliminations;
        public NetworkBool IsActive;

        public PlayerInfo(Brawler.Brawler brawler, int team)
        {
            Name = brawler.Name;
            Id = brawler.Id;
            Character = brawler.Character;
            Sword = brawler.Sword;
            Gun = brawler.Gun;
            Team = team;
            Lives = 3;
            Eliminations = 0;
            IsActive = true;
        }
    }
    
    public enum GameState
    {
        Matchmaking,
        Playing,
        MatchOver
    }
}