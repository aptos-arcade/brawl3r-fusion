using Photon;

namespace Player.PlayerModules
{
    public class PlayerProperties
    {
        private readonly PlayerController player;
        
        public PlayerProperties(PlayerController player)
        {
            this.player = player;
        }
        
        public bool IsStunned => !player.PlayerNetworkState.StunTimer.ExpiredOrNotRunning(player.Runner);
        
        public bool IsDodging => !player.PlayerNetworkState.DodgeTimer.ExpiredOrNotRunning(player.Runner);
        
        public bool CanMove => !player.PlayerNetworkState.IsDead && !IsStunned && !IsDodging;
        
        public bool CanDodge => !player.PlayerNetworkState.DodgeCooldown.IsRunning;

        public bool IsDisabled => !player.PlayerNetworkState.StunTimer.ExpiredOrNotRunning(player.Runner);
        
        public bool IsOnGround => player.Runner.GetPhysicsScene2D().OverlapBox(
            player.PlayerReferences.GroundCheck.position, player.PlayerReferences.GroundCheck.localScale, 0,
            player.PlayerComponents.Ground);

        public bool IsOnPlatform => player.Runner.GetPhysicsScene2D().OverlapBox(
            player.PlayerReferences.GroundCheck.position, player.PlayerReferences.GroundCheck.localScale, 0,
            player.PlayerComponents.Platform);
        public bool IsGrounded => IsOnGround || IsOnPlatform;
        
        public bool IsFalling => player.PlayerComponents.RigidBody.velocity.y < 0 && !IsGrounded;
        
        public bool IsDashing => player.PlayerAnimations.IsCurrentBodyAnimation(Animations.Animations.BodyDash);
        
        public bool IsShielding => player.PlayerAnimations.IsCurrentBodyAnimation(Animations.Animations.BodyShield);

        public bool IsAcceptingInput => !player.PlayerNetworkState.IsDead && !player.PlayerProperties.IsDisabled
                                                                          && MatchManager.Instance.GameState !=
                                                                          GameState.MatchOver;
    }
}