using Fusion;
using TMPro;
using UnityEngine;
using Weapons;

namespace Player
{
    [System.Serializable]
    public class PlayerReferences
    {

        [Header("Text References")]

        [SerializeField] private TMP_Text nameTag;
        public TMP_Text NameTag => nameTag;
        
        [SerializeField] private TMP_Text collectionTag;
        public TMP_Text CollectionTag => collectionTag;

        [SerializeField] private TMP_Text damageDisplay;
        public TMP_Text DamageDisplay => damageDisplay;
        
        [Header("Prefabs")]

        [SerializeField] private NetworkPrefabRef explosionPrefab;
        public NetworkPrefabRef ExplosionPrefab => explosionPrefab;

        [Header("Game Objects")] 
        [SerializeField] private GameObject playerObject;
        public GameObject PlayerObject => playerObject;

        [SerializeField] private GameObject playerCanvas;
        public GameObject PlayerCanvas => playerCanvas;

        [SerializeField] private Weapon[] weaponObjects;
        public Weapon[] WeaponObjects => weaponObjects;

        public Sword Sword => weaponObjects[(int)Global.Weapons.Sword].GetComponent<Sword>();
        
        public Gun Gun => weaponObjects[(int)Global.Weapons.Gun].GetComponent<Gun>();

        [Header("Transforms")]

        [SerializeField] private Transform playerLives;
        public Transform PlayerLives => playerLives;
        
        [SerializeField] private PlayerShield playerShield;
        public PlayerShield PlayerShield => playerShield;

        [Header("Audio Clips")]
        
        [SerializeField] private AudioClip shootAudioClip;
        public AudioClip ShootAudioClip => shootAudioClip;
        
        [SerializeField] private AudioClip sideMeleeAudioClip;
        public AudioClip SideMeleeAudioClip => sideMeleeAudioClip;
        
        [SerializeField] private AudioClip jabMeleeAudioClip;
        public AudioClip JabMeleeAudioClip => jabMeleeAudioClip;
        
        [SerializeField] private AudioClip upMeleeAudioClip;
        public AudioClip UpMeleeAudioClip => upMeleeAudioClip;
        
        [SerializeField] private AudioClip jumpAudioClip;
        public AudioClip JumpAudioClip => jumpAudioClip;
        
        [SerializeField] private AudioClip damageAudioClip;
        public AudioClip DamageAudioClip => damageAudioClip;
        
        [SerializeField] private AudioClip dashAudioClip;
        public AudioClip DashAudioClip => dashAudioClip;
        
        [SerializeField] private AudioClip dodgeAudioClip;
        public AudioClip DodgeAudioClip => dodgeAudioClip;
        
        
        [Header("Character Specific")]
        
        [SerializeField] private NetworkPrefabRef portal;
        public NetworkPrefabRef Portal => portal;
        
        [SerializeField] private GameObject playerMesh;
        public GameObject PlayerMesh => playerMesh;
        
        
    }
}
