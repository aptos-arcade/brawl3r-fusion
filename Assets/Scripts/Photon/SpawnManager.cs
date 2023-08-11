using Fusion;
using UnityEngine;

namespace Photon
{
    public class SpawnManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [SerializeField] private NetworkPrefabRef playerPrefab;
        [SerializeField] private Transform[] spawnPoints;

        public override void Spawned()
        {
            if(!Runner.IsServer) return;
            foreach (var player in Runner.ActivePlayers)
            {
                SpawnPlayer(player);
            }
        }

        private Vector2 GetSpawnPoint(PlayerRef player)
        {
            return spawnPoints[MatchManager.Instance.SessionPlayers[player].Team].position;
        }

        private void SpawnPlayer(PlayerRef player)
        {
            if(!Runner.IsServer) return;
            var spawnPoint = GetSpawnPoint(player);
            var playerObject = Runner.Spawn(playerPrefab, spawnPoint, Quaternion.identity, player);
            Runner.SetPlayerObject(player, playerObject);
        }
        
        private void DespawnPlayer(PlayerRef player)
        {
            if(!Runner.IsServer) return;
            if(!Runner.TryGetPlayerObject(player, out var playerObject)) return;
            Runner.Despawn(playerObject);
            Runner.SetPlayerObject(player, null);
        }

        public void PlayerJoined(PlayerRef player)
        {
            SpawnPlayer(player);
        }

        public void PlayerLeft(PlayerRef player)
        {
            DespawnPlayer(player);
        }
    }
}