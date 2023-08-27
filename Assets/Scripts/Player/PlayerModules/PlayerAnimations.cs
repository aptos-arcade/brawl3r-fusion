using Global;
using Player.Animations;

namespace Player.PlayerModules
{
    public class PlayerAnimations
    {
        
        private readonly PlayerController player;
        
        public PlayerAnimations(PlayerController player)
        {
            this.player = player;
            AddAnimations();
        }

        private void AddAnimations()
        {
            AnyStateAnimation[] animations = {
                new(Rig.Body, false, "Body_Idle", Animations.Animations.BodyIdle,
                    Animations.Animations.BodyAttack, Animations.Animations.BodyJump, Animations.Animations.BodyShield, Animations.Animations.BodyDodge, Animations.Animations.BodyDash),
                new(Rig.Body,false, "Body_Walk", Animations.Animations.BodyWalk,
                    Animations.Animations.BodyAttack, Animations.Animations.BodyJump, Animations.Animations.BodyShield, Animations.Animations.BodyDodge, Animations.Animations.BodyDash),
                new(Rig.Body,false, "Body_Jump", Animations.Animations.BodyJump,
                    Animations.Animations.BodyAttack, Animations.Animations.BodyDodge, Animations.Animations.BodyDash, Animations.Animations.BodyDoubleJump),
                new(Rig.Body,false, "Body_Double_Jump", Animations.Animations.BodyDoubleJump,
                    Animations.Animations.BodyDodge, Animations.Animations.BodyAttack, Animations.Animations.BodyFastFall),
                new(Rig.Body,false, "Body_Fall", Animations.Animations.BodyFall,
                    Animations.Animations.BodyAttack, Animations.Animations.BodyJump, Animations.Animations.BodyDoubleJump, Animations.Animations.BodyShield, Animations.Animations.BodyDodge, Animations.Animations.BodyDash, Animations.Animations.BodyFastFall, Animations.Animations.BodyStunned),
                new(Rig.Body, false,"Body_Attack", Animations.Animations.BodyAttack,
                    Animations.Animations.BodyShield, Animations.Animations.BodyDodge, Animations.Animations.BodyFastFall),
                new(Rig.Body,true, "Body_Shield", Animations.Animations.BodyShield,
                    Animations.Animations.BodyAttack, Animations.Animations.BodyJump, Animations.Animations.BodyDoubleJump, Animations.Animations.BodyDodge, Animations.Animations.BodyDash, Animations.Animations.BodyFastFall),
                new(Rig.Body, false,"Body_Dodge", Animations.Animations.BodyDodge,
                    Animations.Animations.BodyAttack, Animations.Animations.BodyDash, Animations.Animations.BodyFastFall),
                new(Rig.Body, false,"Body_Dash", Animations.Animations.BodyDash,
                    Animations.Animations.BodyAttack, Animations.Animations.BodyShield, Animations.Animations.BodyDodge, Animations.Animations.BodyFastFall),
                new(Rig.Body, true,"Body_FastFall", Animations.Animations.BodyFastFall,
                    Animations.Animations.BodyAttack),
                new(Rig.Body, true, "Body_Stunned", Animations.Animations.BodyStunned),
        
                new(Rig.Legs, false,"Legs_Idle", Animations.Animations.LegsIdle,
                    Animations.Animations.LegsAttack, Animations.Animations.LegsJump, Animations.Animations.LegsShield, Animations.Animations.LegsDodge, Animations.Animations.LegsDash),
                new(Rig.Legs, false,"Legs_Walk", Animations.Animations.LegsWalk,
                    Animations.Animations.LegsShield, Animations.Animations.LegsDodge, Animations.Animations.LegsJump, Animations.Animations.LegsDash, Animations.Animations.LegsAttack),
                new(Rig.Legs, false,"Legs_Jump", Animations.Animations.LegsJump,
                    Animations.Animations.LegsDoubleJump, Animations.Animations.LegsDodge, Animations.Animations.LegsDash),
                new(Rig.Legs, false,"Legs_Double_Jump", Animations.Animations.LegsDoubleJump,
                    Animations.Animations.LegsDodge, Animations.Animations.LegsAttack, Animations.Animations.LegsFastFall),
                new(Rig.Legs, false,"Legs_Fall", Animations.Animations.LegsFall,
                    Animations.Animations.LegsAttack, Animations.Animations.LegsDoubleJump, Animations.Animations.LegsShield, Animations.Animations.LegsDodge, Animations.Animations.LegsDash, Animations.Animations.LegsFastFall, Animations.Animations.LegsStunned),
                new(Rig.Legs, false,"Legs_Attack", Animations.Animations.LegsAttack,
                    Animations.Animations.LegsShield, Animations.Animations.LegsDodge, Animations.Animations.LegsFastFall),
                new(Rig.Legs, true,"Legs_Shield", Animations.Animations.LegsShield,
                    Animations.Animations.LegsAttack, Animations.Animations.LegsJump, Animations.Animations.LegsDoubleJump, Animations.Animations.LegsDodge, Animations.Animations.LegsDash, Animations.Animations.LegsFastFall),
                new(Rig.Legs, false,"Legs_Dodge", Animations.Animations.LegsDodge,
                    Animations.Animations.LegsAttack, Animations.Animations.LegsDash, Animations.Animations.LegsFastFall),
                new(Rig.Legs, false,"Legs_Dash", Animations.Animations.LegsDash,
                    Animations.Animations.LegsAttack, Animations.Animations.LegsShield, Animations.Animations.LegsDodge, Animations.Animations.LegsFastFall),
                new(Rig.Legs, true,"Legs_FastFall", Animations.Animations.LegsFastFall,
                    Animations.Animations.LegsAttack),
                new(Rig.Legs, true, "Legs_Stunned", Animations.Animations.LegsStunned)
            };
        
            player.PlayerReferences.Animator.AnimationTriggerEvent += HandleAnimationTrigger;
            player.PlayerReferences.Animator.AddAnimations(animations);
        }

