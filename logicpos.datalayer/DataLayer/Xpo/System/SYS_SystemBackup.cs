using System;
using DevExpress.Xpo;
using logicpos.datalayer.Enums;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_SystemBackup : XPGuidObject
    {
        public SYS_SystemBackup() : base() { }
        public SYS_SystemBackup(Session session) : base(session) { }

        DatabaseType fDataBaseType;
        public DatabaseType DataBaseType
        {
            get { return fDataBaseType; }
            set { SetPropertyValue<DatabaseType>("DataBaseType", ref fDataBaseType, value); }
        }

        UInt32 fVersion;
        public UInt32 Version
        {
            get { return fVersion; }
            set { SetPropertyValue<UInt32>("Version", ref fVersion, value); }
        }

        string fFileName;
        [Indexed(Unique = true), Size(255)]
        public string FileName
        {
            get { return fFileName; }
            set { SetPropertyValue<string>("FileName", ref fFileName, value); }
        }

        string fFileNamePacked;
        [Indexed(Unique = true), Size(255)]
        public string FileNamePacked
        {
            get { return fFileNamePacked; }
            set { SetPropertyValue<string>("FileNamePacked", ref fFileNamePacked, value); }
        }

        string fFilePath;
        public string FilePath
        {
            get { return fFilePath; }
            set { SetPropertyValue<string>("FilePath", ref fFilePath, value); }
        }

        string fFileHash;
        [Size(255)]
        public string FileHash
        {
            get { return fFileHash; }
            set { SetPropertyValue<string>("FileHash", ref fFileHash, value); }
        }

        SYS_UserDetail fUser;
        public SYS_UserDetail User
        {
            get { return fUser; }
            set { SetPropertyValue<SYS_UserDetail>("User", ref fUser, value); }
        }

        POS_ConfigurationPlaceTerminal fTerminal;
        public POS_ConfigurationPlaceTerminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("Terminal", ref fTerminal, value); }
        }
    }
}