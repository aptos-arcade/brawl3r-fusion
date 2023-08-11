using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerComponents
    {
        [SerializeField] private Rigidbody2D rigidBody;
        public Rigidbody2D RigidBody => rigidBody;

        [SerializeField] private BoxCollider2D footCollider;
        public Collider2D FootCollider => footCollider;

        [SerializeField] private CapsuleCollider2D bodyCollider;
        public CapsuleCollider2D BodyCollider { get => bodyCollider; set => bodyCollider = value; }

        [SerializeField] private LayerMask ground;
        public LayerMask Ground => ground;

        [SerializeField] private LayerMask platform;
        public LayerMask Platform => platform;

        [SerializeField] private AudioSource runAudioSource;
        public AudioSource RunAudioSource => runAudioSource;
        
        [SerializeField] private AudioSource oneShotAudioSource;
        public AudioSource OneShotAudioSource => oneShotAudioSource;

        public List<SpriteRenderer> PlayerSprites { get; set; } = new();
        
        public List<Color> PlayerSpriteColors { get; set; } = new();
    }
}
