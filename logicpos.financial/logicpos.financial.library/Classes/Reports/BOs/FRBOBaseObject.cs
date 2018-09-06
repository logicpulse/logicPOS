using System;

namespace logicpos.financial.library.Classes.Reports.BOs
{
    public abstract class FRBOBaseObject
    {
        //Public DataTable Fields
        internal string _oid;
        //internal bool _disabled;

        //Can Be Override, usefull for Views
        public virtual string Oid
        {
            get { return _oid; }
            set { _oid = value; }
        }

        //public bool Disabled
        //{
        //  get { return _disabled; }
        //  set { _disabled = value; }
        //}
    }
}
