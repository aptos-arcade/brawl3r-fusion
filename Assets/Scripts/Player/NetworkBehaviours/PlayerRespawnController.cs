using Fusion;
using Photon;
using Player.PlayerModules;
using UnityEngine;

namespace Player.NetworkBehaviours
{
    public class PlayerRespawnController : NetworkBehaviour
    {
        
        [SerializeField] private PlayerController playerController;
        
        [SerializeField] private float respawnDuration = 5;
        [SerializeField] private float portalDuration = 2.5f;
        
        [Networked] public TickTimer RespawnTimer { get; private set; }
        
        [Networked(OnChanged = nameof(HandlePortalTimerChanged))] 
        private TickTimer PortalTimer { get; set; }
        
        public float RespawnTimerRemaining => RespawnTimer.RemainingTime(Runner) ?? 0;
        
        private NetworkObject portal;

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;
            if (RespawnTimer.IsRunning && RespawnTimer.Expired(Runner))
            {
                StartPortal();
            }
            else if(PortalTimer.IsRunning && PortalTimer.Expired(Runner))
            {
                Respawn();
            }
        }
        
        public void StartRespawn()
        {
            playerController.transform.position =
                SpawnManager.Instance.GetSpawnPoint(playerController.Object.InputAuthority);
            RespawnTimer = TickTimer.CreateFromSeconds(Runner, respawnDuration);
            PortalTimer = TickTimer.None;
        }
        
        private void StartPortal()
        {
            RespawnTimer = TickTimer.None;
            PortalTimer = TickTimer.CreateFromSeconds(Runner, portalDuration);
            portal = Runner.Spawn(playerController.PlayerReferences.Portal, playerController.transform.position, 
                Quaternion.identity, playerController.Object.InputAuthority);
        }

        private void Respawn()
        {
            Runner.Despawn(portal);
            PortalTimer = TickTimer.None;
            playerController.PlayerUtilities.OnRevive();
        }

        private static void HandlePortalTimerChanged(Changed<PlayerRespawnController> changed)
        {
            if (changed.Behaviour.PortalTimer.IsRunning)
            {
                PlayerCameraController.AddPlayer(changed.Behaviour.playerController.transform);
            }
        }
    }
}