        public void OnAnimationDone(Animations.Animations bodyAnimation, Animations.Animations legsAnimation)
        {
            player.PlayerReferences.Animator.OnAnimationDone(bodyAnimation);
            player.PlayerReferences.Animator.OnAnimationDone(legsAnimation);
        }

        public bool IsCurrentBodyAnimation(Animations.Animations animation)
        {
            return player.PlayerReferences.Animator.CurrentAnimationBody == animation;
        }

        public void SetWeapon(Global.Weapons weapon)
        {
            player.PlayerReferences.Animator.SetWeapon(weapon);
        }
        
        public void SetAttackDirection(Directions attackDirection)
        {
            player.PlayerReferences.Animator.SetAttackDirection(attackDirection);
        }
        
        public void OnDeath()
        {
            player.PlayerReferences.Animator.OnDeath();
        }

        private void HandleAnimationTrigger(string animation)
        {
            switch (animation)
            {
                case "Shoot":
                    player.PlayerReferences.Gun.Shoot();
                    break;
                case "Side_Melee":
                    player.PlayerReferences.Sword.Attack();
                    break;
                case "Up_Melee":
                    player.PlayerReferences.Sword.Attack();
                    break;
                case "Jab_Melee":
                    player.PlayerReferences.Sword.Attack();
                    break;
                case "Down_Melee":
                    player.PlayerReferences.Sword.Attack();
                    break;
                case "Jump":
                    player.PlayerMovementController.Jump(false);
                    break;
                case "Double_Jump":
                    player.PlayerMovementController.Jump(true);
                    break;
                case "Shield":
                    player.PlayerReferences.PlayerShield.TriggerShield(true);
                    break;
                case "Dodge":
                    player.PlayerMovementController.Dodge();
                    break;
                case "Dash":
                    player.PlayerMovementController.Dash();
                    break;
                case "FastFall":
                    player.PlayerMovementController.FastFall();
                    break;
            }
        }

        public void TryWalk()
        {
            TryPlayAnimation(Animations.Animations.BodyWalk, Animations.Animations.LegsWalk);
        }
        
        public void TryIdle()
        {
            TryPlayAnimation(Animations.Animations.BodyIdle, Animations.Animations.LegsIdle);
        }
        
        public void TryJump()
        {
            TryPlayAnimation(Animations.Animations.BodyJump, Animations.Animations.LegsJump);
        }
        
        public void TryDoubleJump()
        {
            TryPlayAnimation(Animations.Animations.BodyDoubleJump, Animations.Animations.LegsDoubleJump);
        }
        
        public void TryFall()
        {
            TryPlayAnimation(Animations.Animations.BodyFall, Animations.Animations.LegsFall);
        }
        
        public void TryAttack()
        {
            TryPlayAnimation(Animations.Animations.BodyAttack, Animations.Animations.LegsAttack);
        }
        
        public void TryShield()
        {
            TryPlayAnimation(Animations.Animations.BodyShield, Animations.Animations.LegsShield);
        }
        
        public void TryDodge()
        {
            TryPlayAnimation(Animations.Animations.BodyDodge, Animations.Animations.LegsDodge);
        }
        
        public void TryDash()
        {
            TryPlayAnimation(Animations.Animations.BodyDash, Animations.Animations.LegsDash);
        }
        
        public void TryFastFall()
        {
            TryPlayAnimation(Animations.Animations.BodyFastFall, Animations.Animations.LegsFastFall);
        }
        
        public void TryStunned()
        {
            TryPlayAnimation(Animations.Animations.BodyStunned, Animations.Animations.LegsStunned);
        }
        
        private void TryPlayAnimation(Animations.Animations bodyAnimation, Animations.Animations legsAnimation)
        {
            player.PlayerReferences.Animator.TryPlayAnimation(bodyAnimation);
            player.PlayerReferences.Animator.TryPlayAnimation(legsAnimation);
        }

    }
}