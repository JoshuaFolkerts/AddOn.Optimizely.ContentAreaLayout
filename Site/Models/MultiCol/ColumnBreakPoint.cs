using System.ComponentModel;

namespace Site.Models.MultiCol
{
    public enum ColumnBreakPoint
    {
        [Description("X-Small (auto)")]
        xs = 1,

        [Description("Small (576px)")]
        sm = 2,

        [Description("Medium (768px)")]
        md = 3,

        [Description("Large (992px)")]
        lg = 4,

        [Description("X-Large (1440px)")]
        xl = 5,

        [Description("XX-Large (1920px)")]
        xxl = 6
    }
}
