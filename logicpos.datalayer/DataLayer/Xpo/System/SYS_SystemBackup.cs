using System;
using DevExpress.Xpo;
using logicpos.datalayer.Enums;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_systembackup : XPGuidObject
    {
        public sys_systembackup() : base() { }
        public sys_systembackup(Session session) : base(session) { }

        private DatabaseType fDataBaseType;
        public DatabaseType DataBaseType
        {
            get { return fDataBaseType; }
            set { SetPropertyValue<DatabaseType>("DataBaseType", ref fDataBaseType, value); }
        }

        private UInt32 fVersion;
        public UInt32 Version
        {
            get { return fVersion; }
            set { SetPropertyValue<UInt32>("Version", ref fVersion, value); }
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
            set { SetPropertyValue<sys_userdetail>("User", ref fUser, value); }
        }

        private pos_configurationplaceterminal fTerminal;
        public pos_configurationplaceterminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue<pos_configurationplaceterminal>("Terminal", ref fTerminal, value); }
        }
    }
}