using NetworkInfrastructure.Web.Data.Entities;

namespace NetworkInfrastructure.Web.Models
{
    public class NetworkAssetDto
    {
        public Guid Id { get; set; }
        public string? ServerName { get; set; }
        public string? Ip { get; set; }
        public BackupType BackupType { get; set; }
        public string? ServiceOwner { get; set; }
        public OsType OsType { get; set; }
        public LocationName LocationName { get; set; }
        public string? Description { get; set; }
        public DateTime LastUpdate { get; set; }
        public string? ServerPort { get; set; }
        public DateTime DateCreated { get; set; }
        public string? UserName { get; set; }
        public bool Monitoring { get; set; }
        public bool Limitation { get; set; }
        public bool FirewallWindows { get; set; }
        public bool NetBios { get; set; }
        public bool FolderShare { get; set; }
        public bool Mcafee { get; set; }
        public bool Activation { get; set; }
        public bool SplunkAgent { get; set; }
        public bool GroupPolicy { get; set; }
        public bool AccessLists { get; set; }
        public bool FtdDatacenter { get; set; }
        public bool FtdInterne { get; set; }
        public bool Sophos { get; set; }
        public bool Waf { get; set; }
        public bool Asr { get; set; }
        public bool Dns { get; set; }

    }
}
