using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Features.Database
{
    public class DatabaseBackup : ApiEntity
    {
        public uint Version { get; set; }
        public string FileName { get; set; }
    }
}
