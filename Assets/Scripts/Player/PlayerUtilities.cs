using System.Collections;
using System.Collections.Generic;
using Fusion;
using Gameplay;
using Photon;
using Player.Commands;
using UnityEngine;
using Utilities;
using Weapons;

namespace Player
{
    public class PlayerUtilities
    {
        private readonly PlayerController player;

        private readonly List<Command> commands = new();
        
        public bool IsOnGround => player.PlayerComponents.FootCollider.IsTouchingLayers(player.PlayerComponents.Ground.value);
        public bool IsOnPlatform => player.PlayerComponents.FootCollider.IsTouchingLayers(player.PlayerComponents.Platform.value);
        public bool IsGrounded => IsOnGround || IsOnPlatform;
        private bool IsFalling => player.PlayerComponents.RigidBody.velocity.y < 0 && !IsGrounded;
        
        public bool IsDashing => player.PlayerComponents.Animator.CurrentAnimationBody == "Body_Dash";
        public bool IsDodging => player.PlayerComponents.Animator.CurrentAnimationBody == "Body_Dodge";
        public bool IsStunned => player.PlayerComponents.Animator.CurrentAnimationBody == "Body_Stunned";
        public bool IsShielding => player.PlayerComponents.Animator.CurrentAnimationBody == "Body_Shield";
        
        public bool IsAcceptingInput => !player.PlayerState.IsDead && !player.PlayerState.IsDisabled 
                                                              && MatchManager.Instance.GameState != GameState.MatchOver;

        private Coroutine hurtCoroutine;
        private Coroutine shieldStunCoroutine;

        public PlayerUtilities(PlayerController player)
        {
            this.player = player;
            commands.Add(new JumpCommand(player, KeyCode.UpArrow));
            commands.Add(new DropCommand(player, KeyCode.DownArrow));
            commands.Add(new ShootCommand(player, KeyCode.Space));
            commands.Add(new JabMeleeCommand(player, KeyCode.S));
            commands.Add(new UpMeleeCommand(player, KeyCode.W));
            commands.Add(new SideMeleeCommand(player, KeyCode.A, -1));
            commands.Add(new SideMeleeCommand(player, KeyCode.D, 1));
            commands.Add(new ShieldCommand(player, KeyCode.LeftShift));
            commands.Add(new DashCommand(player, KeyCode.LeftArrow, -1));
            commands.Add(new DashCommand(player, KeyCode.RightArrow, 1));
        }
        
        public PlayerNetworkInput GetPlayerInput()
        {
            var data = new PlayerNetworkInput()
            {
                HorizontalInput = player.PlayerState.HorizontalInput,
            };
            foreach (var command in commands)
            {
                data.NetworkButtons.Set(command.Button, Input.GetKey(command.Key));
            }
            return data;
        }

        public void HandleInput()
        {
            if (player.PlayerState.IsDisabled || 
                !player.Runner.TryGetInputForPlayer<PlayerNetworkInput>(player.Object.InputAuthority, out var input))
                return;
            // if (IsStunned && Input.anyKeyDown)
            // {
            //     player.PlayerComponents.Animator.TryPlayAnimation("Body_Idle");
            //     player.PlayerComponents.Animator.TryPlayAnimation("Legs_Idle");
            // }

            if (player.PlayerState.CanMove)
            {
                if (input.HorizontalInput != 0 || !player.PlayerUtilities.IsDashing)
                {
                    player.PlayerState.Direction = new Vector2(input.HorizontalInput, 0);
                }
            }
            else if(!IsDodging)
            {
                player.PlayerState.Direction = Vector2.zero;
            }

            var pressed = input.NetworkButtons.GetPressed(player.PlayerState.PrevButtons);
            foreach (var command in commands)
            {
                if (pressed.WasPressed(player.PlayerState.PrevButtons, command.Button))
                {
                    command.WasPressed();
                }
                if(pressed.WasReleased(player.PlayerState.PrevButtons, command.Button))
                {
                    command.WasHeld();
                }
                else
                {
                    command.WasReleased();
                }
            }
            player.PlayerState.PrevButtons = input.NetworkButtons;
        }

        public void HandleAir()
        {
            if(IsFalling)
            {
                player.PlayerComponents.Animator.TryPlayAnimation("Body_Fall");
                player.PlayerComponents.Animator.TryPlayAnimation("Legs_Fall");
            }
            if(IsGrounded)
            {
                player.PlayerState.CanDoubleJump = true;
            }
        }

