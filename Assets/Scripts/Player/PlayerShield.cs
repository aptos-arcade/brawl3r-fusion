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
            if(player.PlayerState.ShieldEnergy <= 0 || player.PlayerComponents.Animator.CurrentAnimationBody != "Body_Shield")
            {
                player.PlayerActions.TriggerShield(false);
                player.PlayerComponents.Animator.OnAnimationDone("Body_Shield");
                player.PlayerComponents.Animator.OnAnimationDone("Legs_Shield");
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
