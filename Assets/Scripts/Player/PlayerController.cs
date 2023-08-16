using Fusion;
using Gameplay;
using Player.NetworkBehaviours;
using Player.PlayerModules;
using Player.SerializedProperties;
using UnityEngine;
using Utilities;

namespace Player
{
    public class PlayerController : NetworkBehaviour
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
        
        // modules

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
            
            PlayerAttacks = new PlayerAttacks(this);
            PlayerUtilities = new PlayerUtilities(this);
            PlayerProperties = new PlayerProperties(this);
            PlayerVisualController = new PlayerVisualController(this);
            PlayerMovementController = new PlayerMovementController(this);
            PlayerAudioController = new PlayerAudioController(this);
            PlayerAnimations = new PlayerAnimations(this);
            
            SetLocalObjects();

            if (Runner.IsServer)
            {
                PlayerRespawnController.StartRespawn();
            }
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

        public override void FixedUpdateNetwork()
        {
            if(PlayerNetworkState.IsDead) return;
            
            PlayerVisualController.HandleVisuals();
            
            PlayerUtilities.HandleTimers();
            PlayerUtilities.HandleAir();
            PlayerUtilities.HandleDeath();
            PlayerUtilities.HandleEnergy();
            
            PlayerAudioController.HandleAudio();
            
            PlayerMovementController.Move();
        }
    }
}
