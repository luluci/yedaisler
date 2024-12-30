using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using yedaisler.Utility;

namespace yedaisler.Config
{
    internal class ColorPickerDialogViewModel : BindableBase, IDisposable
    {
        Utility.ColorPickerViewModel ColorPicker { get; set; }

        public ReactiveCommand OnOk {  get; set; }
        public ReactiveCommand OnCancel { get; set; }

        public bool IsOk { get; set; }

        ColorPickerDialog window;

        public ColorPickerDialogViewModel(ColorPickerDialog window_)
        {
            this.window = window_;

            ColorPicker = window.ColorPicker.DataContext as Utility.ColorPickerViewModel;

            IsOk = false;

            OnOk = new ReactiveCommand();
            OnOk.Subscribe(x =>
            {
                IsOk = true;
                window.Hide();
            })
            .AddTo(Disposables);
            OnCancel = new ReactiveCommand();
            OnCancel.Subscribe(x =>
            {
                IsOk = false;
                window.Hide();
            })
            .AddTo(Disposables);
        }

        public void ShowDialog()
        {
            IsOk = false;
            window.ShowDialog();
        }

        public string GetColor()
        {
            return ColorPicker.Brush.Value.Color.ToString();
        }
        public SolidColorBrush GetBrush()
        {
            return ColorPicker.Brush.Value;
        }

        public void SetColor(byte a, byte r, byte g, byte b)
        {
            ColorPicker.Alpha.Value = a;
            ColorPicker.Red.Value = r;
            ColorPicker.Green.Value = g;
            ColorPicker.Blue.Value = b;
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
