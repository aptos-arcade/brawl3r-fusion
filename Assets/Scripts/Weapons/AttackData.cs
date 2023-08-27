using System;
using Fusion;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public struct AttackData: INetworkStruct
    {
        public float knockBack;
        public float damage;
        public Vector2 knockBackDirection;
        public float stunTime;
    }
}