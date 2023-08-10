using System.Collections;
using Fusion;
using Player;
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

        public IEnumerator SpawnCoroutine(PlayerController player)
        {
            if(!Runner.IsServer) yield break;
            var spawnPosition = GetSpawnPoint(player.Object.InputAuthority);
            player.transform.position = spawnPosition;
            var portal = player.Runner.Spawn(
                player.PlayerReferences.Portal,
                spawnPosition,
                Quaternion.identity
            );
            yield return new WaitForSeconds(2.5f);
            player.Runner.Despawn(portal);
            player.PlayerUtilities.DeathRevive(true);
            // if(FusionUtils.IsLocalPlayer(player.Object)) GameManager.Instance.SetEnergyUIActive(true);
            // player.photonView.RPC("OnRevive", RpcTarget.AllBuffered);
            // player.photonView.RPC("TriggerInvincibility", RpcTarget.AllBuffered, true);
            // yield return new WaitForSeconds(5f);
            // if (player.PlayerState.IsInvincible)
            // {
            //     player.photonView.RPC("TriggerInvincibility", RpcTarget.AllBuffered, false);
            // }
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