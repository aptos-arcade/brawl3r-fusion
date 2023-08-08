using Fusion;

namespace Utilities
{
    public static class PhotonUtilities
    {
        public static bool IsLocalPlayer(NetworkObject networkObject)
        {
            return networkObject.IsValid == networkObject.HasInputAuthority;
        }
    }
}