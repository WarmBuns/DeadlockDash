using RoR2;
using UnityEngine;

namespace DeadlockDash.Content
{
    internal static class DeadlockSkillsBuffs
    {
        internal static BuffDef dashBuff;
        //internal static BuffDef bdDeadlockDashCooldown;
        //internal static BuffDef bdDeadlockDashReady;

        internal static void Init()
        {
            BuffDef hiddenInvincibility = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility");
            if (!hiddenInvincibility)
            {
                Log.Error("Failed to load BuffDefs/HiddenInvincibility for DeadlockDash buff icons.");
                return;
            }

            Sprite dashBuffIcon = hiddenInvincibility.iconSprite;

            dashBuff = Modules.Content.CreateAndAddBuff(
                "DeadlockDashBuff",
                dashBuffIcon,
                Color.white,
                false,
                false);





        }
    }
}
