using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace yedaisler.Config.Model
{
    internal class Config
    {
        [JsonPropertyName("todos")]
        public IList<ToDo> ToDos { get; set; }
    }
}
