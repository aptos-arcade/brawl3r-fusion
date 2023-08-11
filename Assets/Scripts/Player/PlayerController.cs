using Fusion;
using Gameplay;
using Photon;
using Player.Animations;
using UnityEngine;
using Utilities;

namespace Player
{
    public class PlayerController : NetworkBehaviour, IBeforeUpdate
    {

        [SerializeField] private PlayerStats.PlayerStats playerStats;
        public PlayerStats.PlayerStats PlayerStats => playerStats;

        [SerializeField] private PlayerComponents playerComponents;
        public PlayerComponents PlayerComponents => playerComponents;

        [SerializeField] private PlayerReferences playerReferences;
        public PlayerReferences PlayerReferences => playerReferences;
        
        [SerializeField] private PlayerAnimations playerAnimations;
        public PlayerAnimations PlayerAnimations => playerAnimations;
        
        [SerializeField] private PlayerCameraController playerCameraController;
        public PlayerCameraController PlayerCameraController => playerCameraController;
        
        [SerializeField] private PlayerRespawnController playerRespawnController;
        public PlayerRespawnController PlayerRespawnController => playerRespawnController;
        
        [SerializeField] private PlayerVisualController playerVisualController;
        public PlayerVisualController PlayerVisualController => playerVisualController;
        
        [SerializeField] private PlayerState playerState;
        public PlayerState PlayerState => playerState;

        public PlayerActions PlayerActions { get; private set; }

        public PlayerUtilities PlayerUtilities { get; private set; }
        
        // Start is called before the first frame update
        public override void Spawned()
        {
            PlayerActions = new PlayerActions(this);
            PlayerUtilities = new PlayerUtilities(this);
            
            playerAnimations.AddAnimations();
            
            var playerData = MatchManager.Instance.SessionPlayers[Object.InputAuthority];
            var tagColor = PlayerUtilities.IsSameTeam(Object)
            ? new Color(0.6588235f, 0.8078431f, 1f)
            : Color.red;
            playerReferences.NameTag.color = tagColor;
            playerReferences.CollectionTag.color = tagColor;
            playerReferences.NameTag.text = playerData.Name.ToString();
            playerReferences.CollectionTag.text =
            Characters.Characters.AvailableCharacters[playerData.Character].DisplayName;
            playerReferences.DamageDisplay.text = (PlayerState.DamageMultiplier - 1) * 100 + "%";
            playerReferences.PlayerShield.SetPlayer(this);
            
            playerComponents.RigidBody.gravityScale = playerStats.GravityScale;
            
            SetLocalObjects();
            playerRespawnController.StartRespawn();
        }
        
        private void SetLocalObjects()
        {
            if (!FusionUtils.IsLocalPlayer(Object))
            {
                GetComponent<NetworkRigidbody2D>().InterpolationDataSource = InterpolationDataSources.Snapshots;
            }
            else
            {
                GameManager.Instance.Player = this;
            }
        }

        public void BeforeUpdate()
        {
            if(!FusionUtils.IsLocalPlayer(Object) || !PlayerUtilities.IsAcceptingInput) return;
            PlayerState.HorizontalInput = Input.GetAxisRaw("Horizontal");
        }

        public override void FixedUpdateNetwork()
        {
            if(PlayerState.IsDead) return;
            
            PlayerUtilities.HandleInput();
            PlayerUtilities.HandleAir();
            PlayerUtilities.HandleDeath();
            PlayerUtilities.HandleEnergy();
            
            PlayerActions.Move();
            PlayerVisualController.UpdateScaleTransforms();
        }
    }
}
