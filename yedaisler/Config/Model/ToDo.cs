using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace yedaisler.Config.Model
{
    internal class ToDo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("ready")]
        public ToDoStateInfo Ready { get; set; }

        [JsonPropertyName("doing")]
        public ToDoStateInfo Doing { get; set; }

        [JsonPropertyName("done")]
        public ToDoStateInfo Done { get; set; }
    }

    internal class ToDoStateInfo
    {
        // 状態表示名
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        // 動作モード:アクション遷移条件
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = string.Empty;

        // 状態表示名
        [JsonPropertyName("action")]
        public ToDoAction Action { get; set; }

        // 指定した動作をブロックする
        [JsonPropertyName("block")]
        public ToDoActionBlock Block { get; set; }
    }

    internal class ToDoAction
    {
        // 状態表示名
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        // アクション動作内容
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        // Type個別設定情報
        [JsonPropertyName("shellExecute")]
        public ToDoActionShellExecute ShellExecute { get; set; }
        [JsonPropertyName("openUrl")]
        public ToDoActionOpenUrl OpenUrl { get; set; }

        // Actionにより記憶するデータ(予定)
        [JsonPropertyName("data")]
        public string Data { get; set; } = string.Empty;

    }

    internal class ToDoActionShellExecute
    {
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;
    }
    internal class ToDoActionOpenUrl
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }

    internal class ToDoActionBlock
    {
        [JsonPropertyName("shutdown")]
        public bool Shutdown { get; set; } = false;

        [JsonPropertyName("sleep")]
        public bool Sleep { get; set; } = false;
    }
}
