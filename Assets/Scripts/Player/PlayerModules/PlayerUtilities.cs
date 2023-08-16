using Fusion;
using Photon;
using Player.NetworkBehaviours;
using UnityEngine;
using Utilities;
using Weapons;

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
            if (player.PlayerNetworkState.DropTimer.IsRunning)
            {
                if (player.PlayerNetworkState.DropTimer.Expired(player.Runner))
                {
                    player.PlayerNetworkState.DropTimer = TickTimer.None;
                }
            }
            
            if (player.PlayerNetworkState.DodgeCooldown.IsRunning)
            {
                if (player.PlayerNetworkState.DodgeCooldown.Expired(player.Runner))
                {
                    player.PlayerNetworkState.DodgeCooldown = TickTimer.None;
                }
            }

            if (player.PlayerNetworkState.DodgeTimer.IsRunning)
            {
                if(player.PlayerNetworkState.DodgeTimer.Expired(player.Runner))
                {
                    player.PlayerNetworkState.DodgeTimer = TickTimer.None;
                    player.PlayerComponents.RigidBody.gravityScale = player.PlayerStats.GravityScale;
                    player.PlayerNetworkState.IsInvincible = false;
                }
            }
            
            if(player.PlayerNetworkState.StunTimer.IsRunning)
            {
                if(player.PlayerNetworkState.StunTimer.Expired(player.Runner))
                {
                    player.PlayerNetworkState.StunTimer = TickTimer.None;
                }
            }
        }

        public void HandleDeath()
        {
            if (player.PlayerNetworkState.IsDead || (!(Mathf.Abs(player.transform.position.x) > 30) &&
                                              !(Mathf.Abs(player.transform.position.y) > 16))) return;

            var lostLife = MatchManager.Instance.SessionPlayers[player.Object.InputAuthority].Lives - 1;
            player.PlayerReferences.PlayerLives.GetChild(lostLife).gameObject.SetActive(false);
            player.PlayerUtilities.DeathRevive(false);
            player.PlayerAnimations.OnDeath();

            if (!player.Runner.IsServer) return;
            player.Runner.Spawn(player.PlayerReferences.ExplosionPrefab, player.transform.position, Quaternion.identity);
            MatchManager.Instance.OnPlayerDeath(player);
            player.PlayerNetworkState.LastStriker = -1;
            player.PlayerNetworkState.StunTimer = TickTimer.None;
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

        public void StrikerCollision(Striker striker)
        {
            player.PlayerNetworkState.HurtTimer = TickTimer.CreateFromSeconds(player.Runner,
                striker.strikerData.StunTime * player.PlayerNetworkState.DamageMultiplier);
            player.PlayerAnimations.TryStunned();
            PlayerCameraController.ShakeCamera(0.25f, new Vector2(5f, 5f));
            player.PlayerNetworkState.LastStriker = striker.Object.InputAuthority;
            player.PlayerNetworkState.DamageMultiplier += striker.strikerData.Damage;
            player.PlayerComponents.RigidBody.velocity = striker.KnockBackSignedDirection.normalized *
                                                         striker.strikerData.KnockBack *
                                                         player.PlayerNetworkState.DamageMultiplier;
        }

        public void ShieldCollision(Striker striker)
        {
            player.PlayerNetworkState.ShieldEnergy -= striker.strikerData.Damage;
        }

        public void ShieldHit(PlayerShield shield)
        {
            player.PlayerNetworkState.StunTimer = TickTimer.CreateFromSeconds(player.Runner, shield.ShieldStunDuration);
        }

        public void DeathRevive(bool isRevive)
        {
            player.PlayerNetworkState.IsDead = !isRevive;
            player.PlayerNetworkState.Direction = Vector2.zero;
            player.PlayerNetworkState.MeleeEnergy = isRevive ? 1 : 0;
            player.PlayerNetworkState.RangedEnergy = isRevive ? 1 : 0;
            
            player.PlayerComponents.RigidBody.velocity = Vector2.zero;
            player.PlayerComponents.RigidBody.simulated = isRevive;

            if (!isRevive)
            {
                PlayerCameraController.RemovePlayer(player.transform);
            }

            if (FusionUtils.IsLocalPlayer(player.Object))
            {
                player.PlayerReferences.PlayerCanvasManager.ShowEnergyUi(isRevive);
            }
            
            player.PlayerReferences.PlayerObject.SetActive(isRevive);
        }

        public static bool IsSameTeam(NetworkObject other)
        {
            return MatchManager.Instance.SessionPlayers[other.InputAuthority].Team == 
                   MatchManager.Instance.LocalPlayerInfo.Team;
        }
        
        public void TriggerInvincibility(bool isInvincible)
        {
            if(player.PlayerNetworkState.IsInvincible == isInvincible) return;
            player.PlayerNetworkState.IsInvincible = isInvincible;
        }
    }
}
