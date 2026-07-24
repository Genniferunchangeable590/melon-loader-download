using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace MelonLoaderDownloader
{
    public class Installer
    {
        public async Task InstallAsync(string gamePath, string melonLoaderZip, IProgress<string> log = null)
        {
            log?.Report("Extracting MelonLoader…");
            await Task.Run(() =>
            {
                using var archive = ZipFile.OpenRead(melonLoaderZip);
                foreach (var entry in archive.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name)) continue;
                    string dest = Path.Combine(gamePath, entry.FullName);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                    entry.ExtractToFile(dest, overwrite: true);
                }
            });

            string modsDir = Path.Combine(gamePath, "Mods");
            Directory.CreateDirectory(modsDir);
            log?.Report($"Mods folder ready: {modsDir}");

            log?.Report("MelonLoader installed successfully.");
        }

        public void Uninstall(string gamePath, IProgress<string> log = null)
        {
            foreach (var target in new[] { "version.dll", "MelonLoader", "dobby.dll" })
            {
                string fullPath = Path.Combine(gamePath, target);
                if (File.Exists(fullPath)) { File.Delete(fullPath); log?.Report($"Removed {target}"); }
                else if (Directory.Exists(fullPath)) { Directory.Delete(fullPath, true); log?.Report($"Removed {target}/"); }
            }
        }
    }
}
