using System;
using System.Collections.Generic;
using System.Linq;
using ApiServices;
using ApiServices.Models.CasualMatch;
using ApiServices.Models.RankedMatch;
using Brawler;
using Characters;
using Fusion;
using Gameplay;
using Global;
using Player.PlayerModules;
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
        
        [Networked] 
        public GameState GameState { get; private set; } = GameState.Matchmaking;
        
        [Networked] private NetworkString<_64> MatchId { get; set; }
        
        [Networked(OnChanged = nameof(OnWinnerIndexChanged))] 
        public int WinnerIndex { get; private set; }
        
        // events

        public static event Action OnMatchCreateError;

        public static event Action PlayerListChanged;

        public override void Spawned()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            NetworkRunnerManager.Instance.MatchManager = this;
            RpcAddPlayer(Runner.LocalPlayer,
                new PlayerInfo(BrawlerManager.Instance.Brawler,
                    Runner.LocalPlayer % SessionNumTeams));
        }
        
        // RPCs
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RpcAddPlayer(PlayerRef playerRef, PlayerInfo playerInfo)
        {
            PlayerInfos.Set(playerRef, playerInfo);
        }
        
        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RpcOnPlayerDeath(PlayerRef deadPlayer, PlayerRef lastStriker)
        {
            GameManager.Instance.DeathFeedMessage(deadPlayer, lastStriker);
            if (lastStriker == Runner.LocalPlayer)
            {
                GameManager.Instance.OnKill();
            }

            if(!HasStateAuthority) return;
            
            // update dead player
            var playerInfo = PlayerInfos[deadPlayer];
            playerInfo.Lives--;
            PlayerInfos.Set(deadPlayer, playerInfo);
            
            // update killer
            if (lastStriker == -1) return;
            var strikerInfo = PlayerInfos[lastStriker];
            strikerInfo.Eliminations++;
            PlayerInfos.Set(lastStriker, strikerInfo);
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcOnMatchCreateError()
        {
            OnMatchCreateError?.Invoke();
        }
        
        // change handlers

        private static void OnPlayerInfosChanged(Changed<MatchManager> changed)
        {
            PlayerListChanged?.Invoke();
            switch (changed.Behaviour.GameState)
            {
                case GameState.Matchmaking:
                    changed.Behaviour.TryStartGame();
                    break;
                case GameState.Playing:
                    changed.Behaviour.ScoreCheck();
                    break;
                case GameState.MatchOver:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void OnWinnerIndexChanged(Changed<MatchManager> changed)
        {
            if(changed.Behaviour.GameState == GameState.MatchOver)
            {
                GameManager.Instance.EndGame();
            }
        }
        
        private void TryStartGame()
        {
            if (!HasStateAuthority || SessionMaxPlayers != SessionPlayerCount) return;
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
        
        private IEnumerable<List<string>> TeamsList()
        {
            List<List<string>> teams = new();
            for(var i = 0; i < SessionNumTeams; i++) teams.Add(new List<string>());
            for (var i = 0; i < SessionMaxPlayers; i++)
            {
                teams[SessionPlayers[i].Team].Add(SessionPlayers[i].Id.ToString());
            }
            return teams;
        }
        
        private List<CreateRankedMatchTeam> RankedTeamsList()
        {
            return TeamsList().Select(team => new CreateRankedMatchTeam(team)).ToList();
        }

        private void CreateCasualMatch()
        {
            StartCoroutine(CasualMatchServices.CreateMatch(TeamsList(), OnMatchCreated));
        }

        private void CreateRankedMatch()
        {
            StartCoroutine(RankedMatchServices.CreateMatch(RankedTeamsList(), OnMatchCreated));
        }
        
        private void OnMatchCreated(bool success, string message)
        {
            if (!success)
            {
                RpcOnMatchCreateError();
                return;
            }
            GameState = GameState.Playing;
            MatchId = message;
            LoadGame();
        }

        private void LoadGame()
        {
            Runner.SetActiveScene("GameplayScene");
        }

        private void ScoreCheck()
        {
            if (SessionMaxPlayers == 1 || GameState == GameState.MatchOver) return;
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
                    for(var i = 0; i < SessionMaxPlayers; i++)
                    {
                        casualTeams[SessionPlayers[i].Team]
                            .Add(new CasualMatchPlayer(SessionPlayers[i].Id.ToString(), SessionPlayers[i].Character, SessionPlayers[i].Eliminations));
                    }
                    StartCoroutine(CasualMatchServices.SetMatchResult(MatchId.ToString(), winnerIndex,
                        casualTeams.Select(team => new CasualMatchTeam(team)).ToList(),
                        OnCasualMatchResultReported));
                    break;
                case GameModes.Ranked:
                    var rankedTeams = new List<List<RankedMatchPlayer>>();
                    for (var i = 0; i < SessionNumTeams; i++)
                        rankedTeams.Add(new List<RankedMatchPlayer>());
                    for (var i = 0; i < SessionMaxPlayers; i++)
                    {
                        rankedTeams[SessionPlayers[i].Team]
                            .Add(new RankedMatchPlayer(SessionPlayers[i].Id.ToString(), SessionPlayers[i].Character, SessionPlayers[i].Eliminations));
                    }
                    StartCoroutine(RankedMatchServices.SetMatchResult(MatchId.ToString(), winnerIndex, 
                        rankedTeams.Select(team => new RankedMatchTeam(team)).ToList(), 
                        OnRankedMatchResultReported));
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
        public readonly CharactersEnum Character;
        public readonly Swords Sword;
        public readonly Guns Gun;
        public readonly int Team;
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