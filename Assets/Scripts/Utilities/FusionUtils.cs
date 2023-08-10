using Fusion;

namespace Utilities
{
    public static class FusionUtils
    {
        public static bool IsLocalPlayer(NetworkObject networkObject)
        {
            return networkObject.IsValid == networkObject.HasInputAuthority;
        }
    }
}