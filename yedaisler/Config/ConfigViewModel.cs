using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Shapes;
using yedaisler.Utility;

namespace yedaisler.Config
{
    internal partial class ConfigViewModel : BindableBase, IDisposable
    {
        // Model
        private JsonSerializerOptions jsonOptions;
        Model.Config Model { get; set; }

        // ViewModel
        public ViewModel.Gui.Gui Gui { get; set; }
        public ReactiveCollection<ToDo> ToDos { get; set; }

        // Model-ViewModel I/F
        private ConfigItemApplier applier;

        // GUI
        public ColorPickerDialog ColorPickerDialog { get; set; }
        public ReactiveCommand OnColorPicker { get; set; }

        Config window;

        public ConfigViewModel(Config window_)
        {
            window = window_;

            applier = new ConfigItemApplier();

            Gui = new ViewModel.Gui.Gui(applier);
            ToDos = new ReactiveCollection<ToDo>();
            ToDos.AddTo(Disposables);

            //
            ColorPickerDialog = new ColorPickerDialog();
            //
            OnColorPicker = new ReactiveCommand();
            OnColorPicker.Subscribe(x => {
                if (x is FrameworkElement elem)
                {
                    var pos = Utility.Screen.GetPopupPos(window, elem, ColorPickerDialog);
                    ColorPickerDialog.Top = pos.Y;
                    ColorPickerDialog.Left = pos.X;
                }
                ColorPickerDialog.ShowDialog();
            })
            .AddTo(Disposables);

        }

        public void OnLoaded()
        {
            ColorPickerDialog.Owner = window;
            ColorPickerDialog.Visibility = Visibility.Hidden;
            ColorPickerDialog.Show();
            ColorPickerDialog.Hide();
        }

        public async Task InitAsync()
        {
            // JSON読み込みオプション
            jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                //Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                //NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
            };

            bool loaded = false;
            // デフォルトパス
            string rootPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            string configPath = rootPath + @"\Config.json";
            // 設定ファイルチェック
            if (File.Exists(configPath))
            {
                try
                {
                    await LoadConfigAsync(configPath);
                    loaded = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            // ファイル読み込みに失敗した場合
            if (!loaded)
            {

            }
        }

        public async Task LoadConfigAsync(string configPath)
        {
            // jsonファイル解析
            using (var stream = new FileStream(configPath, FileMode.Open, FileAccess.Read))
            {
                // jsonファイルパース
                Model = await JsonSerializer.DeserializeAsync<Model.Config>(stream, jsonOptions);
            }
            // ModelからViewModelを作成
            ConvertModel2ViewModel();
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
