using System.Collections.Generic;
using Fusion;
using Player;
using Player.NetworkBehaviours;
using UnityEngine;
using Utilities;

namespace Weapons
{
    public abstract class Striker: NetworkBehaviour
    {
        public StrikerData strikerData;
        
        [SerializeField] private NetworkPrefabRef playerHitEffect;
        [SerializeField] private NetworkPrefabRef shieldHitEffect;
        
        [SerializeField] private Collider2D strikerCollider;
        
        // [SerializeField] private LayerMask playerLayerMask;
        // [SerializeField] private LayerMask shieldLayerMask;
        
        // public override void FixedUpdateNetwork()
        // {
        //     if (!strikerCollider.enabled || !HasStateAuthority) return;
        //     CheckIfHitPlayer();
        //     CheckIfHitShield();
        // }

        protected virtual void OnPlayerStrike(Vector2 position, PlayerController player)
        {
            Runner.Spawn(playerHitEffect, position, Quaternion.identity);
        }
        
        protected virtual void OnShieldStrike(Vector2 position, PlayerShield shield)
        {
            Runner.Spawn(shieldHitEffect, position, Quaternion.identity);
        }

        // private readonly List<LagCompensatedHit> playerHits = new();
        // private void CheckIfHitPlayer()
        // {
        //     var colliderBounds = strikerCollider.bounds;
        //     Runner.LagCompensation.OverlapBox(colliderBounds.center, colliderBounds.size, transform.rotation,
        //         Object.InputAuthority, playerHits, playerLayerMask);
        //     if (playerHits.Count <= 0) return;
        //     foreach (var item in playerHits)
        //     {
        //         if(item.Hitbox == null) continue;
        //         var player = item.Hitbox.GetComponentInParent<PlayerController>();
        //         if(player == null) continue;
        //         var didNotHitSelf = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;
        //         if (!didNotHitSelf || player.PlayerNetworkState.IsDead || player.PlayerNetworkState.IsInvincible) continue;
        //         Debug.Log("Hit player");
        //         player.PlayerRpcs.RpcStrikerCollision(strikerData.AttackData, GetDirection(), Object.StateAuthority);
        //         OnPlayerStrike(item.Hitbox.transform.position, player);
        //         break;
        //     }
        // }
        //
        // private readonly List<LagCompensatedHit> shieldHits = new();
        // private void CheckIfHitShield()
        // {
        //     // var colliderBounds = strikerCollider.bounds;
        //     // Runner.LagCompensation.OverlapBox(colliderBounds.center, colliderBounds.size, transform.rotation, 
        //     //     Object.InputAuthority, shieldHits, shieldLayerMask);
        //     // if (shieldHits.Count <= 0) return;
        //     // Debug.Log("Hit shield");
        //     // foreach (var item in shieldHits)
        //     // {
        //     //     if(item.Hitbox == null) continue;
        //     //     var shield = item.Hitbox.GetComponentInParent<PlayerShield>();
        //     //     var didNotHitSelf = shield.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;
        //     //     if (!didNotHitSelf) continue;
        //     //     if (Runner.IsServer)
        //     //     {
        //     //         shield.OnHit(this);
        //     //     }
        //     //     OnShieldStrike(item.Hitbox.transform.position, shield);
        //     //     break;
        //     // }
        // }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(!HasStateAuthority) return;
            
            var player = col.GetComponent<PlayerController>();
            if (player != null && !FusionUtils.IsLocalPlayer(player.Object)
                               && !FusionUtils.IsSameTeam(player.Object) && !player.PlayerNetworkState.IsInvincible)
            {
                player.PlayerRpcs.RpcStrikerCollision(strikerData.AttackData, GetDirection(), Object.StateAuthority);
                OnPlayerStrike(col.transform.position, player);
                return;
            }
            
            var shield = strikerCollider.GetComponent<PlayerShield>();
            if (shield != null && !FusionUtils.IsLocalPlayer(shield.Object)
                               && !FusionUtils.IsSameTeam(shield.Object))
            {
                OnShieldStrike(strikerCollider.transform.position, shield);
                shield.OnHit(this);
            }
        }

        protected abstract int GetDirection();
    }
}