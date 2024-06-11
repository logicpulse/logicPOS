using DevExpress.Xpo;
using LogicPOS.Settings.Enums;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_systembackup : Entity
    {
        public sys_systembackup() : base() { }
        public sys_systembackup(Session session) : base(session) { }

        private DatabaseType fDataBaseType;
        public DatabaseType DataBaseType
        {
            get { return fDataBaseType; }
            set { SetPropertyValue("DataBaseType", ref fDataBaseType, value); }
        }

        private uint fVersion;
        public uint Version
        {
            get { return fVersion; }
            set { SetPropertyValue("Version", ref fVersion, value); }
        }

        private string fFileName;
        [Indexed(Unique = true), Size(255)]
        public string FileName
        {
            get { return fFileName; }
            set { SetPropertyValue<string>("FileName", ref fFileName, value); }
        }

        private string fFileNamePacked;
        [Indexed(Unique = true), Size(255)]
        public string FileNamePacked
        {
            get { return fFileNamePacked; }
            set { SetPropertyValue<string>("FileNamePacked", ref fFileNamePacked, value); }
        }

        private string fFilePath;
        public string FilePath
        {
            get { return fFilePath; }
            set { SetPropertyValue<string>("FilePath", ref fFilePath, value); }
        }

        private string fFileHash;
        [Size(255)]
        public string FileHash
        {
            get { return fFileHash; }
            set { SetPropertyValue<string>("FileHash", ref fFileHash, value); }
        }

        private sys_userdetail fUser;
        public sys_userdetail User
        {
            get { return fUser; }
            set { SetPropertyValue("User", ref fUser, value); }
        }

        private pos_configurationplaceterminal fTerminal;
        public pos_configurationplaceterminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue("Terminal", ref fTerminal, value); }
        }
    }
}