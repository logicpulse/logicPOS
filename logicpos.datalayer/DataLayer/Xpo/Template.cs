using System;
using DevExpress.Xpo;

// Notes: 
//  Search/Replace Template with new Class Name ex PFX_OBJECT
//  Remove [NonPersistent] Attribute

namespace logicpos.datalayer.DataLayer.Xpo
{
    [NonPersistent]
    [DeferredDeletion(false)]
    public class Template : XPGuidObject
    {
        public Template() : base() { }
        public Template(Session session) : base(session) { }

        //This Can be Optional
        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        //This Can be Optional
        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        //Add Custom Fields Here

        //Add Custom Relation Here
    }
}