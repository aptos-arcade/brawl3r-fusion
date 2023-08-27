using System;
using UnityEngine;

namespace Weapons
{
    [Serializable]
    public class StrikerData
    {
        [SerializeField] private float energy;
        public float Energy => energy;

        [SerializeField] private AudioClip audioClip;
        public AudioClip AudioClip => audioClip;
        
        [SerializeField] private AttackData attackData;
        public AttackData AttackData => attackData;
    }
}