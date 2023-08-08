using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Weapons
{
    public class Striker: NetworkBehaviour
    {
        // protected PlayerScript Owner;

        public StrikerData strikerData;

        public Vector2 KnockBackSignedDirection { get; protected set; }
        
        [SerializeField] private NetworkPrefabRef playerHitEffect;
        [SerializeField] private NetworkPrefabRef shieldHitEffect;
        
        [SerializeField] private Collider2D col;
        
        [SerializeField] private LayerMask playerLayerMask;
        [SerializeField] private LayerMask shieldLayerMask;

        private void Start()
        {
            KnockBackSignedDirection = strikerData.KnockBackDirection;
        }

        protected virtual void OnPlayerStrike(Vector2 position)
        {
            Runner.Spawn(playerHitEffect, position, Quaternion.identity);
        }
        
        protected virtual void OnShieldStrike(Vector2 position)
        {
            Runner.Spawn(shieldHitEffect, position, Quaternion.identity);
        }

        private void CheckIfHitPlayer()
        {
            List<LagCompensatedHit> hits = new();
            Runner.LagCompensation.OverlapBox(transform.position, col.bounds.size, Quaternion.identity,
                Object.InputAuthority, hits, playerLayerMask);
            if (hits.Count <= 0) return;
            foreach (var item in hits)
            {
                if(item.Hitbox == null) continue;
                // var player = item.Hitbox.GetComponentInParent<PlayerController>();
                // var didNotHitSelf = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;
                // if (!didNotHitSelf || !player.IsAlive) continue;
                // if (Runner.IsServer)
                // {
                    // player.GetComponent<PlayerHealthController>().RpcReducePlayerHealth(damage);
                // }
                // DidHitSomething = true;
                // break;
            }
        }
        
        private void CheckIfHitShield()
        {
            List<LagCompensatedHit> hits = new();
            Runner.LagCompensation.OverlapBox(transform.position, col.bounds.size, Quaternion.identity,
                Object.InputAuthority, hits, shieldLayerMask);
            if (hits.Count <= 0) return;
            foreach (var item in hits)
            {
                if(item.Hitbox == null) continue;
                // var player = item.Hitbox.GetComponentInParent<PlayerController>();
                // var didNotHitSelf = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;
                // if (!didNotHitSelf || !player.IsAlive) continue;
                // if (Runner.IsServer)
                // {
                // player.GetComponent<PlayerHealthController>().RpcReducePlayerHealth(damage);
                // }
                // DidHitSomething = true;
                // break;
            }
        }
    }
}