using System;

namespace LogicPOS.Reporting.Common
{
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Property,
        AllowMultiple = true,
        Inherited = false
        )]
    public class ReportAttribute : Attribute
    {
        public string Sql { get; set; }

        public string Entity { get; set; }

        public string Fields { get; set; }

        public string Filter { get; set; }

        public string Group { get; set; }

        public string Order { get; set; }

        public int Limit { get; set; }

        public string Field { get; set; }

        public bool Hide { get; set; }
    }
}
