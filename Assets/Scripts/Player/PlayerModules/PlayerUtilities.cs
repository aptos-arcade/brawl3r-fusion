using Fusion;
using Photon;
using Player.NetworkBehaviours;
using UnityEngine;
using Utilities;

namespace Player.PlayerModules
{
    public class PlayerUtilities
    {
        private readonly PlayerController player;

        public PlayerUtilities(PlayerController player)
        {
            this.player = player;
        }

        public void HandleAir()
        {
            if(player.PlayerProperties.IsGrounded)
            {
                player.PlayerNetworkState.CanDoubleJump = true;
            }
        }

        public void HandleTimers()
        {
            if (player.PlayerNetworkState.DropTimer.IsRunning 
                && player.PlayerNetworkState.DropTimer.Expired(player.Runner))
            {
                player.PlayerNetworkState.DropTimer = TickTimer.None;
            }
            
            if (player.PlayerNetworkState.DodgeCooldown.IsRunning 
                && player.PlayerNetworkState.DodgeCooldown.Expired(player.Runner))
            {
                player.PlayerNetworkState.DodgeCooldown = TickTimer.None;
            }

            if (player.PlayerNetworkState.DodgeTimer.IsRunning 
                && player.PlayerNetworkState.DodgeTimer.Expired(player.Runner))
            {
                player.PlayerNetworkState.DodgeTimer = TickTimer.None;
                player.PlayerComponents.RigidBody.gravityScale = player.PlayerStats.GravityScale;
                player.PlayerNetworkState.IsInvincible = false;
            }
            
            if(player.PlayerNetworkState.ShieldStunTimer.IsRunning 
               && player.PlayerNetworkState.ShieldStunTimer.Expired(player.Runner))
            {
                player.PlayerNetworkState.ShieldStunTimer = TickTimer.None;
            }

            if (player.PlayerNetworkState.HurtTimer.IsRunning 
                && player.PlayerNetworkState.HurtTimer.Expired(player.Runner))
            {
                player.PlayerNetworkState.HurtTimer = TickTimer.None;
            }

            if (player.PlayerNetworkState.InvincibleTimer.IsRunning
                && player.PlayerNetworkState.InvincibleTimer.Expired(player.Runner))
            {
                player.PlayerNetworkState.InvincibleTimer = TickTimer.None;
            }
        }

        public void HandleDeath()
        {
            if (player.PlayerNetworkState.IsDead || (!(Mathf.Abs(player.transform.position.x) > 30) &&
                                              !(Mathf.Abs(player.transform.position.y) > 16))) return;

            // var lostLife = MatchManager.Instance.SessionPlayers[player.Object.InputAuthority].Lives - 1;
            // player.PlayerReferences.PlayerLives.GetChild(lostLife).gameObject.SetActive(false);
            OnDeath();
            

            player.Runner.Spawn(player.PlayerReferences.ExplosionPrefab, player.transform.position, Quaternion.identity);
            MatchManager.Instance.RpcOnPlayerDeath(player.Object.InputAuthority, player.PlayerNetworkState.LastStriker);
            player.PlayerNetworkState.LastStriker = -1;
            player.PlayerNetworkState.ShieldStunTimer = TickTimer.None;
            player.PlayerNetworkState.DamageMultiplier = 1;
            player.PlayerRespawnController.StartRespawn();
        }

        public void HandleEnergy()
        {
            if(player.PlayerNetworkState.MeleeEnergy < 1)
            {
                player.PlayerNetworkState.MeleeEnergy += player.Runner.DeltaTime / player.PlayerStats.MeleeEnergyRegenTime;
            }
            if(player.PlayerNetworkState.RangedEnergy < 1)
            {
                player.PlayerNetworkState.RangedEnergy += player.Runner.DeltaTime / player.PlayerStats.RangedEnergyRegenTime;
            }
            if(player.PlayerNetworkState.ShieldEnergy < 1 && !player.PlayerReferences.PlayerShield.gameObject.activeSelf)
            {
                player.PlayerNetworkState.ShieldEnergy += player.Runner.DeltaTime / player.PlayerStats.ShieldEnergyRegenTime;
            }
        }

        public void ShieldHit(PlayerShield shield)
        {
            player.PlayerNetworkState.ShieldStunTimer = TickTimer.CreateFromSeconds(player.Runner, shield.ShieldStunDuration);
        }

        public void OnRevive()
        {
            player.PlayerNetworkState.IsDead = false;
            player.PlayerNetworkState.MeleeEnergy = 1;
            player.PlayerNetworkState.RangedEnergy = 1;
            player.PlayerNetworkState.IsInvincible = true;
            player.PlayerNetworkState.InvincibleTimer = TickTimer.CreateFromSeconds(player.Runner, 5f);
        }

        private void OnDeath()
        {
            player.PlayerNetworkState.IsDead = true;
            
            player.PlayerAnimations.OnDeath();
            
            if (FusionUtils.IsLocalPlayer(player.Object))
            {
                player.PlayerReferences.PlayerCanvasManager.ShowEnergyUi(false);
            }
        }
        
        public void TriggerInvincibility(bool isInvincible)
        {
            if(player.PlayerNetworkState.IsInvincible == isInvincible) return;
            player.PlayerNetworkState.IsInvincible = isInvincible;
        }
    }
}
