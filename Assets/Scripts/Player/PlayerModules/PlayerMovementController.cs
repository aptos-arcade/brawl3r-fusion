using System;
using Fusion;
using UnityEngine;

namespace Player.PlayerModules
{
    public class PlayerMovementController
    {
        private readonly PlayerController player;
        
        public PlayerMovementController(PlayerController player)
        {
            this.player = player;
            player.PlayerComponents.RigidBody.gravityScale = player.PlayerStats.GravityScale;
        }
        
        public void Move()
        {
            if (player.PlayerProperties.IsStunned) return;

            float targetSpeed;
            if (player.PlayerProperties.IsDashing)
            {
                targetSpeed = player.PlayerNetworkState.Direction.x * player.PlayerStats.DashVelocity;
            }
            else if (player.PlayerProperties.IsDodging)
            {
                targetSpeed = player.PlayerNetworkState.Direction.x * player.PlayerStats.DodgeVelocity;
            }
            else if (player.PlayerProperties.IsShielding)
            {
                targetSpeed = 0;
            }
            else
            {
                targetSpeed = player.PlayerNetworkState.Direction.x * player.PlayerStats.Speed;
            }

            var speedDiff = targetSpeed - player.PlayerComponents.RigidBody.velocity.x;
            var accelerationRate = Mathf.Abs(targetSpeed) > 0.01f || Math.Abs(Mathf.Sign(targetSpeed) - Mathf.Sign(player.PlayerComponents.RigidBody.velocity.x)) < 0.1
                ? player.PlayerStats.Acceleration 
                : player.PlayerStats.Deceleration;
            var movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, player.PlayerStats.VelocityPower) * Mathf.Sign(speedDiff);
            player.PlayerComponents.RigidBody.AddForce(movement * Vector2.right);
        }
        
        public void Jump(bool isDoubleJump)
        {
            if (isDoubleJump)
            {
                player.PlayerNetworkState.CanDoubleJump = false;
            }
            var jumpForce = isDoubleJump ? player.PlayerStats.DoubleJumpForce : player.PlayerStats.JumpForce;
            player.PlayerComponents.RigidBody.velocity = new Vector2(player.PlayerComponents.RigidBody.velocity.x, 0);
            player.PlayerComponents.RigidBody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Force);
            player.PlayerAudioController.PlayOneShotAudio(player.PlayerReferences.JumpAudioClip);
        }
        
        public void Drop()
        {
            player.PlayerNetworkState.DropTimer = TickTimer.CreateFromSeconds(player.Runner, 0.25f);
        }
        
        public void Dash()
        {
            player.PlayerAudioController.PlayOneShotAudio(player.PlayerReferences.DashAudioClip);
        }
        
        public void FastFall()
        {
            player.PlayerComponents.RigidBody.velocity = new Vector2(player.PlayerComponents.RigidBody.velocity.x, 0);
            player.PlayerComponents.RigidBody.AddForce(Vector2.down * player.PlayerStats.FastFallForce);
        }
        
        public void Dodge()
        {
            player.PlayerAudioController.PlayOneShotAudio(player.PlayerReferences.DodgeAudioClip);
            player.PlayerNetworkState.DodgeCooldown = TickTimer.CreateFromSeconds(player.Runner, 1f);
            player.PlayerNetworkState.DodgeTimer = TickTimer.CreateFromSeconds(player.Runner, 0.35f);
            player.PlayerComponents.RigidBody.velocity = Vector2.zero;
            player.PlayerComponents.RigidBody.gravityScale = 0;
            player.PlayerNetworkState.IsInvincible = true;
        }
    }
}