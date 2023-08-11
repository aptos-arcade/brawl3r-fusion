using Fusion;
using UnityEngine;

namespace Player
{
    public class PlayerShield : NetworkBehaviour
    {
        private PlayerController player;
        
        [Networked] private NetworkBool IsActive { get; set; }

        public float ShieldStunDuration => 
            player.PlayerStats.ShieldStunDuration * 
            transform.localScale.x;
        
        public void SetPlayer(PlayerController localPlayer)
        {
            player = localPlayer;
        }
        
        public override void FixedUpdateNetwork()
        {
            if(IsActive != gameObject.activeSelf)
            {
                gameObject.SetActive(IsActive);
            }
            if(player.PlayerState.ShieldEnergy <= 0 || !player.PlayerAnimations.IsCurrentBodyAnimation(Animations.Animations.BodyShield))
            {
                player.PlayerActions.TriggerShield(false);
                player.PlayerAnimations.OnAnimationDone(Animations.Animations.BodyShield,
                    Animations.Animations.LegsShield);
            }
            var scale = 0.1f + 0.9f * player.PlayerState.ShieldEnergy;
            transform.localScale = new Vector3(scale, scale, 1);
            player.PlayerState.ShieldEnergy -= Runner.DeltaTime / player.PlayerStats.ShieldDuration;
        }

        public void TriggerShield(bool trigger)
        {
            IsActive = trigger;
            player.PlayerState.IsInvincible = trigger;
        }
    }
}
