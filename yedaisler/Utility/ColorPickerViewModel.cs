﻿using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace yedaisler.Utility
{
    internal class ColorPickerViewModel : BindableBase, IDisposable
    {
        public ReactivePropertySlim<int> Alpha { get; set; }
        public ReactivePropertySlim<int> Red {  get; set; }
        public ReactivePropertySlim<int> Blue { get; set; }
        public ReactivePropertySlim<int> Green { get; set; }

        // private WriteableBitmap SVbitmap;
        public ReactivePropertySlim<SolidColorBrush> Brush { get; set; }

        public ColorPickerViewModel()
        {
            Brush = new ReactivePropertySlim<SolidColorBrush>(new SolidColorBrush(System.Windows.Media.Color.FromArgb(255,255,255,255)));

            Alpha = new ReactivePropertySlim<int>(255, ReactivePropertyMode.DistinctUntilChanged);
            Alpha.Subscribe(x => {
                UpdateBrush();
            })
            .AddTo(Disposables);
            Red = new ReactivePropertySlim<int>(255, ReactivePropertyMode.DistinctUntilChanged);
            Red.Subscribe(x => {
                UpdateBrush();
            })
            .AddTo(Disposables);
            Blue = new ReactivePropertySlim<int>(255, ReactivePropertyMode.DistinctUntilChanged);
            Blue.Subscribe(x => {
                UpdateBrush();
            })
            .AddTo(Disposables);
            Green = new ReactivePropertySlim<int>(255, ReactivePropertyMode.DistinctUntilChanged);
            Green.Subscribe(x => {
                UpdateBrush();
            })
            .AddTo(Disposables);
        }

        public void UpdateBrush()
        {
            Brush.Value = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)Alpha.Value, (byte)Red.Value, (byte)Green.Value, (byte)Blue.Value));
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
