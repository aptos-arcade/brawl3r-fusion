using System;
using Brawler;
using Characters;
using Fusion;

namespace Photon
{
    public class MatchManager : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(OnPlayerInfosChanged))]
        [Capacity(8)]
        public NetworkArray<PlayerInfo> PlayerInfos { get; }
        
        public string MatchId { get; set; }
        
        // events

        public static event Action PlayerListChanged;

        public override void Spawned()
        {
            DontDestroyOnLoad(gameObject);
            NetworkRunnerManager.Instance.MatchManager = this;
            Runner.SetActiveScene("MatchmakingScene");
            RpcAddPlayer(Runner.LocalPlayer,
                new PlayerInfo(BrawlerManager.Instance.Brawler,
                    Runner.LocalPlayer % NetworkRunnerManager.Instance.SessionNumTeams));
           
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RpcAddPlayer(PlayerRef playerRef, PlayerInfo playerInfo)
        {
            PlayerInfos.Set(playerRef, playerInfo);
        }

        private static void OnPlayerInfosChanged(Changed<MatchManager> changed)
        {
            PlayerListChanged?.Invoke();
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
        public int Kills;

        public PlayerInfo(Brawler.Brawler brawler, int team)
        {
            Name = brawler.Name;
            Id = brawler.Id;
            Character = brawler.Character;
            Sword = brawler.Sword;
            Gun = brawler.Gun;
            Team = team;
            Lives = 3;
            Kills = 0;
        }
    }
}