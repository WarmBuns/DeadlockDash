using RoR2;
using UnityEngine;

namespace DeadlockDash.Content
{
    internal static class DeadlockDashBuffs
    {
        internal static BuffDef armorBuff;
        internal static BuffDef bdDeadlockDashCooldown;
        internal static BuffDef bdDeadlockDashReady;

        internal static void Init()
        {
            BuffDef hiddenInvincibility = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility");
            if (!hiddenInvincibility)
            {
                Log.Error("Failed to load BuffDefs/HiddenInvincibility for DeadlockDash buff icons.");
                return;
            }

            Sprite dashBuffIcon = hiddenInvincibility.iconSprite;

            armorBuff = Modules.Content.CreateAndAddBuff(
                "DeadlockDashArmorBuff",
                dashBuffIcon,
                Color.white,
                false,
                false);

            bdDeadlockDashReady = Modules.Content.CreateAndAddBuff(
                "bdDeadlockDashReady",
                dashBuffIcon,
                Color.white,
                true,
                false);
            bdDeadlockDashReady.ignoreGrowthNectar = true;

            bdDeadlockDashCooldown = Modules.Content.CreateAndAddBuff(
                "bdDeadlockDashCooldown",
                dashBuffIcon,
                Color.gray,
                true,
                false);
            bdDeadlockDashCooldown.ignoreGrowthNectar = true;
            bdDeadlockDashCooldown.isCooldown = true;

            Log.Info("Initialized DeadlockDash buff definitions.");
        }
    }
}
