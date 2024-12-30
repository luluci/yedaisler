using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yedaisler.Utility;

namespace yedaisler.Config
{
    internal class ColorPickerDialogViewModel : BindableBase, IDisposable
    {
        Utility.ColorPickerViewModel ColorPicker { get; set; }

        public ReactiveCommand OnOk {  get; set; }
        public ReactiveCommand OnCancel { get; set; }

        public ColorPickerDialogViewModel(ColorPickerDialog window)
        {
            ColorPicker = window.ColorPicker.DataContext as Utility.ColorPickerViewModel;

            OnOk = new ReactiveCommand();
            OnOk.Subscribe(x =>
            {
                int i = 0;
                i++;
                window.Hide();
            })
            .AddTo(Disposables);
            OnCancel = new ReactiveCommand();
            OnCancel.Subscribe(x =>
            {
                int i = 0;
                i++;
                window.Hide();
            })
            .AddTo(Disposables);
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
