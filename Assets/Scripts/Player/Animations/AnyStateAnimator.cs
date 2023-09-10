using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Global;
using UnityEngine;

namespace Player.Animations
{
    public delegate void AnimationTriggerEvent(string animation);

    public class AnyStateAnimator : NetworkBehaviour
    {
        private Animator animator;
        
        private readonly Dictionary<Animations, AnyStateAnimation> animations = new();
        
        [Networked]
        [Capacity(22)]
        private NetworkDictionary<Animations, NetworkBool> ActiveAnimations { get; }

        [Networked] private Global.Weapons Weapon { get; set; }
        
        [Networked] private Directions AttackDirection { get; set; }

        [Networked] public Animations CurrentAnimationBody { get; private set; } = Animations.None;
        
        [Networked] private Animations CurrentAnimationLegs { get; set; } = Animations.None;
        
        public AnimationTriggerEvent AnimationTriggerEvent { get; set; }


        private static readonly int WeaponHash = Animator.StringToHash("Weapon");
        private static readonly int AttackDirectionHash = Animator.StringToHash("AttackDirection");

        public override void Spawned()
        {
            animator = GetComponent<Animator>();
        }

        public override void Render()
        {
            Animate();
        }

        public void OnDeath()
        {
            foreach (var key in animations.Keys)
            {
                animator.SetBool(animations[key].Name, false);
            }
            TryPlayAnimation(Animations.BodyIdle);
            TryPlayAnimation(Animations.LegsIdle);
        }

        public void AddAnimations(params AnyStateAnimation[] newAnimations)
        {
            foreach (var t in newAnimations)
            {
                animations.Add(t.AnimationsEnum, t);
                ActiveAnimations.Add(t.AnimationsEnum, false);
            }
        }

        public void TryPlayAnimation(Animations newAnimation)
        {
            if(!HasStateAuthority) return;
            var rig = animations[newAnimation].AnimationRig;
            switch (rig)
            {
                case Rig.Body:
                    PlayAnimation(CurrentAnimationBody);
                    break;
                case Rig.Legs:
                    PlayAnimation(CurrentAnimationLegs);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            void PlayAnimation(Animations currentAnimation)
            {
                if(currentAnimation == Animations.None)
                {
                    ActiveAnimations.Set(currentAnimation, true);
                    if(rig == Rig.Body)
                        CurrentAnimationBody = newAnimation;
                    else
                        CurrentAnimationLegs = newAnimation;
                }
                else if(
                    (currentAnimation != newAnimation 
                     && !animations[newAnimation].HigherPriority.Contains(currentAnimation))
                    || !ActiveAnimations[currentAnimation]
                    || (!animations[currentAnimation].HoldOnEnd
                        && animator.GetCurrentAnimatorStateInfo((int)rig).IsName(animations[currentAnimation].Name)
                        && animator.GetCurrentAnimatorStateInfo((int)rig).normalizedTime >= 1)
                )
                {
                    ActiveAnimations.Set(currentAnimation, false);
                    ActiveAnimations.Set(newAnimation, true);

                    if(rig == Rig.Body)
                        CurrentAnimationBody = newAnimation;
                    else
                        CurrentAnimationLegs = newAnimation;
                    
                }
            }
        }

        public void SetWeapon(Global.Weapons weapon)
        {
            Weapon = weapon;
        }

        public void SetAttackDirection(Directions direction)
        {
            AttackDirection = direction;
        }

        private void Animate()
        {
            foreach (var key in animations.Keys)
            {
                animator.SetBool(animations[key].Name, ActiveAnimations[key]);
            }
            animator.SetFloat(WeaponHash, (float)Weapon);
            animator.SetFloat(AttackDirectionHash, (float)AttackDirection);
        }

        public void OnAnimationDone(Animations doneAnimation)
        {
            ActiveAnimations.Set(doneAnimation, false);
        }

        public void OnAnimationTrigger(string startAnimation)
        {
            AnimationTriggerEvent?.Invoke(startAnimation);
        }
    }
}