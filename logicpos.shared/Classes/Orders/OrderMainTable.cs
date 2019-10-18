using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.shared.App;
using System;

namespace logicpos.shared.Classes.Orders
{
    public class OrderMainTable
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Guid _oid;
        public Guid Oid
        {
            get { return _oid; }
            set { _oid = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private PriceType _priceType;
        public PriceType PriceType
        {
            get { return _priceType; }
            set { _priceType = value; }
        }

        private Guid _placeId;
        public Guid PlaceId
        {
            get { return _placeId; }
            set { _placeId = value; }
        }

        private Guid _orderMainOid;
        public Guid OrderMainOid
        {
            get { return _orderMainOid; }
            set { _orderMainOid = value; }
        }

        //Required Parameterless Constructor for Json.NET (Load)
        public OrderMainTable() { }
        //Constructor without Json.NET Load, With Defaults
        public OrderMainTable(Guid pOrderMainOid, Guid pTableOid)
        {
            _orderMainOid = pOrderMainOid;
            _oid = pTableOid;
            try
            {
                pos_configurationplacetable table = (pos_configurationplacetable)FrameworkUtils.GetXPGuidObject(typeof(pos_configurationplacetable), pTableOid);
                _name = table.Designation;
                //Enum is not Zero Indexed
                _priceType = (PriceType)table.Place.PriceType.EnumValue;
                _placeId = table.Place.Oid;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}