        public void HandleDeath()
        {
            if (!player.PlayerState.IsDead && (Mathf.Abs(player.transform.position.x) > 30 || Mathf.Abs(player.transform.position.y) > 16))
            {
                OnDeath();
            }
        }

        public void HandleEnergy()
        {
            if(player.PlayerState.MeleeEnergy < 1)
            {
                player.PlayerState.MeleeEnergy += player.Runner.DeltaTime / player.PlayerStats.MeleeEnergyRegenTime;
            }
            if(player.PlayerState.RangedEnergy < 1)
            {
                player.PlayerState.RangedEnergy += player.Runner.DeltaTime / player.PlayerStats.RangedEnergyRegenTime;
            }
            if(player.PlayerState.ShieldEnergy < 1 && !player.PlayerReferences.PlayerShield.gameObject.activeSelf)
            {
                player.PlayerState.ShieldEnergy += player.Runner.DeltaTime / player.PlayerStats.ShieldEnergyRegenTime;
            }
        }
        
        private void OnDeath()
        {
            player.Runner.Spawn(player.PlayerReferences.ExplosionPrefab, player.transform.position, Quaternion.identity);
            player.PlayerUtilities.DeathRevive(false);
            player.PlayerComponents.Animator.OnDeath();
            player.PlayerComponents.RigidBody.velocity = Vector2.zero;
            player.PlayerState.Direction = Vector2.zero;
            player.PlayerState.Lives--;
            player.PlayerReferences.PlayerLives.GetChild(player.PlayerState.Lives).gameObject.SetActive(false);
            GameManager.Instance.SetEnergyUIActive(false);
            // MatchManager.PlayerDeathSend(player.photonView.OwnerActorNr, player.PlayerState.StrikerActorNumber);
            player.PlayerState.LastStriker = -1;
        }

        public void StrikerCollision(Striker striker)
        {
            if(hurtCoroutine != null) player.StopCoroutine(hurtCoroutine);
            hurtCoroutine = player.StartCoroutine(HurtCoroutine(striker));
            player.PlayerCameraController.ShakeCamera(0.25f, new Vector2(5f, 5f));
            player.PlayerState.LastStriker = striker.Object.InputAuthority;
            player.PlayerState.DamageMultiplier += striker.strikerData.Damage;
            player.PlayerReferences.DamageDisplay.text = ((player.PlayerState.DamageMultiplier - 1) * 100).ToString("F0") + "%";
            player.PlayerUtilities.PlayOneShotAudio(player.PlayerReferences.DamageAudioClip);
            player.PlayerComponents.RigidBody.velocity = striker.KnockBackSignedDirection.normalized *
                                                         striker.strikerData.KnockBack *
                                                         player.PlayerState.DamageMultiplier;
        }

        private IEnumerator HurtCoroutine(Striker striker)
        {
            player.PlayerUtilities.StunEffect(true, Color.red);
            yield return new WaitForSeconds(striker.strikerData.StunTime * player.PlayerState.DamageMultiplier);
            player.PlayerUtilities.StunEffect(false, Color.red);
            hurtCoroutine = null;
        }

        public void ShieldCollision(Striker striker)
        {
            player.PlayerState.ShieldEnergy -= striker.strikerData.Damage;
        }

        private void StunEffect(bool stunned, Color effectColor)
        {
            if (stunned)
            {
                player.PlayerComponents.Animator.TryPlayAnimation("Body_Stunned");
                player.PlayerComponents.Animator.TryPlayAnimation("Legs_Stunned");
            }
            player.PlayerState.IsDisabled = stunned;
            player.PlayerState.CanMove = !stunned;
            for (var i = 0; i < player.PlayerComponents.PlayerSprites.Count; i++)
            {
                player.PlayerComponents.PlayerSprites[i].color = stunned 
                    ? effectColor
                    : player.PlayerComponents.PlayerSpriteColors[i];
            }
        }

        public void ShieldHit(PlayerShield shield)
        {
            if(shieldStunCoroutine != null) player.StopCoroutine(shieldStunCoroutine);
            shieldStunCoroutine = player.StartCoroutine(ShieldStunCoroutine(shield));
        }

        private IEnumerator ShieldStunCoroutine(PlayerShield shield)
        {
            player.PlayerUtilities.StunEffect(true, new Color(240, 0, 255));
            yield return new WaitForSeconds(shield.ShieldStunDuration);
            player.PlayerUtilities.StunEffect(false, new Color(240, 0, 255));
            shieldStunCoroutine = null;
        }

