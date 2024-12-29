﻿using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using yedaisler.Utility;

namespace yedaisler.Config.ViewModel.Gui
{
    internal class ColorInfo : BindableBase
    {
        public yedaisler.Config.Color Id { get; internal set; }
        public ConfigItem<string> Str { get; set; }
        public ReactivePropertySlim<SolidColorBrush> Brush { get; set; }

        //
        private Model.Color model_ref;

        public ColorInfo(yedaisler.Config.Color id, ConfigItemApplier applier, CompositeDisposable disposables)
        {
            Id = id;

            //
            Str = new ConfigItem<string>("#FFFFFFFF", applier);
            Brush = new ReactivePropertySlim<SolidColorBrush>(Str.View.Value.ToSolidColorBrush());
            Brush.AddTo(disposables);
        }

        public void Init(Model.Color color)
        {
            model_ref = color;

            Str.View.Value = GetModelStr();
            Str.WriteBack = (string str) =>
            {
                SetModelStr(str);
                Brush.Value = str.ToSolidColorBrush();
            };
        }

        private string GetModelStr()
        {
            switch (Id)
            {
                case yedaisler.Config.Color.FontReady:
                    return model_ref.FontReady;
                case yedaisler.Config.Color.FontDoing:
                    return model_ref.FontDoing;
                case yedaisler.Config.Color.FontDone:
                    return model_ref.FontDone;
                case yedaisler.Config.Color.BackReady:
                    return model_ref.BackReady;
                case yedaisler.Config.Color.BackDoing:
                    return model_ref.BackDoing;
                case yedaisler.Config.Color.BackDone:
                    return model_ref.BackDone;

                default:
                    throw new Exception("unknown id");
            }
        }
        private void SetModelStr(string value)
        {
            switch (Id)
            {
                case yedaisler.Config.Color.FontReady:
                    model_ref.FontReady = value;
                    break;
                case yedaisler.Config.Color.FontDoing:
                    model_ref.FontDoing = value;
                    break;
                case yedaisler.Config.Color.FontDone:
                    model_ref.FontDone = value;
                    break;
                case yedaisler.Config.Color.BackReady:
                    model_ref.BackReady = value;
                    break;
                case yedaisler.Config.Color.BackDoing:
                    model_ref.BackDoing = value;
                    break;
                case yedaisler.Config.Color.BackDone:
                    model_ref.BackDone = value;
                    break;

                default:
                    throw new Exception("unknown id");
            }
        }
    }

    internal class Color : BindableBase, IDisposable
    {
        public ReactiveCollection<ColorInfo> Items;

        public Color(ConfigItemApplier applier)
        {
            Items = new ReactiveCollection<ColorInfo>();
            foreach (var obj in Enum.GetValues(typeof(yedaisler.Config.Color)))
            {
                var id = (yedaisler.Config.Color)obj;
                Items.Add(new ColorInfo(id, applier, Disposables));
            }

        }


        public void Init(Model.Color color, ConfigItemApplier applier)
        {
            // Model(json)チェック
            if (color is null)
            {
                color = new Model.Color();
            }
            // Modelオブジェクト作成(json読み込み)後に初期化する
            foreach (var inf in Items)
            {
                inf.Init(color);
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