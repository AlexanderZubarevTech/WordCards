using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UpdaterLibrary.Json
{
    public sealed class Release : IJsonEntity
    {
        public sealed class User
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("login")]
            public string Login { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("node_id")]
            public string NodeId { get; set; }

            [JsonPropertyName("avatar_url")]
            public string AvatarUrl { get; set; }

            [JsonPropertyName("gravatar_id")]
            public string GravatarId { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("html_url")]
            public string HtmlUrl { get; set; }

            [JsonPropertyName("followers_url")]
            public string FollowersUrl { get; set; }

            [JsonPropertyName("following_url")]
            public string FollowingUrl { get; set; }

            [JsonPropertyName("gists_url")]
            public string GistsUrl { get; set; }

            [JsonPropertyName("starred_url")]
            public string StarredUrl { get; set; }

            [JsonPropertyName("subscriptions_url")]
            public string SubscriptionsUrl { get; set; }

            [JsonPropertyName("organizations_url")]
            public string OrganizationsUrl { get; set; }

            [JsonPropertyName("repos_url")]
            public string ReposUrl { get; set; }

            [JsonPropertyName("events_url")]
            public string EventsUrl { get; set; }

            [JsonPropertyName("received_events_url")]
            public string ReceivedEventsUrl { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("site_admin")]
            public bool SiteAdmin { get; set; }

            [JsonPropertyName("starred_at")]
            public string StarredAt { get; set; }
        }

        public sealed class Asset
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("browser_download_url")]
            public string BrowserDownloadUrl { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("node_id")]
            public string NodeId { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("label")]
            public string Label { get; set; }

            [JsonPropertyName("state")]
            public string State { get; set; }

            [JsonPropertyName("content_type")]
            public string ContentType { get; set; }

            [JsonPropertyName("size")]
            public int Size { get; set; }

            [JsonPropertyName("download_count")]
            public int DownloadCount { get; set; }

            [JsonPropertyName("created_at")]
            public string CreatedAt { get; set; }

            [JsonPropertyName("updated_at")]
            public string UpdatedAt { get; set; }

            [JsonPropertyName("uploader")]
            public User Uploader { get; set; }
        }

        //public sealed class Reactions
        //{
        //    [JsonPropertyName("url")]
        //    public string Url { get; set; }

        //    [JsonPropertyName("total_count")]
        //    public int TotalCount { get; set; }

        //    [JsonPropertyName("+1")]
        //    public int Plus { get; set; }

        //    [JsonPropertyName("-1")]
        //    public int Minus { get; set; }

        //    [JsonPropertyName("laugh")]
        //    public int Laugh { get; set; }

        //    [JsonPropertyName("confused")]
        //    public int Confused { get; set; }

        //    [JsonPropertyName("heart")]
        //    public int Heart { get; set; }

        //    [JsonPropertyName("hooray")]
        //    public int Hooray { get; set; }

        //    [JsonPropertyName("eyes")]
        //    public int Eyes { get; set; }

        //    [JsonPropertyName("rocket")]
        //    public int Rocket { get; set; }            
        //}


        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("assets_url")]
        public string AssetsUrl { get; set; }

        [JsonPropertyName("upload_url")]
        public string UploadUrl { get; set; }

        [JsonPropertyName("tarball_url")]
        public string TarballUrl { get; set; }

        [JsonPropertyName("zipball_url")]
        public string ZipballUrl { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }

        private string _tagName;

        [JsonPropertyName("tag_name")]
        public string TagName
        {
            get { return _tagName; }
            set
            {
                _tagName = value;

                Version = Version.Parse(_tagName.Replace("v", string.Empty));
            }
        }

        [JsonIgnore]
        public Version Version { get; private set; }

        [JsonPropertyName("target_commitish")]
        public string TargetCommitish { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        //[JsonPropertyName("body")]
        //public string Body { get; set; }

        [JsonPropertyName("draft")]
        public bool Draft { get; set; }

        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("published_at")]
        public string PublishedAt { get; set; }

        [JsonPropertyName("author")]
        public User Author { get; set; }

        [JsonPropertyName("assets")]
        public List<Asset> Assets { get; set; }

        //[JsonPropertyName("body_html")]
        //public string BodyHtml { get; set; }

        //[JsonPropertyName("body_text")]
        //public string BodyText { get; set; }

        //[JsonPropertyName("mentions_count")]
        //public int MentionsCount { get; set; }

        //[JsonPropertyName("discussion_url")]
        //public string DiscussionUrl { get; set; }

        //[JsonPropertyName("reactions")]
        //public Reactions ReleaseReactions { get; set; }
    }
}
