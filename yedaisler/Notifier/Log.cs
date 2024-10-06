using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using yedaisler.Utility;

namespace yedaisler.Notifier
{
    internal static class Log
    {
        static public LogImpl NotifierLog = new LogImpl();
    }

    public enum NotifyType
    {
        None,
        ToDoAction,
    }
    internal class NotifyItem
    {
        public string Text { get; set; }
        public object Object { get; set; } = null;
        public NotifyType Type { get; set; } = NotifyType.None;

        // 制御フラグ
        public bool IsNotified { get; set; } = false;
    }

    internal class LogImpl : BindableBase, IDisposable
    {
        public ReactiveCollection<NotifyItem> Data { get; set; }
        public StreamWriter Writer { get; set; }

        public LogImpl()
        {
            Data = new ReactiveCollection<NotifyItem>();
            Data.AddTo(Disposables);

            Writer = null;
        }

        public void Close()
        {
            //
            if (!(Writer is null))
            {
                Writer.Flush();
                Writer.Close();
                Writer = null;
            }
        }

        public void Open(string path)
        {
            //
            Writer = new StreamWriter(path);
        }


        #region IDisposable Support
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();
        protected bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    this.Disposables.Dispose();
                    Close();
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
