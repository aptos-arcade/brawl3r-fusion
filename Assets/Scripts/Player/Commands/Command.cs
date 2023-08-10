using UnityEngine;

namespace Player.Commands
{
    public abstract class Command
    {
        public KeyCode Key { get; }
        public InputButtons Button { get; }

        protected Command(KeyCode key, InputButtons button)
        {
            Key = key;
            Button = button;
        }

        public virtual void WasPressed() {}

        public virtual void WasHeld() {}
        
        public virtual void WasReleased() {}
    }
}
