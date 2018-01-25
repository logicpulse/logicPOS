using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_UserPermissionProfile : XPGuidObject
    {
        public SYS_UserPermissionProfile() : base() { }
        public SYS_UserPermissionProfile(Session session) : base(session) { }

        private bool _granted = false;
        public bool Granted
        {
            get
            {
                return (_granted);
            }
            set
            {
                _granted = value;
            }
        }

        //UserProfile One <> Many UserProfilePermissions
        SYS_UserProfile fUserProfile;
        [Association(@"UserProfile-UserPermissionProfile")]
        public SYS_UserProfile UserProfile
        {
            get { return fUserProfile; }
            set { SetPropertyValue<SYS_UserProfile>("UserProfile", ref fUserProfile, value); }
        }

        //UserPermissionProfile One <> Many UserPermissionItem 
        SYS_UserPermissionItem fPermissionItem;
        [Association(@"UserPermissionProfile-UserPermissionItem")]
        public SYS_UserPermissionItem PermissionItem
        {
            get { return fPermissionItem; }
            set { SetPropertyValue<SYS_UserPermissionItem>("PermissionItem", ref fPermissionItem, value); }
        }
    }
}