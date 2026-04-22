using DeadlockDash.Survivors.DeadlockDash.SkillStates;

namespace DeadlockDash.Survivors.DeadlockDash
{
    public static class DeadlockDashStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(Shoot));

            Modules.Content.AddEntityState(typeof(Roll));

            Modules.Content.AddEntityState(typeof(ThrowBomb));
        }
    }
}
