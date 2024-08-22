using LogicPOS.Api.Entities.Enums;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class PreferenceParameter : ApiEntity, IWithCode
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Token { get; set; }
        public string Value { get; set; }
        public string ValueTip { get; set; }
        public bool Required { get; set; }
        public string RegEx { get; set; }
        public string ResourceString { get; set; }
        public string ResourceStringValue { get; set; }
        public string ResourceStringInfo { get; set; }
        public int FormType { get; set; }
        public Nullable<int> FormPageNo { get; set; }
        public PreferenceParameterInputType InputType { get; set; }
    }
}
