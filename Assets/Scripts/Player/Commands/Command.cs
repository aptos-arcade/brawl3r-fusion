using UnityEngine;

namespace Player.Commands
{
    public abstract class Command
    {
        public KeyCode Key { get; }
        
        protected Command(KeyCode key)
        {
            Key = key;
        }

        public virtual void GetKeyDown() {}

        public virtual void GetKey() {}
        
        public virtual void GetKeyUp() {}
    }
}
