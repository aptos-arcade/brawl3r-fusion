using System;
using Global;
using UnityEngine;

namespace Player.Animations
{
    [Serializable]
    public class PlayerAnimations
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private AnyStateAnimator animator;

        public void AddAnimations()
        {
            AnyStateAnimation[] animations = {
                new(Rig.Body, false, "Body_Idle", Animations.BodyIdle,
                    Animations.BodyAttack, Animations.BodyJump, Animations.BodyShield, Animations.BodyDodge, Animations.BodyDash),
                new(Rig.Body,false, "Body_Walk", Animations.BodyWalk,
                    Animations.BodyAttack, Animations.BodyJump, Animations.BodyShield, Animations.BodyDodge, Animations.BodyDash),
                new(Rig.Body,false, "Body_Jump", Animations.BodyJump,
                    Animations.BodyAttack, Animations.BodyDodge, Animations.BodyDash, Animations.BodyDoubleJump),
                new(Rig.Body,false, "Body_Double_Jump", Animations.BodyDoubleJump,
                    Animations.BodyDodge, Animations.BodyAttack, Animations.BodyFastFall),
                new(Rig.Body,false, "Body_Fall", Animations.BodyFall,
                    Animations.BodyAttack, Animations.BodyJump, Animations.BodyDoubleJump, Animations.BodyShield, Animations.BodyDodge, Animations.BodyDash, Animations.BodyFastFall, Animations.BodyStunned),
                new(Rig.Body, false,"Body_Attack", Animations.BodyAttack,
                    Animations.BodyShield, Animations.BodyDodge, Animations.BodyFastFall),
                new(Rig.Body,true, "Body_Shield", Animations.BodyShield,
                    Animations.BodyAttack, Animations.BodyJump, Animations.BodyDoubleJump, Animations.BodyDodge, Animations.BodyDash, Animations.BodyFastFall),
                new(Rig.Body, false,"Body_Dodge", Animations.BodyDodge,
                    Animations.BodyAttack, Animations.BodyDash, Animations.BodyFastFall),
                new(Rig.Body, false,"Body_Dash", Animations.BodyDash,
                    Animations.BodyAttack, Animations.BodyShield, Animations.BodyDodge, Animations.BodyFastFall),
                new(Rig.Body, true,"Body_FastFall", Animations.BodyFastFall,
                    Animations.BodyAttack),
                new(Rig.Body, true, "Body_Stunned", Animations.BodyStunned),
        
                new(Rig.Legs, false,"Legs_Idle", Animations.LegsIdle,
                    Animations.LegsAttack, Animations.LegsJump, Animations.LegsShield, Animations.LegsDodge, Animations.LegsDash),
                new(Rig.Legs, false,"Legs_Walk", Animations.LegsWalk,
                    Animations.LegsShield, Animations.LegsDodge, Animations.LegsJump, Animations.LegsDash, Animations.LegsAttack),
                new(Rig.Legs, false,"Legs_Jump", Animations.LegsJump,
                    Animations.LegsDoubleJump, Animations.LegsDodge, Animations.LegsDash),
                new(Rig.Legs, false,"Legs_Double_Jump", Animations.LegsDoubleJump,
                    Animations.LegsDodge, Animations.LegsAttack, Animations.LegsFastFall),
                new(Rig.Legs, false,"Legs_Fall", Animations.LegsFall,
                    Animations.LegsAttack, Animations.LegsDoubleJump, Animations.LegsShield, Animations.LegsDodge, Animations.LegsDash, Animations.LegsFastFall, Animations.LegsStunned),
                new(Rig.Legs, false,"Legs_Attack", Animations.LegsAttack,
                    Animations.LegsShield, Animations.LegsDodge, Animations.LegsFastFall),
                new(Rig.Legs, true,"Legs_Shield", Animations.LegsShield,
                    Animations.LegsAttack, Animations.LegsJump, Animations.LegsDoubleJump, Animations.LegsDodge, Animations.LegsDash, Animations.LegsFastFall),
                new(Rig.Legs, false,"Legs_Dodge", Animations.LegsDodge,
                    Animations.LegsAttack, Animations.LegsDash, Animations.LegsFastFall),
                new(Rig.Legs, false,"Legs_Dash", Animations.LegsDash,
                    Animations.LegsAttack, Animations.LegsShield, Animations.LegsDodge, Animations.LegsFastFall),
                new(Rig.Legs, true,"Legs_FastFall", Animations.LegsFastFall,
                    Animations.LegsAttack),
                new(Rig.Legs, true, "Legs_Stunned", Animations.LegsStunned)
            };
        
            animator.AnimationTriggerEvent += HandleAnimation;
            animator.AddAnimations(animations);
        }

        public void TryPlayAnimation(Animations bodyAnimation, Animations legsAnimation)
        {
            animator.TryPlayAnimation(bodyAnimation);
            animator.TryPlayAnimation(legsAnimation);
        }

        public void OnAnimationDone(Animations bodyAnimation, Animations legsAnimation)
        {
            animator.OnAnimationDone(bodyAnimation);
            animator.OnAnimationDone(legsAnimation);
        }

        public bool IsCurrentBodyAnimation(Animations animation)
        {
            return animator.CurrentAnimationBody == animation;
        }

        public void SetWeapon(Global.Weapons weapon)
        {
            animator.SetWeapon((int)weapon);
        }
        
        public void SetAttackDirection(Directions attackDirection)
        {
            animator.SetAttackDirection(attackDirection);
        }
        
        public void OnDeath()
        {
            animator.OnDeath();
        }

        private void HandleAnimation(string animation)
        {
            switch (animation)
            {
                case "Shoot":
                    player.PlayerActions.Shoot();
                    break;
                case "Side_Melee":
                    player.PlayerActions.SideMelee();
                    break;
                case "Up_Melee":
                    player.PlayerActions.UpMelee();
                    break;
                case "Jab_Melee":
                    player.PlayerActions.JabMelee();
                    break;
                case "Down_Melee":
                    player.PlayerActions.DownMelee();
                    break;
                case "Jump":
                    player.PlayerActions.Jump(false);
                    break;
                case "Double_Jump":
                    player.PlayerActions.Jump(true);
                    break;
                case "Shield":
                    player.PlayerActions.TriggerShield(true);
                    break;
                case "Dodge":
                    player.StartCoroutine(player.PlayerActions.DodgeCoroutine());
                    break;
                case "Dash":
                    player.PlayerActions.Dash();
                    break;
                case "FastFall":
                    player.PlayerActions.FastFall();
                    break;
            }
        }
    }
}