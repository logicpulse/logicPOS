using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_userpermissionprofile : XPGuidObject
    {
        public sys_userpermissionprofile() : base() { }
        public sys_userpermissionprofile(Session session) : base(session) { }

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
        sys_userprofile fUserProfile;
        [Association(@"UserProfile-UserPermissionProfile")]
        public sys_userprofile UserProfile
        {
            get { return fUserProfile; }
            set { SetPropertyValue<sys_userprofile>("UserProfile", ref fUserProfile, value); }
        }

        //UserPermissionProfile One <> Many UserPermissionItem 
        sys_userpermissionitem fPermissionItem;
        [Association(@"UserPermissionProfile-UserPermissionItem")]
        public sys_userpermissionitem PermissionItem
        {
            get { return fPermissionItem; }
            set { SetPropertyValue<sys_userpermissionitem>("PermissionItem", ref fPermissionItem, value); }
        }
    }
}