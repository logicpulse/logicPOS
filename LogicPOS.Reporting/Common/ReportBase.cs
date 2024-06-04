namespace LogicPOS.Reporting.Common
{
    public abstract class ReportBase
    {
        internal string _oid;

        public virtual string Oid
        {
            get { return _oid; }
            set { _oid = value; }
        }
    }
}
