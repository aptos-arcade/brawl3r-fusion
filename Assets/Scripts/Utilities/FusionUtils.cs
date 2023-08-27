using Fusion;
using Photon;

namespace Utilities
{
    public static class FusionUtils
    {
        public static bool IsLocalPlayer(NetworkObject networkObject)
        {
            return networkObject.HasStateAuthority;
        }
        
        public static bool IsSameTeam(NetworkObject other)
        {
            return MatchManager.Instance.SessionPlayers[other.InputAuthority].Team == 
                   MatchManager.Instance.LocalPlayerInfo.Team;
        }
    }
}