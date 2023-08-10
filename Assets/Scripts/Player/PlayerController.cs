using Animations;
using Fusion;
using Gameplay;
using Photon;
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
        
        [SerializeField] private PlayerCameraController playerCameraController;
        public PlayerCameraController PlayerCameraController => playerCameraController;

        public PlayerActions PlayerActions { get; private set; }

        public PlayerUtilities PlayerUtilities { get; private set; }
        
        public PlayerState PlayerState { get; } = new();

        // Start is called before the first frame update
        public override void Spawned()
        {
            PlayerActions = new PlayerActions(this);
            PlayerUtilities = new PlayerUtilities(this);
            AddAnimations();
            
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
            GameManager.Instance.SpawnPlayer(this);
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

        private void AddAnimations()
        {
            AnyStateAnimation[] animations = {
                new(Rig.Body, false, "Body_Idle", "Body_Attack", "Body_Jump", "Body_Shield", "Body_Dodge", "Body_Dash"),
                new(Rig.Body,false, "Body_Walk", "Body_Attack", "Body_Jump", "Body_Shield", "Body_Dodge", "Body_Dash"),
                new(Rig.Body,false, "Body_Jump", "Body_Attack", "Body_Dodge", "Body_Dash", "Body_Double_Jump"),
                new(Rig.Body,false, "Body_Double_Jump", "Body_Dodge", "Body_Attack", "Body_FastFall"),
                new(Rig.Body,false, "Body_Fall", "Body_Attack", "Body_Jump", "Body_Double_Jump", "Body_Shield", "Body_Dodge", "Body_Dash", "Body_FastFall", "Body_Stunned"),
                new(Rig.Body, false,"Body_Attack", "Body_Shield", "Body_Dodge", "Body_FastFall"),
                new(Rig.Body,true, "Body_Shield", "Body_Attack", "Body_Jump", "Body_Double_Jump", "Body_Dodge", "Body_Dash", "Body_FastFall"),
                new(Rig.Body, false,"Body_Dodge", "Body_Attack", "Body_Dash", "Body_FastFall"),
                new(Rig.Body, false,"Body_Dash", "Body_Attack", "Body_Shield", "Body_Dodge", "Body_FastFall"),
                new(Rig.Body, true,"Body_FastFall", "Body_Attack"),
                new(Rig.Body, true, "Body_Stunned"),
        
                new(Rig.Legs, false,"Legs_Idle", "Legs_Attack", "Legs_Jump", "Legs_Shield", "Legs_Dodge", "Legs_Dash"),
                new(Rig.Legs, false,"Legs_Walk", "Legs_Shield", "Legs_Dodge", "Legs_Jump", "Legs_Dash", "Legs_Attack"),
                new(Rig.Legs, false,"Legs_Jump", "Legs_Double_Jump", "Legs_Dodge", "Legs_Dash"),
                new(Rig.Legs, false,"Legs_Double_Jump", "Legs_Dodge", "Legs_Attack", "Legs_FastFall"),
                new(Rig.Legs, false,"Legs_Fall", "Legs_Attack", "Legs_Double_Jump", "Legs_Shield", "Legs_Dodge", "Legs_Dash", "Legs_FastFall", "Legs_Stunned"),
                new(Rig.Legs, false,"Legs_Attack", "Legs_Shield", "Legs_Dodge", "Legs_FastFall"),
                new(Rig.Legs, true,"Legs_Shield", "Legs_Attack", "Legs_Jump", "Legs_Double_Jump", "Legs_Dodge", "Legs_Dash", "Legs_FastFall"),
                new(Rig.Legs, false,"Legs_Dodge", "Legs_Attack", "Legs_Dash", "Legs_FastFall"),
                new(Rig.Legs, false,"Legs_Dash", "Legs_Attack", "Legs_Shield", "Legs_Dodge", "Legs_FastFall"),
                new(Rig.Legs, true,"Legs_FastFall", "Legs_Attack"),
                new(Rig.Legs, true, "Legs_Stunned")
            };
        
            playerComponents.Animator.AnimationTriggerEvent += PlayerUtilities.HandleAnimation;
            playerComponents.Animator.AddAnimations(animations);
        }
        
        public void BeforeUpdate()
        {
            if(!FusionUtils.IsLocalPlayer(Object) || !PlayerUtilities.IsAcceptingInput) return;
            PlayerState.HorizontalInput = Input.GetAxisRaw("Horizontal");
        }

        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            if(PlayerState.IsDead) return;
            PlayerUtilities.HandleInput();
            PlayerUtilities.HandleAir();
            PlayerUtilities.HandleDeath();
            PlayerUtilities.HandleEnergy();
            PlayerActions.Move(transform);
        }
    }
}
