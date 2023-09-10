using Fusion;
using Gameplay;
using Player.NetworkBehaviours;
using Player.PlayerModules;
using Player.SerializedProperties;
using UnityEngine;
using Utilities;

namespace Player
{
    public class PlayerController : NetworkBehaviour, IBeforeUpdate
    {
        
        // serialized fields

        [SerializeField] private PlayerStats.PlayerStats playerStats;
        public PlayerStats.PlayerStats PlayerStats => playerStats;

        [SerializeField] private PlayerComponents playerComponents;
        public PlayerComponents PlayerComponents => playerComponents;

        [SerializeField] private PlayerReferences playerReferences;
        public PlayerReferences PlayerReferences => playerReferences;

        // components

        public PlayerRespawnController PlayerRespawnController { get; private set; }
        
        public PlayerNetworkState PlayerNetworkState { get; private set; }
        
        public PlayerRpcs PlayerRpcs { get; private set; }
        
        // modules

        private PlayerInputController PlayerInputController { get; set; }

        public PlayerAttacks PlayerAttacks { get; private set; }

        public PlayerUtilities PlayerUtilities { get; private set; }
        
        public PlayerProperties PlayerProperties { get; private set; }
        
        public PlayerVisualController PlayerVisualController { get; private set; }
        
        public PlayerMovementController PlayerMovementController { get; private set; }
        
        public PlayerAudioController PlayerAudioController { get; private set; }
        
        public PlayerAnimations PlayerAnimations { get; private set; }

        // Start is called before the first frame update
        public override void Spawned()
        {
            PlayerRespawnController = GetComponent<PlayerRespawnController>();
            PlayerNetworkState = GetComponent<PlayerNetworkState>();
            PlayerRpcs = GetComponent<PlayerRpcs>();
            
            PlayerAttacks = new PlayerAttacks(this);
            PlayerUtilities = new PlayerUtilities(this);
            PlayerProperties = new PlayerProperties(this);
            PlayerVisualController = new PlayerVisualController(this);
            PlayerMovementController = new PlayerMovementController(this);
            PlayerAudioController = new PlayerAudioController(this);
            PlayerAnimations = new PlayerAnimations(this);
            PlayerInputController = new PlayerInputController(this);
            
            SetLocalObjects();

            PlayerRespawnController.StartRespawn();
        }
        
        private void SetLocalObjects()
        {
            if(FusionUtils.IsLocalPlayer(Object))
            {
                GameManager.Instance.Player = this;
            }
            else
            {
                GetComponent<NetworkRigidbody2D>().InterpolationDataSource = InterpolationDataSources.Snapshots;
            }
        }

        public void BeforeUpdate()
        {
            if (!HasStateAuthority) return;
            PlayerInputController.HandleInput();
        }

        public override void FixedUpdateNetwork()
        {
            if(PlayerNetworkState.IsDead) return;

            if (HasStateAuthority)
            {
                PlayerVisualController.HandleVisuals();
                PlayerUtilities.HandleTimers();
                PlayerUtilities.HandleAir();
                PlayerUtilities.HandleDeath();
                PlayerUtilities.HandleEnergy();
            }

            PlayerAudioController.HandleAudio();
            
            PlayerMovementController.Move();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            Destroy(gameObject);
        }
    }
}
