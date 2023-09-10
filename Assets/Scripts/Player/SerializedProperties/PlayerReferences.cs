using Characters;
using Fusion;
using Player.Animations;
using Player.NetworkBehaviours;
using Player.PlayerCanvas;
using TMPro;
using UnityEngine;
using Weapons;

namespace Player.SerializedProperties
{
    [System.Serializable]
    public class PlayerReferences
    {
        [Header("Custom Components")] 
        
        [SerializeField] private AnyStateAnimator animator;
        public AnyStateAnimator Animator => animator;
        
        [SerializeField] private PlayerCanvasManager playerCanvasManager;
        public PlayerCanvasManager PlayerCanvasManager => playerCanvasManager;
        
        [SerializeField] private PlayerShield playerShield;
        public PlayerShield PlayerShield => playerShield;
        
        [SerializeField] private Weapon[] weaponObjects;
        public Weapon[] WeaponObjects => weaponObjects;

        public Sword Sword => weaponObjects[(int)Global.Weapons.Sword].GetComponent<Sword>();
        
        public Gun Gun => weaponObjects[(int)Global.Weapons.Gun].GetComponent<Gun>();
        
        [Header("Text References")]

        [SerializeField] private TMP_Text nameTag;
        public TMP_Text NameTag => nameTag;
        
        [SerializeField] private TMP_Text collectionTag;
        public TMP_Text CollectionTag => collectionTag;

        [SerializeField] private TMP_Text damageDisplay;
        public TMP_Text DamageDisplay => damageDisplay;
        
        [Header("Network Prefabs")]

        [SerializeField] private NetworkPrefabRef explosionPrefab;
        public NetworkPrefabRef ExplosionPrefab => explosionPrefab;
        
        [SerializeField] private NetworkPrefabRef portal;
        public NetworkPrefabRef Portal => portal;
        
        [SerializeField] private NetworkPrefabRef dashEffect;
        public NetworkPrefabRef DashEffect => dashEffect;
        
        [SerializeField] private NetworkPrefabRef jumpEffect;
        public NetworkPrefabRef JumpEffect => jumpEffect;
        
        [SerializeField] private NetworkPrefabRef doubleJumpEffect;
        public NetworkPrefabRef DoubleJumpEffect => doubleJumpEffect;
        
        [SerializeField] private NetworkPrefabRef bloodEffect;
        public NetworkPrefabRef BloodEffect => bloodEffect;

        [Header("Game Objects")] 
        
        [SerializeField] private GameObject playerObject;
        public GameObject PlayerObject => playerObject;

        [SerializeField] private GameObject playerCanvas;
        public GameObject PlayerCanvas => playerCanvas;
        
        [SerializeField] private GameObject stunEffect;
        public GameObject StunEffect => stunEffect;

        [Header("Transforms")]

        [SerializeField] private Transform playerLives;
        public Transform PlayerLives => playerLives;

        [SerializeField] private Transform groundCheck;
        public Transform GroundCheck => groundCheck;
        
        [SerializeField] private Transform dashEffectTransform;
        public Transform DashEffectTransform => dashEffectTransform;
        
        [SerializeField] private Transform jumpEffectTransform;
        public Transform JumpEffectTransform => jumpEffectTransform;
        
        [SerializeField] private Transform bloodEffectTransform;
        public Transform BloodEffectTransform => bloodEffectTransform;

        [Header("Audio Clips")]

        [SerializeField] private AudioClip jumpAudioClip;
        public AudioClip JumpAudioClip => jumpAudioClip;

        [SerializeField] private AudioClip dashAudioClip;
        public AudioClip DashAudioClip => dashAudioClip;
        
        [SerializeField] private AudioClip dodgeAudioClip;
        public AudioClip DodgeAudioClip => dodgeAudioClip;
        
        [Header("Display References")]
        
        [SerializeField] private CharacterDisplay characterDisplay;
        public CharacterDisplay CharacterDisplay => characterDisplay;
        
        [SerializeField] private GunSprite gunSprite;
        public GunSprite GunSprite => gunSprite;
        
        [SerializeField] private SwordSprite swordSprite;
        public SwordSprite SwordSprite => swordSprite;
    }
}
