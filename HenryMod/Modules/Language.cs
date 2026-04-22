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
            string path = Path.Combine(Path.GetDirectoryName(DeadlockDashPlugin.instance.Info.Location), "Language");
            if (Directory.Exists(path))
            {
                folders.Add(path);
            }
        }

        internal static void Add(string token, string text)
        {
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
                string path = Path.Combine(Directory.GetParent(DeadlockDashPlugin.instance.Info.Location).FullName, "Language", "en", fileName);
                File.WriteAllText(path, strings);
            }

            TokensOutput = string.Empty;
        }
    }
}
