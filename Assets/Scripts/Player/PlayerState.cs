using Fusion;
using Photon;
using UnityEngine;

namespace Player
{
    public class PlayerState
    {
        
        // networked state
        
        [Networked] public Vector2 Direction { get; set; }
        
        [Networked] public bool CanDoubleJump { get; set; } = true;
        
        [Networked] public float ShieldEnergy { get; set; } = 1;

        [Networked] public PlayerRef LastStriker { get; set; } = -1;
        
        [Networked] public NetworkButtons PrevButtons { get; set; }
        
        [Networked] 
        public NetworkBool IsDead { get; set; } = true;
        
        [Networked(OnChanged = nameof(NetworkChangeHandlers.HandleWeaponChanged))] 
        public Global.Weapons Weapon { get; set; } = Global.Weapons.Gun;
        
        [Networked] public NetworkBool IsInvincible { get; set; }
        
        [Networked] public float DamageMultiplier { get; set; } = 1;

        // local state
        public float HorizontalInput { get; set; }
        
        public bool CanMove { get; set; }
        
        public bool CanDodge { get; set; } = true;

        
        public bool IsDisabled { get; set; }
        
        
        public int Lives { get; set; } = 3;
        
        public float RangedEnergy { get; set; } = 1;
        
        public float MeleeEnergy { get; set; } = 1;
    }
}