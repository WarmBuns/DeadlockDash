using DeadlockDashState = DeadlockDash.SkillStates.DeadlockDash;

namespace DeadlockDash.Content
{
    internal static class DeadlockDashStates
    {
        internal static void Init()
        {
            Modules.Content.AddEntityState(typeof(DeadlockDashState));
        }
    }
}
