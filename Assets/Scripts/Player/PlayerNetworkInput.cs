using Fusion;

namespace Player
{
    public struct PlayerNetworkInput : INetworkInput
    {
        public float HorizontalInput;
        public NetworkButtons NetworkButtons;
    }
}