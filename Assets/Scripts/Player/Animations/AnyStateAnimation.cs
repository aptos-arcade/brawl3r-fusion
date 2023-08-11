namespace Player.Animations
{
    public enum Rig { Body, Legs }

    public class AnyStateAnimation
    {
        public Rig AnimationRig { get; }

        public Animations[] HigherPriority { get; }

        public string Name { get; }
        
        public bool HoldOnEnd { get; }
        
        public Animations AnimationsEnum { get; }

        public AnyStateAnimation(Rig rig, bool holdOnEnd, string name, Animations animationsEnum, params Animations[] higherPriority)
        {
            AnimationRig = rig;
            Name = name;
            HigherPriority = higherPriority;
            HoldOnEnd = holdOnEnd;
            AnimationsEnum = animationsEnum;
        }


    }
}