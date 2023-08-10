using Fusion;
using UnityEngine;

namespace Player
{
    public class PlayerRespawnController : NetworkBehaviour
    {
        
        [SerializeField] private PlayerController playerController;
        
        [SerializeField] private float respawnDuration = 5;
        [SerializeField] private float portalDuration = 2.5f;
        
        [Networked] public TickTimer RespawnTimer { get; private set; }
        [Networked] public TickTimer PortalTimer { get; private set; }
        
        public float RespawnTimerRemaining => RespawnTimer.RemainingTime(Runner) ?? 0;
        
        private NetworkObject portal;

        public override void FixedUpdateNetwork()
        {
            if (RespawnTimer.IsRunning)
            {
                if (RespawnTimer.Expired(Runner))
                {
                    StartPortal();
                }
            }
            else if(PortalTimer.IsRunning)
            {
                if (PortalTimer.Expired(Runner))
                {
                    Respawn();
                }
            }
        }
        
        public void StartRespawn()
        {
            RespawnTimer = TickTimer.CreateFromSeconds(Runner, respawnDuration);
            PortalTimer = TickTimer.None;
        }
        
        private void StartPortal()
        {
            RespawnTimer = TickTimer.None;
            PortalTimer = TickTimer.CreateFromSeconds(Runner, portalDuration);
            if (!Runner.IsServer) return;
            portal = Runner.Spawn(playerController.PlayerReferences.Portal, playerController.transform.position, 
                Quaternion.identity);
        }

        private void Respawn()
        {
            Runner.Despawn(portal);
            PortalTimer = TickTimer.None;
            playerController.PlayerUtilities.DeathRevive(true);
        }
    }
}