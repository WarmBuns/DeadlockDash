using RoR2;
using UnityEngine;

namespace DeadlockDash.Content
{
    internal static class DeadlockDashBuffs
    {
        internal static BuffDef armorBuff;

        internal static void Init()
        {
            armorBuff = Modules.Content.CreateAndAddBuff(
                "DeadlockDashArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);
        }
    }
}
