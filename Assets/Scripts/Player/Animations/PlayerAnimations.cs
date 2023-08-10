using System;
using Animations;
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
                new(Rig.Body, false, "Body_Idle", "Body_Attack", "Body_Jump", "Body_Shield", "Body_Dodge", "Body_Dash"),
                new(Rig.Body,false, "Body_Walk", "Body_Attack", "Body_Jump", "Body_Shield", "Body_Dodge", "Body_Dash"),
                new(Rig.Body,false, "Body_Jump", "Body_Attack", "Body_Dodge", "Body_Dash", "Body_Double_Jump"),
                new(Rig.Body,false, "Body_Double_Jump", "Body_Dodge", "Body_Attack", "Body_FastFall"),
                new(Rig.Body,false, "Body_Fall", "Body_Attack", "Body_Jump", "Body_Double_Jump", "Body_Shield", "Body_Dodge", "Body_Dash", "Body_FastFall", "Body_Stunned"),
                new(Rig.Body, false,"Body_Attack", "Body_Shield", "Body_Dodge", "Body_FastFall"),
                new(Rig.Body,true, "Body_Shield", "Body_Attack", "Body_Jump", "Body_Double_Jump", "Body_Dodge", "Body_Dash", "Body_FastFall"),
                new(Rig.Body, false,"Body_Dodge", "Body_Attack", "Body_Dash", "Body_FastFall"),
                new(Rig.Body, false,"Body_Dash", "Body_Attack", "Body_Shield", "Body_Dodge", "Body_FastFall"),
                new(Rig.Body, true,"Body_FastFall", "Body_Attack"),
                new(Rig.Body, true, "Body_Stunned"),
        
                new(Rig.Legs, false,"Legs_Idle", "Legs_Attack", "Legs_Jump", "Legs_Shield", "Legs_Dodge", "Legs_Dash"),
                new(Rig.Legs, false,"Legs_Walk", "Legs_Shield", "Legs_Dodge", "Legs_Jump", "Legs_Dash", "Legs_Attack"),
                new(Rig.Legs, false,"Legs_Jump", "Legs_Double_Jump", "Legs_Dodge", "Legs_Dash"),
                new(Rig.Legs, false,"Legs_Double_Jump", "Legs_Dodge", "Legs_Attack", "Legs_FastFall"),
                new(Rig.Legs, false,"Legs_Fall", "Legs_Attack", "Legs_Double_Jump", "Legs_Shield", "Legs_Dodge", "Legs_Dash", "Legs_FastFall", "Legs_Stunned"),
                new(Rig.Legs, false,"Legs_Attack", "Legs_Shield", "Legs_Dodge", "Legs_FastFall"),
                new(Rig.Legs, true,"Legs_Shield", "Legs_Attack", "Legs_Jump", "Legs_Double_Jump", "Legs_Dodge", "Legs_Dash", "Legs_FastFall"),
                new(Rig.Legs, false,"Legs_Dodge", "Legs_Attack", "Legs_Dash", "Legs_FastFall"),
                new(Rig.Legs, false,"Legs_Dash", "Legs_Attack", "Legs_Shield", "Legs_Dodge", "Legs_FastFall"),
                new(Rig.Legs, true,"Legs_FastFall", "Legs_Attack"),
                new(Rig.Legs, true, "Legs_Stunned")
            };
        
            animator.AnimationTriggerEvent += HandleAnimation;
            animator.AddAnimations(animations);
        }

        public void TryPlayAnimation(string animation)
        {
            animator.TryPlayAnimation($"Body_{animation}");
            animator.TryPlayAnimation($"Legs_{animation}");
        }

        public void OnAnimationDone(string animation)
        {
            animator.OnAnimationDone($"Body_{animation}");
            animator.OnAnimationDone($"Legs_{animation}");
        }
        
        public bool IsCurrentBodyAnimation(string animation)
        {
            return animator.CurrentAnimationBody == $"Body_{animation}";
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