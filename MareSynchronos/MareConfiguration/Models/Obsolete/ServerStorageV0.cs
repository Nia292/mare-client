﻿namespace MareSynchronos.MareConfiguration.Models;

[Serializable]
[Obsolete("Deprecated, use ServerStorage")]
public class ServerStorageV0
{
    public string ServerUri { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
    public List<Authentication> Authentications { get; set; } = new();
    public Dictionary<string, string> UidServerComments { get; set; } = new(StringComparer.Ordinal);
    public Dictionary<string, string> GidServerComments { get; set; } = new(StringComparer.Ordinal);
    public Dictionary<string, List<string>> UidServerPairedUserTags = new(StringComparer.Ordinal);
    public HashSet<string> ServerAvailablePairTags { get; set; } = new(StringComparer.Ordinal);
    public HashSet<string> OpenPairTags { get; set; } = new(StringComparer.Ordinal);
    public Dictionary<int, SecretKey> SecretKeys { get; set; } = new();
    public bool FullPause { get; set; } = false;

    public ServerStorage ToV1()
    {
        return new ServerStorage()
        {
            ServerUri = this.ServerUri,
            ServerName = this.ServerName,
            Authentications = this.Authentications.ToList(),
            FullPause = this.FullPause,
            SecretKeys = this.SecretKeys.ToDictionary(p => p.Key, p => p.Value)
        };
    }
}
