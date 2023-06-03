using System;
using System.Text.Json.Serialization;

namespace UpdaterLibrary.Json
{
    public sealed class Tag : IJsonEntity
    {
        public sealed class CommitInfo
        {
            [JsonPropertyName("sha")]
            public string Sha { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }
        }

        private string name;

        [JsonPropertyName("name")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;

                Version = Version.Parse(name.Replace("v", string.Empty));
            }
        }

        [JsonIgnore]
        public Version Version { get; private set; }

        [JsonPropertyName("commit")]
        public CommitInfo Commit { get; set; }

        [JsonPropertyName("zipball_url")]
        public string ZipBallUrl { get; set; }

        [JsonPropertyName("tarball_url")]
        public string TarBallUrl { get; set; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }
    }
}
