using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerActions
    {

        private readonly PlayerController player;
        
        public PlayerActions(PlayerController player)
        {
            this.player = player;
        }

        public void Move()
        {
            if (player.PlayerUtilities.IsStunned)
            {
                player.PlayerComponents.RunAudioSource.Stop();
                return;
            }
            
            if(player.PlayerState.Direction.x != 0)
            {
                var direction = player.PlayerState.Direction.x < 0 ? -1 : 1;
                player.transform.localScale = new Vector3(direction, 1, 1);
                player.PlayerReferences.PlayerCanvas.transform.localScale = new Vector3(direction, 1, 1);
                if (player.PlayerUtilities.IsGrounded)
                {
                    player.PlayerAnimations.TryPlayAnimation(Animations.Animations.BodyWalk, Animations.Animations.LegsWalk);
                }
                if (!player.PlayerComponents.RunAudioSource.isPlaying)
                {
                    player.PlayerComponents.RunAudioSource.Play();
                }
            }
            else if(player.PlayerComponents.RigidBody.velocity.magnitude < 0.1f && player.PlayerUtilities.IsGrounded)
            {
                player.PlayerAnimations.TryPlayAnimation(Animations.Animations.BodyIdle, Animations.Animations.LegsIdle);
            }
            
            if(!player.PlayerUtilities.IsGrounded || player.PlayerComponents.RigidBody.velocity.magnitude < 0.1f || 
               player.PlayerUtilities.IsDashing || player.PlayerUtilities.IsDodging)
            {
                player.PlayerComponents.RunAudioSource.Stop();
            }

            float targetSpeed;
            if (player.PlayerUtilities.IsDashing)
            {
                targetSpeed = player.PlayerState.Direction.x * player.PlayerStats.DashVelocity;
            }
            else if (player.PlayerUtilities.IsDodging)
            {
                targetSpeed = player.PlayerState.Direction.x * player.PlayerStats.DodgeVelocity;
            }
            else if (player.PlayerUtilities.IsShielding)
            {
                targetSpeed = 0;
            }
            else
            {
                targetSpeed = player.PlayerState.Direction.x * player.PlayerStats.Speed;
            }

            var speedDiff = targetSpeed - player.PlayerComponents.RigidBody.velocity.x;
            var accelerationRate = Mathf.Abs(targetSpeed) > 0.01f || Math.Abs(Mathf.Sign(targetSpeed) - Mathf.Sign(player.PlayerComponents.RigidBody.velocity.x)) < 0.1
                ? player.PlayerStats.Acceleration 
                : player.PlayerStats.Deceleration;
            var movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, player.PlayerStats.VelocityPower) * Mathf.Sign(speedDiff);
            player.PlayerComponents.RigidBody.AddForce(movement * Vector2.right);
        }

        public void TryJump()
        {
            if (player.PlayerUtilities.IsGrounded)
            {
                player.PlayerAnimations.TryPlayAnimation(Animations.Animations.BodyJump, Animations.Animations.LegsJump);
            }
            else if(player.PlayerState.CanDoubleJump)
            {
                player.PlayerAnimations.TryPlayAnimation(Animations.Animations.BodyDoubleJump, Animations.Animations.LegsDoubleJump);
            }
        }

        public void Attack()
        {
            player.PlayerUtilities.TriggerInvincibility(false);
            player.PlayerAnimations.TryPlayAnimation(Animations.Animations.BodyAttack, Animations.Animations.LegsAttack);
        }

        public void TrySwapWeapon(Global.Weapons weapon)
        {
            if(weapon == player.PlayerState.Weapon) return;
            player.PlayerState.Weapon = weapon;
            player.PlayerAnimations.SetWeapon(weapon);
            SwapWeapon();
        }
        
        public void TryShield()
        {
            player.PlayerAnimations.TryPlayAnimation(Animations.Animations.BodyShield, Animations.Animations.LegsShield);
        }
        
        public void TryDash()
        {
            player.PlayerAnimations.TryPlayAnimation(Animations.Animations.BodyDash, Animations.Animations.LegsDash);
        }
        
        public void TryDodge()
        {
            player.PlayerAnimations.TryPlayAnimation(Animations.Animations.BodyDodge, Animations.Animations.LegsDodge);
        }

        public void UnEquipWeapons()
        {
            foreach (var weapon in player.PlayerReferences.WeaponObjects)
            {
                if (weapon.gameObject.activeSelf) weapon.gameObject.SetActive(false);
            }
        }

        public void SwapWeapon()
        {
            UnEquipWeapons();
            player.PlayerReferences.WeaponObjects[(int)player.PlayerState.Weapon].gameObject.SetActive(true);
        }

        private void PlayWeaponSound(AudioClip clip)
        {
            player.PlayerReferences.WeaponObjects[(int)player.PlayerState.Weapon].PlaySound(clip);
        }

        public void Drop()
        {
            player.StartCoroutine(DropCoroutine());
        }

        private IEnumerator DropCoroutine()
        {
            player.PlayerComponents.FootCollider.enabled = false;
            yield return new WaitForSeconds(0.25f);
            player.PlayerComponents.FootCollider.enabled = true;
        }

        public void Shoot()
        {
            player.PlayerReferences.Gun.Shoot();
            PlayWeaponSound(player.PlayerReferences.ShootAudioClip);
            player.PlayerState.RangedEnergy -= player.PlayerStats.RangedAttack.Energy;
        }

        public void SideMelee()
        {
            PlayWeaponSound(player.PlayerReferences.SideMeleeAudioClip);
            player.PlayerState.MeleeEnergy -= player.PlayerStats.SideMeleeAttack.Energy;
        }
        
        public void UpMelee()
        {
            PlayWeaponSound(player.PlayerReferences.UpMeleeAudioClip);
            player.PlayerState.MeleeEnergy -= player.PlayerStats.UpMeleeAttack.Energy;
        }
        
        public void JabMelee()
        {
            PlayWeaponSound(player.PlayerReferences.JabMeleeAudioClip);
            player.PlayerState.MeleeEnergy -= player.PlayerStats.JabMeleeAttack.Energy;
        }
        
        public void DownMelee()
        {
            PlayWeaponSound(player.PlayerReferences.SideMeleeAudioClip);
            player.PlayerState.MeleeEnergy -= player.PlayerStats.DownMeleeAttack.Energy;
        }

        public void Jump(bool isDoubleJump)
        {
            if (isDoubleJump)
            {
                player.PlayerState.CanDoubleJump = false;
            }
            var jumpForce = isDoubleJump ? player.PlayerStats.DoubleJumpForce : player.PlayerStats.JumpForce;
            player.PlayerComponents.RigidBody.velocity = new Vector2(player.PlayerComponents.RigidBody.velocity.x, 0);
            player.PlayerComponents.RigidBody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Force);
            player.PlayerUtilities.PlayOneShotAudio(player.PlayerReferences.JumpAudioClip);
        }

        

        public void TriggerShield(bool active)
        {
            player.PlayerReferences.PlayerShield.TriggerShield(active);
        }

        public IEnumerator DodgeCoroutine()
        {
            player.PlayerUtilities.PlayOneShotAudio(player.PlayerReferences.DodgeAudioClip);
            player.PlayerState.CanDodge = false;
            player.PlayerComponents.RigidBody.velocity = Vector2.zero;
            player.PlayerUtilities.DodgeEffect(true);
            yield return new WaitForSeconds(0.35f);
            player.PlayerUtilities.DodgeEffect(false);
            yield return new WaitForSeconds(0.65f);
            player.PlayerState.CanDodge = true;
        }

        public void Dash()
        {
            player.PlayerUtilities.PlayOneShotAudio(player.PlayerReferences.DashAudioClip);
            player.PlayerComponents.RigidBody.velocity = Vector2.zero;
        }

        public void FastFall()
        {
            player.PlayerComponents.RigidBody.velocity = new Vector2(player.PlayerComponents.RigidBody.velocity.x, 0);
            player.PlayerComponents.RigidBody.AddForce(Vector2.down * player.PlayerStats.FastFallForce);
        }
    }
}
