using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logicpos.reports
{
    [FlagsAttribute()]
    public enum CornerCurveMode
    {

        None = 0,
        TopLeft = 1,
        TopRight = 2,
        TopLeft_TopRight = 3,
        BottomLeft = 4,
        TopLeft_BottomLeft = 5,
        TopRight_BottomLeft = 6,
        TopLeft_TopRight_BottomLeft = 7,
        BottomRight = 8,
        BottomRight_TopLeft = 9,
        BottomRight_TopRight = 10,
        BottomRight_TopLeft_TopRight = 11,
        BottomRight_BottomLeft = 12,
        BottomRight_TopLeft_BottomLeft = 13,
        BottomRight_TopRight_BottomLeft = 14,
        All = 15

    }
}