        public void DodgeEffect(bool dodging)
        {
            player.PlayerState.CanMove = !dodging;
            player.PlayerComponents.BodyCollider.enabled = !dodging;
            player.PlayerComponents.RigidBody.gravityScale = dodging ? 0 : player.PlayerStats.GravityScale;
            foreach (var renderer in player.PlayerComponents.PlayerSprites)
            {
                var color = renderer.color;
                color.a = dodging ? 0.5f : 1;
                renderer.color = color;
            }
        }

        public void GetSpriteRenderers()
        {
            foreach (Transform transform in player.PlayerReferences.PlayerMesh.transform)
            {
                var spriteRenderer = transform.GetComponent<SpriteRenderer>();
                player.PlayerComponents.PlayerSprites.Add(spriteRenderer);
                player.PlayerComponents.PlayerSpriteColors.Add(spriteRenderer.color);
            }
        }

        public void DeathRevive(bool isRevive)
        {
            Debug.Log("Setting IsDead to " + !isRevive + " and CanMove to " + isRevive + "");
            player.PlayerState.IsDead = !isRevive;
            player.PlayerState.CanMove = isRevive;
            
        }

        public void JumpImpl(float jumpForce)
        {
            Debug.Log("Jump");
            Debug.Log(jumpForce);
            player.PlayerComponents.RigidBody.velocity = new Vector2(player.PlayerComponents.RigidBody.velocity.x, 0);
            player.PlayerComponents.RigidBody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Force);
            PlayOneShotAudio(player.PlayerReferences.JumpAudioClip);
        }

        public void HandleAnimation(string animation)
        {
            switch (animation)
            {
                case "Shoot":
                    player.PlayerActions.Shoot();
                    break;
                case "Side_Melee":
                    player.PlayerActions.SideMelee();
                    break;
                case "Up_Melee":
                    player.PlayerActions.UpMelee();
                    break;
                case "Jab_Melee":
                    player.PlayerActions.JabMelee();
                    break;
                case "Down_Melee":
                    player.PlayerActions.DownMelee();
                    break;
                case "Jump":
                    player.PlayerActions.Jump();
                    break;
                case "Double_Jump":
                    player.PlayerActions.DoubleJump();
                    break;
                case "Shield":
                    player.PlayerActions.TriggerShield(true);
                    break;
                case "Dodge":
                    player.StartCoroutine(player.PlayerActions.DodgeCoroutine());
                    break;
                case "Dash":
                    player.PlayerActions.Dash();
                    break;
                case "FastFall":
                    player.PlayerActions.FastFall();
                    break;
            }
        }
        
        public static bool IsSameTeam(NetworkObject other)
        {
            return MatchManager.Instance.SessionPlayers[other.InputAuthority].Team == 
                   MatchManager.Instance.LocalPlayerInfo.Team;
        }
        
        public void TriggerInvincibility(bool isInvincible)
        {
            if(player.PlayerState.IsInvincible == isInvincible) return;
            player.PlayerState.IsInvincible = isInvincible;
            foreach (var renderer in player.PlayerComponents.PlayerSprites)
            {
                var color = renderer.color;
                color.a = isInvincible ? 0.5f : 1;
                renderer.color = color;
            }
        }

        public void PlayOneShotAudio(AudioClip clip)
        {
            player.PlayerComponents.OneShotAudioSource.Stop();
            player.PlayerComponents.OneShotAudioSource.PlayOneShot(clip);
        }

        public static void HandleWeaponChanged(Changed<PlayerController> changed)
        {
            changed.Behaviour.PlayerActions.SwapWeapon();
        }
        
        public static void HandleIsDeadChanged(Changed<PlayerController> changed)
        {
            Debug.Log("HandleIsDeadChanged");
            var isDead = changed.Behaviour.PlayerState.IsDead;
            var player = changed.Behaviour;
            Debug.Log("IsDead: " + isDead);
            
            if (isDead)
            {
                player.PlayerCameraController.RemovePlayer(player.transform);
                player.PlayerActions.UnEquipWeapons();
            }
            else
            {
                player.PlayerCameraController.AddPlayer(player.transform);
                player.PlayerActions.SwapWeapon();
            }
            
            if (FusionUtils.IsLocalPlayer(player.Object))
            {
                GameManager.Instance.SetEnergyUIActive(!isDead);
            }
            
            player.PlayerReferences.PlayerObject.SetActive(!isDead);
            
            player.PlayerComponents.RigidBody.simulated = !isDead;
            
            player.PlayerState.MeleeEnergy = isDead ? 0 : 1;
            player.PlayerState.RangedEnergy = isDead ? 0 : 1;
        }

    }
}
