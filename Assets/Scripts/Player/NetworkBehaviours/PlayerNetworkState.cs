using Fusion;
using UnityEngine;

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
        
        [Networked] public TickTimer StunTimer { get; set; } = TickTimer.None;
        [Networked] public TickTimer DodgeTimer { get; set; } = TickTimer.None;
        
        [Networked(OnChanged = nameof(HandleDropTimerChanged))] 
        public TickTimer DropTimer { get; set; } = TickTimer.None;
        
        [Networked] public TickTimer DodgeCooldown { get; set; } = TickTimer.None;


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
        
        [Networked] public NetworkBool IsDead { get; set; } = true;
        
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
                $"{(changed.Behaviour.player.PlayerNetworkState.DamageMultiplier - 1) * 100}%";
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
        
        public static void HandleIsInvincibleChanged(Changed<PlayerNetworkState> changed)
        {
            changed.Behaviour.player.PlayerVisualController.SetSpriteOpacity(changed.Behaviour.IsInvincible ? 0.5f : 1);
        }
    }
}