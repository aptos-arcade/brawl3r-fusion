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

        protected virtual void OnPlayerStrike(Vector2 position, PlayerController player)
        {
            Runner.Spawn(playerHitEffect, position, Quaternion.identity);
        }
        
        protected virtual void OnShieldStrike(Vector2 position, PlayerShield shield)
        {
            Runner.Spawn(shieldHitEffect, position, Quaternion.identity);
        }

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
            
            var shield = col.GetComponent<PlayerShield>();
            if (shield != null && !FusionUtils.IsLocalPlayer(shield.Object)
                               && !FusionUtils.IsSameTeam(shield.Object))
            {
                OnShieldStrike(col.transform.position, shield);
                shield.OnHit(this);
            }
        }

        protected abstract int GetDirection();
    }
}