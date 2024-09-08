using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yedaisler.Utility;

namespace yedaisler.ToDo
{
    // アクション動作内容
    internal enum ActionType
    {
        None = 0,
        OpenUrl,        // URLを既定のブラウザで開く(ShellExecuteのalias)
        shellExecute,   // ShellExecuteで実行する(拡張子の既定の動作で実行)
    }

    // 動作モード:アクション遷移条件
    internal enum ActionMode
    {
        None = 0,
        Exec,   // 実行したら次状態へ遷移
    }

    internal class StateInfo : BindableBase
    {
        Config.ToDoStateInfo stateInfo;

        public ReactivePropertySlim<string> Name { get; set; }

        public State State { get; internal set; }

        public ReactivePropertySlim<string> ActionName { get; set; }

        public ReactiveCommand Action {  get; set; }

        public ReactivePropertySlim<bool> HasAction { get; set; }

        public StateInfo(State state, Config.ToDoStateInfo info)
        {
            stateInfo = info;

            State = state;

            HasAction = new ReactivePropertySlim<bool>(true);

            Name = new ReactivePropertySlim<string>(info.Name.Value);

            ActionName = new ReactivePropertySlim<string>(info.Action.Value.Name.Value);
            // Name設定
            if (System.Object.ReferenceEquals(ActionName.Value, string.Empty))
            {
                ActionName.Value = "<なし>";
                HasAction.Value = false;
            }

            //
            Action = new ReactiveCommand();
            Action.Subscribe(x =>
            {
                //stateInfo.Action.Value.Action.Exec();
            });
        }
    }
}
