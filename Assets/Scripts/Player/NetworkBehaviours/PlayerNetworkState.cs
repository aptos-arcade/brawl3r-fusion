using Fusion;
using UnityEngine;
using Utilities;

namespace Player.NetworkBehaviours
{
    public class PlayerNetworkState: NetworkBehaviour
    {
        
        private PlayerController player;

        public override void Spawned()
        {
            player = GetComponent<PlayerController>();
        }
        
        // energy
        
        [Networked] public float ShieldEnergy { get; set; } = 1;
        
        [Networked] public float RangedEnergy { get; set; } = 1;
        
        [Networked] public float MeleeEnergy { get; set; } = 1;
        
        // timers
        
        [Networked(OnChanged = nameof(HandleHurtTimerChanged))] 
        public TickTimer HurtTimer { get; set; } = TickTimer.None;
        
        [Networked] public TickTimer ShieldStunTimer { get; set; } = TickTimer.None;
        [Networked] public TickTimer DodgeTimer { get; set; } = TickTimer.None;
        
        [Networked(OnChanged = nameof(HandleDropTimerChanged))] 
        public TickTimer DropTimer { get; set; } = TickTimer.None;
        
        [Networked] public TickTimer DodgeCooldown { get; set; } = TickTimer.None;
        
        [Networked(OnChanged = nameof(HandleInvincibleTimerChanged))]
        public TickTimer InvincibleTimer { get; set; } = TickTimer.None;

        // movement

        [Networked(OnChanged = nameof(HandleDirectionChanged))] 
        public Vector2 Direction { get; set; }
        
        [Networked] public NetworkBool CanDoubleJump { get; set; } = true;
        
        // combat
        
        [Networked(OnChanged = nameof(HandleWeaponChanged))] 
        public Global.Weapons Weapon { get; set; } = Global.Weapons.Gun;
        
        [Networked(OnChanged = nameof(HandleDamageMultiplierChanged))] 
        public float DamageMultiplier { get; set; } = 1;

        [Networked] public PlayerRef LastStriker { get; set; } = -1;
        
        [Networked(OnChanged = nameof(HandleIsInvincibleChanged))] 
        public NetworkBool IsInvincible { get; set; } = false;
        
        // input
        
        [Networked] public NetworkButtons PrevButtons { get; set; }
        
        [Networked(OnChanged = nameof(HandleIsDeadChanged))] 
        public NetworkBool IsDead { get; set; } = true;
        
        // change handlers
        
        public static void HandleDirectionChanged(Changed<PlayerNetworkState> changed)
        {
            if (changed.Behaviour.player.PlayerNetworkState.Direction.x == 0) return;
            var direction = changed.Behaviour.player.PlayerNetworkState.Direction.x < 0 ? -1 : 1;
            changed.Behaviour.player.transform.localScale = new Vector3(direction, 1, 1);
            changed.Behaviour.player.PlayerReferences.PlayerCanvas.transform.localScale = new Vector3(direction, 1, 1);
        }


        public static void HandleWeaponChanged(Changed<PlayerNetworkState> changed)
        {
            changed.Behaviour.player.PlayerAttacks.SwapWeapon();
        }

        public static void HandleDamageMultiplierChanged(Changed<PlayerNetworkState> changed)
        {
            changed.Behaviour.player.PlayerReferences.DamageDisplay.text =
                $"{((changed.Behaviour.player.PlayerNetworkState.DamageMultiplier - 1) * 100):F0}%";
        }
        
        public static void HandleDropTimerChanged(Changed<PlayerNetworkState> changed)
        {
            changed.Behaviour.player.PlayerComponents.FootCollider.enabled = !changed.Behaviour.DropTimer.IsRunning;
        }

        public static void HandleHurtTimerChanged(Changed<PlayerNetworkState> changed)
        {
            if (changed.Behaviour.HurtTimer.IsRunning)
            {
                changed.Behaviour.player.PlayerVisualController.SetSpriteColors(Color.red);
            }
            else
            {
                changed.Behaviour.player.PlayerVisualController.ResetSpriteColors();
            }
        }
        
        public static void HandleInvincibleTimerChanged(Changed<PlayerNetworkState> changed)
        {
            if (!changed.Behaviour.InvincibleTimer.IsRunning)
            {
                changed.Behaviour.IsInvincible = false;
            }
        }
        
        public static void HandleIsInvincibleChanged(Changed<PlayerNetworkState> changed)
        {
            changed.Behaviour.player.PlayerVisualController.SetSpriteOpacity(changed.Behaviour.IsInvincible ? 0.5f : 1);
        }

        public static void HandleIsDeadChanged(Changed<PlayerNetworkState> changed)
        {
            var player = changed.Behaviour.player;
            var isDead = changed.Behaviour.IsDead;
            
            player.PlayerReferences.PlayerObject.SetActive(!isDead);
            
            player.PlayerComponents.RigidBody.simulated = !isDead;
            player.PlayerComponents.RigidBody.velocity = Vector2.zero;

            if (!isDead)
            {
                player.PlayerVisualController.UpdateLivesDisplay();
            }
            
            if (FusionUtils.IsLocalPlayer(player.Object))
            {
                player.PlayerReferences.PlayerCanvasManager.ShowEnergyUi(!isDead);
            }
        }
    }
}