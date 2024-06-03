using DevExpress.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_userpermissionprofile : Entity
    {
        public sys_userpermissionprofile() : base() { }
        public sys_userpermissionprofile(Session session) : base(session) { }

        private bool _granted = false;
        public bool Granted
        {
            get
            {
                return _granted;
            }
            set
            {
                _granted = value;
            }
        }

        //UserProfile One <> Many UserProfilePermissions
        private sys_userprofile fUserProfile;
        [Association(@"UserProfile-UserPermissionProfile")]
        public sys_userprofile UserProfile
        {
            get { return fUserProfile; }
            set { SetPropertyValue("UserProfile", ref fUserProfile, value); }
        }

        //UserPermissionProfile One <> Many UserPermissionItem 
        private sys_userpermissionitem fPermissionItem;
        [Association(@"UserPermissionProfile-UserPermissionItem")]
        public sys_userpermissionitem PermissionItem
        {
            get { return fPermissionItem; }
            set { SetPropertyValue("PermissionItem", ref fPermissionItem, value); }
        }
    }
}