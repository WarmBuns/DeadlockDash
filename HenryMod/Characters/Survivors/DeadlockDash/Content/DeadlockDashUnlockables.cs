using DeadlockDash.Survivors.DeadlockDash.Achievements;
using RoR2;
using UnityEngine;

namespace DeadlockDash.Survivors.DeadlockDash
{
    public static class DeadlockDashUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                DeadlockDashMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(DeadlockDashMasteryAchievement.identifier),
                DeadlockDashSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
