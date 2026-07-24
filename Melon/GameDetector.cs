using System;
using System.IO;
using Microsoft.Win32;

namespace MelonLoaderDownloader
{
    public static class GameDetector
    {
        private static readonly string[] UnityMarkers = { "UnityPlayer.dll", "GameAssembly.dll" };

        public static string[] FindInstalledGames()
        {
            var results = new System.Collections.Generic.List<string>();

            // Check Steam library paths from registry
            string steamPath = GetSteamPath();
            if (!string.IsNullOrEmpty(steamPath))
            {
                ScanSteamLibraries(steamPath, results);
            }

            return results.ToArray();
        }

        private static string GetSteamPath()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam");
                return key?.GetValue("InstallPath") as string;
            }
            catch { return null; }
        }

        private static void ScanSteamLibraries(string steamPath, System.Collections.Generic.List<string> results)
        {
            string defaultLib = Path.Combine(steamPath, "steamapps", "common");
            ScanDirectory(defaultLib, results);
        }

        private static void ScanDirectory(string dir, System.Collections.Generic.List<string> results)
        {
            if (!Directory.Exists(dir)) return;
            foreach (var subdir in Directory.GetDirectories(dir))
            {
                foreach (var marker in UnityMarkers)
                {
                    if (File.Exists(Path.Combine(subdir, marker)))
                    {
                        results.Add(subdir);
                        break;
                    }
                }
            }
        }
    }
}
