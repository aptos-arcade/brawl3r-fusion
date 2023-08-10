using Fusion;

namespace Player
{
    public static class NetworkChangeHandlers
    {
        public static void HandleWeaponChanged(Changed<PlayerController> changed)
        {
            changed.Behaviour.PlayerActions.SwapWeapon();
        }
    }
}