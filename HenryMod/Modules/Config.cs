using BepInEx.Configuration;
using RoR2.Skills;
using UnityEngine;

namespace DeadlockDash.Modules
{
    public static class Config
    {
        public static ConfigEntry<KeyboardShortcut> DashKeybind { get; private set; }
        public static ConfigEntry<float> DashCooldown { get; private set; }
        public static ConfigEntry<int> DashStocks { get; private set; }

        public static void Init()
        {
            ConfigFile config = DeadlockSkillsPlugin.instance.Config;
            DashKeybind = config.Bind("Keybinds", "Deadlock Dash", new KeyboardShortcut(KeyCode.C), "Custom keybind used to trigger Deadlock Dash.");
            DashCooldown = config.Bind("Gameplay", "Deadlock Dash Cooldown", 4f, "Base cooldown used by the injected Deadlock Dash skill.");
            DashStocks = config.Bind("Gameplay", "Deadlock Dash Starting Stocks", 2, "Starting and maximum stock count used by the injected Deadlock Dash skill.");
        }

        public static void ApplyToSkill(SkillDef skillDef)
        {
            if (skillDef != null)
            {
                skillDef.baseRechargeInterval = DashCooldown.Value;
            }
        }

        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            return GetKeyPressed(entry.Value);
        }

        public static bool GetKeyPressed(KeyboardShortcut entry)
        {
            foreach (var item in entry.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.MainKey);
        }
    }
}
