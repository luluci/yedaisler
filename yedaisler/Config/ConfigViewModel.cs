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
using System.Windows.Documents;
using System.Windows.Shapes;
using yedaisler.Utility;

namespace yedaisler.Config
{
    internal partial class ConfigViewModel : BindableBase, IDisposable
    {
        private JsonSerializerOptions jsonOptions;

        public ReactivePropertySlim<Gui> Gui { get; set; }
        public ReactiveCollection<ToDo> ToDos { get; set; }

        public ConfigViewModel()
        {
            Gui = new ReactivePropertySlim<Gui>(new Gui());
            Gui.AddTo(Disposables);
            ToDos = new ReactiveCollection<ToDo>();
            ToDos.AddTo(Disposables);
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
                var model = await JsonSerializer.DeserializeAsync<Model.Config>(stream, jsonOptions);
                // json読み込み
                LoadModel(model);
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
