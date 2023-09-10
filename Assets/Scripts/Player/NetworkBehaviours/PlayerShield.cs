using Fusion;
using UnityEngine;
using Weapons;

namespace Player.NetworkBehaviours
{
    public class PlayerShield : SimulationBehaviour
    {
        [SerializeField] private PlayerController player;
        
        [SerializeField] private NetworkPrefabRef shieldOffEffect;
        
        public float ShieldStunDuration => 
            player.PlayerStats.ShieldStunDuration * 
            transform.localScale.x;

        public override void FixedUpdateNetwork()
        {
            if(player.PlayerNetworkState.ShieldEnergy <= 0 || !player.PlayerAnimations.IsCurrentBodyAnimation(Animations.Animations.BodyShield))
            {
                TriggerShield(false);
                player.PlayerAnimations.OnAnimationDone(Animations.Animations.BodyShield,
                    Animations.Animations.LegsShield);
            }
            else
            {
                var scale = 0.1f + 0.9f * player.PlayerNetworkState.ShieldEnergy;
                transform.localScale = new Vector3(scale, scale, 1);
                player.PlayerNetworkState.ShieldEnergy -= Runner.DeltaTime / player.PlayerStats.ShieldDuration;
            }
        }

        public void TriggerShield(bool trigger)
        {
            gameObject.SetActive(trigger);
            player.PlayerUtilities.TriggerInvincibility(trigger);
            if (trigger) return;
            var shieldOff = Runner.Spawn(shieldOffEffect, transform.position, Quaternion.identity, Object.StateAuthority);
            shieldOff.transform.localScale = transform.localScale;
        }

        public void OnHit(Striker striker)
        {
            player.PlayerRpcs.RpcShieldCollision(striker.strikerData.AttackData);
        }
    }
}
