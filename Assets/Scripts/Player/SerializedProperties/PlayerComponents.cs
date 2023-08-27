using System;
using UnityEngine;

namespace Player.SerializedProperties
{
    [Serializable]
    public class PlayerComponents
    {
        [SerializeField] private Rigidbody2D rigidBody;
        public Rigidbody2D RigidBody => rigidBody;
        
        [SerializeField] private BoxCollider2D footCollider;
        public BoxCollider2D FootCollider => footCollider;

        [SerializeField] private LayerMask ground;
        public LayerMask Ground => ground;

        [SerializeField] private LayerMask platform;
        public LayerMask Platform => platform;
        
        [SerializeField] private AudioSource runAudioSource;
        public AudioSource RunAudioSource => runAudioSource;
        
        [SerializeField] private AudioSource oneShotAudioSource;
        public AudioSource OneShotAudioSource => oneShotAudioSource;
    }
}
