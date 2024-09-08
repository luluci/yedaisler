﻿using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using yedaisler.Menu;
using yedaisler.Utility;
using static System.Windows.Forms.AxHost;

namespace yedaisler.ToDo
{
    // ToDo状態
    internal enum State
    {
        Ready,
        Doing,
        Done,
        None,
    }

    internal class Item : BindableBase, IDisposable
    {
        public ItemType Type { get; set; } = ItemType.ToDo;

        public ReactivePropertySlim<string> Name { get; set; }

        public ReactivePropertySlim<State> State { get; set; }

        public ReactivePropertySlim<string> StateDisp { get; set; }
        public ReactivePropertySlim<string> StateName { get; set; }

        public ReactiveCollection<StateInfo> StateInfos { get; set; }

        public ReactiveCommand OnMenuStateAction { get; set; }

        public ReactiveCommand OnSystemMenuAction {  get; set; }

        public Config.ToDo ToDoRef;

        public Item(Config.ToDo todo)
        {
            //
            Name = new ReactivePropertySlim<string>(todo.Name.Value);

            // 現在StateInfo名
            StateName = new ReactivePropertySlim<string>(string.Empty);
            StateName.AddTo(Disposables);
            // State表示文字列
            StateDisp = new ReactivePropertySlim<string>(string.Empty);
            StateDisp.AddTo(Disposables);

            //
            StateInfos = new ReactiveCollection<StateInfo>();
            StateInfos.AddTo(Disposables);
            //
            StateInfos.Add(new StateInfo(ToDo.State.Ready, todo.Ready.Value));
            StateInfos.Add(new StateInfo(ToDo.State.Doing, todo.Doing.Value));
            StateInfos.Add(new StateInfo(ToDo.State.Done, todo.Done.Value));

            //
            OnMenuStateAction = new ReactiveCommand();
            OnMenuStateAction.Subscribe(x =>
            {
                StateAction();
            });

            ToDoRef = todo;

            // State制御値
            State = new ReactivePropertySlim<State>(ToDo.State.Ready);
            State.Subscribe(x =>
            {
                // 状態名
                StateDisp.Value = x.ToString();
                // StateInfo名
                switch (x)
                {
                    case ToDo.State.Ready:
                    case ToDo.State.Doing:
                    case ToDo.State.Done:
                        StateName.Value = StateInfos[(int)x].Name.Value;
                        break;

                    default:
                        StateName.Value = "<None>";
                        break;
                }
            })
            .AddTo(Disposables);

            OnSystemMenuAction = new ReactiveCommand();
            OnSystemMenuAction.Subscribe(x =>
            {
                int i = 0;
                i++;
            })
            .AddTo(Disposables);
        }

        public void Init()
        {
            // 状態初期化
            var state = ToDo.State.Ready;
            // 無条件で遷移するアクションをチェックする
            // Readyをチェック
            if (CheckThrowStateAction(ToDoRef.Ready.Value))
            {
                state = ToDo.State.Doing;

                // 状態遷移するならDoingをチェック
                if (CheckThrowStateAction(ToDoRef.Doing.Value))
                {
                    state = ToDo.State.Done;
                }
            }
            //
            State.Value = state;
        }

        private void StateAction()
        {
            // 状態初期化
            var state = State.Value;
            switch (State.Value)
            {
                case ToDo.State.Ready:
                    // Action実行
                    if (ExecToDoAction(ToDoRef.Ready.Value))
                    {
                        // ActionOKなら次状態へ遷移
                        state = ToDo.State.Doing;
                        // 次状態が無条件遷移かチェック
                        if (CheckThrowStateAction(ToDoRef.Doing.Value))
                        {
                            state = ToDo.State.Done;
                        }
                    }
                    break;

                case ToDo.State.Doing:
                    // Action実行
                    if (ExecToDoAction(ToDoRef.Doing.Value))
                    {
                        // ActionOKなら次状態へ遷移
                        state = ToDo.State.Done;
                    }
                    break;

                case ToDo.State.Done:
                    // Action実行
                    if (ExecToDoAction(ToDoRef.Done.Value))
                    {
                        // ActionOKなら次状態へ遷移
                        state = ToDo.State.Ready;
                    }
                    break;

                default:
                    return;
            }
            //
            State.Value = state;
        }

        private bool CheckThrowStateAction(Config.ToDoStateInfo action)
        {
            // 何もせずに次状態に遷移するかどうかチェックする

            switch (action.Mode.Value)
            {
                case ActionMode.None:
                    // 無条件でDoingに
                    return true;

                case ActionMode.Exec:
                default:
                    // ユーザ操作トリガなら何もしない
                    // 自動判定するモードを追加したら新しいパスを追加する
                    return false;
            }
        }
        private bool ExecToDoAction(Config.ToDoStateInfo action)
        {
            switch (action.Mode.Value)
            {
                case ActionMode.Exec:
                    // アクションを実行する
                    return true;

                case ActionMode.None:
                default:
                    // 無条件でAction有効
                    return true;
            }
        }

        #region IDisposable Support
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    this.Disposables.Dispose();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~MainWindowViewModel()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            //GC.SuppressFinalize(this);
        }
        #endregion
    }
}