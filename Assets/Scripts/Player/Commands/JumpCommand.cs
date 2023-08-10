using UnityEngine;

namespace Player.Commands
{
    public class JumpCommand : Command
    {

        private readonly PlayerController player;

        public JumpCommand(PlayerController player, KeyCode key) : base(key, InputButtons.Jump)
        {
            this.player = player;
        }

        public override void WasPressed()
        {
            player.PlayerActions.TryJump();
        }
    }
}
