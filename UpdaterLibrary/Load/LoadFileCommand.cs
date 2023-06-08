using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UpdaterLibrary.Commands;

namespace UpdaterLibrary.Load
{
    internal class LoadFileCommand : EntityCommand, ILoadFileCommand
    {
        public void Execute(HttpClient httpClient, string url, string path)
        {
            Task.Run(() => LoadAsync(httpClient, url, path)).GetAwaiter().GetResult();
        }

        private async Task LoadAsync(HttpClient httpClient, string url, string path)
        {
            using (var stream = await httpClient.GetStreamAsync(url))
            {
                using (var fileStream = new FileStream(path, FileMode.CreateNew))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }
    }
}
