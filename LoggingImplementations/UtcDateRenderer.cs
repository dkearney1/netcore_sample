using System.Globalization;
using System.Text;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;

namespace LoggingImplementations
{
    [LayoutRenderer("utc_date")]
    public sealed class UtcDateRenderer : LayoutRenderer
    {
        [DefaultParameter]
        public string Format { get; set; }
        public CultureInfo Culture { get; set; }

        public UtcDateRenderer()
        {
            Format = "G";
            Culture = CultureInfo.InvariantCulture;
        }

        protected override void Append(StringBuilder builder, LogEventInfo logEventInfo)
        {
            builder.Append(logEventInfo.TimeStamp.ToUniversalTime().ToString(Format, Culture));
        }
    }
}