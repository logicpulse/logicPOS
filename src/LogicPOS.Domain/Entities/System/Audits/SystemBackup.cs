using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("sys_systembackup")]
public class SystemBackup : Entity
{
    public int DatabaseType { get; set; }
    public uint Version { get; set; }
    public string? FileName { get; set; }
    public string? FileNamePacked { get; set; }
    public string? FilePath { get; set; }
    public string? FileHash { get; set; }
}
