using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using yedaisler.Utility;

namespace yedaisler.Config.ViewModel.Gui
{
    internal class ColorInfo : BindableBase
    {
        public yedaisler.Config.Color Id { get; internal set; }
        public ConfigItem<string> Str { get; set; }
        public ConfigItem<SolidColorBrush> Brush { get; set; }

        // 解析パターン
        static Regex re_rgba = new Regex(@"#([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})", RegexOptions.Compiled);

        //
        private Model.Color model_ref;

        public ColorInfo(yedaisler.Config.Color id, ConfigItemApplier applier, CompositeDisposable disposables)
        {
            Id = id;

            //
            Str = new ConfigItem<string>("#FFFFFFFF", applier);
            Brush = new ConfigItem<SolidColorBrush>(Str.View.Value.ToSolidColorBrush(), applier);
        }

        public void Init(Model.Color color)
        {
            model_ref = color;

            // 色指定文字列初期化
            Str.View.Value = GetModelStr();
            Str.WriteBack = (string str) =>
            {
                SetModelStr(str);
            };
            // Brush初期化
            // Model(json)には文字列のみ保存するためWriteBackの設定はしない
            Brush.View.Value = Str.View.Value.ToSolidColorBrush();
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
        public (byte A, byte R, byte G, byte B) GetArgb()
        {
            (byte A, byte R, byte G, byte B) result = (A: 0, R: 0, G: 0, B: 0);
            var match = re_rgba.Match(Str.View.Value);
            if (match.Success)
            {
                byte value;
                if (byte.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber, null, out value))
                {
                    result.A = value;
                }
                if (byte.TryParse(match.Groups[2].Value, System.Globalization.NumberStyles.HexNumber, null, out value))
                {
                    result.R = value;
                }
                if (byte.TryParse(match.Groups[3].Value, System.Globalization.NumberStyles.HexNumber, null, out value))
                {
                    result.G = value;
                }
                if (byte.TryParse(match.Groups[4].Value, System.Globalization.NumberStyles.HexNumber, null, out value))
                {
                    result.B = value;
                }
            }
            return result;
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
