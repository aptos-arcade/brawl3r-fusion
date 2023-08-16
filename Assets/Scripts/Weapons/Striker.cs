using System.Collections.Generic;
using Fusion;
using Player;
using Player.NetworkBehaviours;
using UnityEngine;

namespace Weapons
{
    public class Striker: SimulationBehaviour
    {
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

        public override void FixedUpdateNetwork()
        {
            if (!col.enabled) return;
            CheckIfHitPlayer();
            CheckIfHitShield();
        }

        protected virtual void OnPlayerStrike(Vector2 position, PlayerController player)
        {
            Runner.Spawn(playerHitEffect, position, Quaternion.identity);
        }
        
        protected virtual void OnShieldStrike(Vector2 position, PlayerShield shield)
        {
            Runner.Spawn(shieldHitEffect, position, Quaternion.identity);
        }

        private readonly List<LagCompensatedHit> playerHits = new();
        private void CheckIfHitPlayer()
        {
            var colliderBounds = col.bounds;
            Runner.LagCompensation.OverlapBox(colliderBounds.center, colliderBounds.size, transform.rotation,
                Object.InputAuthority, playerHits, playerLayerMask);
            if (playerHits.Count <= 0) return;
            foreach (var item in playerHits)
            {
                if(item.Hitbox == null) continue;
                var player = item.Hitbox.GetComponentInParent<PlayerController>();
                if(player == null) continue;
                var didNotHitSelf = player.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;
                if (!didNotHitSelf || player.PlayerNetworkState.IsDead) continue;
                if (Runner.IsServer)
                {
                    Debug.Log("Hit player");
                    player.PlayerUtilities.StrikerCollision(this);
                }
                OnPlayerStrike(item.Hitbox.transform.position, player);
                break;
            }
        }

        private readonly List<LagCompensatedHit> shieldHits = new();
        private void CheckIfHitShield()
        {
            var colliderBounds = col.bounds;
            Runner.LagCompensation.OverlapBox(colliderBounds.center, colliderBounds.size, transform.rotation, 
                Object.InputAuthority, shieldHits, shieldLayerMask);
            if (shieldHits.Count <= 0) return;
            Debug.Log("Hit shield");
            // foreach (var item in shieldHits)
            // {
            //     if(item.Hitbox == null) continue;
            //     var shield = item.Hitbox.GetComponentInParent<PlayerShield>();
            //     var didNotHitSelf = shield.Object.InputAuthority.PlayerId != Object.InputAuthority.PlayerId;
            //     if (!didNotHitSelf) continue;
            //     if (Runner.IsServer)
            //     {
            //         shield.OnHit(this);
            //     }
            //     OnShieldStrike(item.Hitbox.transform.position, shield);
            //     break;
            // }
        }
    }
}