using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yedaisler.Utility
{
    internal class ColorPickerViewModel : BindableBase, IDisposable
    {

        public ReactivePropertySlim<int> Alpha { get; set; }
        public ReactivePropertySlim<int> Red {  get; set; }
        public ReactivePropertySlim<int> Blue { get; set; }
        public ReactivePropertySlim<int> Green { get; set; }

        public ColorPickerViewModel()
        {
            // http://kagenyan.blog69.fc2.com/blog-entry-139.html
            // https://gogowaten.hatenablog.com/entry/2023/04/20/164232


            Alpha = new ReactivePropertySlim<int>(255);
            Alpha.AddTo(Disposables);
            Red = new ReactivePropertySlim<int>(255);
            Red.AddTo(Disposables);
            Blue = new ReactivePropertySlim<int>(255);
            Blue.AddTo(Disposables);
            Green = new ReactivePropertySlim<int>(255);
            Green.AddTo(Disposables);
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
