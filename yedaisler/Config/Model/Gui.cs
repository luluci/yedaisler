using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace yedaisler.Config.Model
{
    internal class Gui
    {
        [JsonPropertyName("color")]
        public Color Color { get; set; }

    }

    internal class Color
    {
        [JsonPropertyName("fontReady")]
        public string FontReady { get; set; } = "#FF000000";

        [JsonPropertyName("fontDoing")]
        public string FontDoing { get; set; } = "#FF000000";

        [JsonPropertyName("fontDone")]
        public string FontDone { get; set; } = "#FF000000";

        [JsonPropertyName("backReady")]
        public string BackReady { get; set; } = "#A0C0C0C0";

        [JsonPropertyName("backDoing")]
        public string BackDoing { get; set; } = "#A0C0C0C0";

        [JsonPropertyName("backDone")]
        public string BackDone { get; set; } = "#A0C0C0C0";
    }
}
