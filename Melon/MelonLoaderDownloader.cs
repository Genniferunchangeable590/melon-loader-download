using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MelonLoaderDownloader
{
    public class MelonLoaderDownloader
    {
        private static readonly HttpClient Http = new HttpClient();
        private const string API = "https://api.github.com/repos/LavaGang/MelonLoader/releases/latest";

        static MelonLoaderDownloader()
        {
            Http.DefaultRequestHeaders.Add("User-Agent", "MelonLoaderDownloader");
        }

        public async Task<(string version, string url)> GetLatestAsync()
        {
            var json = await Http.GetStringAsync(API);
            var tag = Regex.Match(json, "\"tag_name\":\\s*\"([^\"]+)\"").Groups[1].Value;
            var url = Regex.Match(json, "\"browser_download_url\":\\s*\"([^\"]+MelonLoader\\.zip)\"").Groups[1].Value;
            return (tag, url);
        }

        public async Task DownloadAsync(string url, string dest, IProgress<double> progress = null)
        {
            using var res = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            long? total = res.Content.Headers.ContentLength;
            using var src = await res.Content.ReadAsStreamAsync();
            using var dst = System.IO.File.Create(dest);
            var buf = new byte[65536];
            long got = 0; int n;
            while ((n = await src.ReadAsync(buf, 0, buf.Length)) > 0)
            {
                await dst.WriteAsync(buf, 0, n);
                got += n;
                if (total > 0) progress?.Report((double)got / total.Value);
            }
        }
    }
}
