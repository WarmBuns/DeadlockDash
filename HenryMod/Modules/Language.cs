using R2API;
using System;
using System.Collections.Generic;
using System.IO;

namespace DeadlockDash.Modules
{
    internal static class Language
    {
        internal static string TokensOutput = string.Empty;

        internal static bool usingLanguageFolder = false;

        internal static bool printingEnabled = false;

        internal static void Init()
        {
            if (usingLanguageFolder)
            {
                RoR2.Language.collectLanguageRootFolders += CollectLanguageRootFolders;
            }
        }

        private static void CollectLanguageRootFolders(List<string> folders)
        {
            if (DeadlockSkillsPlugin.instance == null)
            {
                Log.Warning("CollectLanguageRootFolders called before plugin instance was initialized.");
                return;
            }

            string pluginLocation = DeadlockSkillsPlugin.instance.Info?.Location;
            if (string.IsNullOrEmpty(pluginLocation))
            {
                Log.Warning("CollectLanguageRootFolders could not resolve plugin location.");
                return;
            }

            string pluginDirectory = Path.GetDirectoryName(pluginLocation);
            if (string.IsNullOrEmpty(pluginDirectory))
            {
                Log.Warning($"CollectLanguageRootFolders could not resolve plugin directory from '{pluginLocation}'.");
                return;
            }

            string path = Path.Combine(pluginDirectory, "Language");
            if (Directory.Exists(path))
            {
                folders.Add(path);
            }
            else
            {
                Log.Warning($"Configured language folder does not exist at '{path}'.");
            }
        }

        internal static void Add(string token, string text)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Log.Warning("Attempted to add language text with an empty token.");
                return;
            }

            if (!usingLanguageFolder)
            {
                LanguageAPI.Add(token, text);
            }

            if (!printingEnabled)
            {
                return;
            }

            TokensOutput += $"\n    \"{token}\" : \"{text.Replace(Environment.NewLine, "\\n").Replace("\n", "\\n")}\",";
        }

        internal static void PrintOutput(string fileName = "")
        {
            if (!printingEnabled)
            {
                return;
            }

            string strings = $"{{\n    strings:\n    {{{TokensOutput}\n    }}\n}}";
            Log.Message($"{fileName}: \n{strings}");

            if (!string.IsNullOrEmpty(fileName))
            {
                if (DeadlockSkillsPlugin.instance == null)
                {
                    Log.Warning($"PrintOutput('{fileName}') was called before plugin instance initialization.");
                    TokensOutput = string.Empty;
                    return;
                }

                string pluginLocation = DeadlockSkillsPlugin.instance.Info?.Location;
                if (string.IsNullOrEmpty(pluginLocation))
                {
                    Log.Warning($"PrintOutput('{fileName}') could not resolve plugin location.");
                    TokensOutput = string.Empty;
                    return;
                }

                DirectoryInfo parentDirectory = Directory.GetParent(pluginLocation);
                if (parentDirectory == null)
                {
                    Log.Warning($"PrintOutput('{fileName}') could not resolve plugin parent directory from '{pluginLocation}'.");
                    TokensOutput = string.Empty;
                    return;
                }

                string path = Path.Combine(parentDirectory.FullName, "Language", "en", fileName);
                File.WriteAllText(path, strings);
            }

            TokensOutput = string.Empty;
        }
    }
}
