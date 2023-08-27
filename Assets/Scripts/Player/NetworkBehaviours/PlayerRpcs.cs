using Fusion;
using Player.PlayerModules;
using UnityEngine;
using Weapons;

namespace Player.NetworkBehaviours
{
    public class PlayerRpcs : NetworkBehaviour
    {

        private PlayerController player;

        private void Start()
        {
            player = GetComponent<PlayerController>();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RpcStrikerCollision(AttackData attackData, int xScale, PlayerRef striker)
        {
            if (player.PlayerNetworkState.IsInvincible
                || player.PlayerNetworkState.InvincibleTimer.IsRunning) return;
            
            player.PlayerNetworkState.HurtTimer = TickTimer.CreateFromSeconds(player.Runner,
                attackData.stunTime * player.PlayerNetworkState.DamageMultiplier);
            player.PlayerNetworkState.LastStriker = striker;
            player.PlayerNetworkState.DamageMultiplier += attackData.damage;
            player.PlayerNetworkState.IsInvincible = true;
            player.PlayerNetworkState.InvincibleTimer = TickTimer.CreateFromSeconds(player.Runner, 0.2f);
            
            player.PlayerComponents.RigidBody.velocity =
                new Vector2(attackData.knockBackDirection.x * xScale, attackData.knockBackDirection.y).normalized *
                attackData.knockBack * player.PlayerNetworkState.DamageMultiplier;
            
            player.PlayerAnimations.TryStunned();
            
            PlayerCameraController.ShakeCamera(0.25f, new Vector2(5f, 5f));
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RpcShieldCollision(AttackData attackData)
        {
            player.PlayerNetworkState.ShieldEnergy -= attackData.damage;
        }
        
    }
}