using DevExpress.Xpo;
using LogicPOS.Domain.Entities;


// Notes: 
//  Search/Replace Template with new Class Name ex PFX_OBJECT
//  Remove [NonPersistent] Attribute

namespace LogicPOS.Data.XPO
{
    [NonPersistent]
    [DeferredDeletion(false)]
    public class TemplateEntity : Entity
    {
        public TemplateEntity() : base() { }
        public TemplateEntity(Session session) : base(session) { }

        //This Can be Optional
        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        //This Can be Optional
        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
        }

        //Add Custom Fields Here

        //Add Custom Relation Here
    }
}