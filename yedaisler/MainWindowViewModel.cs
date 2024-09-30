using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using yedaisler.Behaviors;
using yedaisler.Utility;
using static yedaisler.Utility.WindowsApi;

namespace yedaisler
{
    internal class MainWindowViewModel : BindableBase, IDisposable
    {
        // Window
        MainWindow window;

        // メイン画面
        public ReactivePropertySlim<string> Disp {  get; set; }
        //public ReactivePropertySlim<double> DispWidth { get; set; }
        public ReactiveCommand DispSizeChangedCommand { get; private set; }
        public ReactiveCommand ContentRenderedCommand { get; private set; }

        public ReactivePropertySlim<double> Width { get; set; }
        public ReactivePropertySlim<double> Height { get; set; }

        public ReactivePropertySlim<double> SnapDistH { get; set; }
        public ReactivePropertySlim<double> SnapDistV { get; set; }

        public ReactiveCollection<ToDo.Item> ToDos { get; set; }
        public ReactivePropertySlim<ToDo.State> State { get; set; }

        public ReactiveCommand AppExitCommand { get; set; }

        // 色設定
        public ReactivePropertySlim<SolidColorBrush> BrushBackMenu { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushBaseFont { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushFontLabel { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushBackLabel { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushFontReady { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushFontDoing { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushFontDone { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushBackReady { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushBackDoing { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushBackDone { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushBackSystemHeader { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushBackNone { get; private set; }
        public ReactivePropertySlim<SolidColorBrush> BrushFontNone { get; private set; }

        // 表示制御
        enum BoxDisplayMode
        {
            System,     // システム共通表示
            TaskState,  // タスク状態表示
        }
        BoxDisplayMode boxDisplayMode;
        ToDo.Item boxDisplayTaskRef;

        // 機能制御フラグ
        public ReactivePropertySlim<bool> BlockShutdown { get; private set; }
        public ReactivePropertySlim<bool> BlockSleep { get; private set; }

        // Config関連
        Config.Config config;
        Config.ConfigViewModel config_vm;

        // WindowMessage操作
        HwndSource hwndSource;

        // 周期処理機能
        //Observable.Timer cycleProc;
        System.Windows.Threading.DispatcherTimer cycleProc;
        // Option機能
        public ReactivePropertySlim<string> Clock {  get; private set; }

        public ReactiveCommand Command2 { get; private set; }
        public ReactivePropertySlim<bool> ContextMenuIsOpen { get; private set; }

        public MainWindowViewModel() {
            Command2 = new ReactiveCommand();
            Command2.Subscribe(x =>
            {
                int i = 0;
                i++;
            });
            ContextMenuIsOpen = new ReactivePropertySlim<bool>(false, ReactivePropertyMode.DistinctUntilChanged);
            ContextMenuIsOpen.Subscribe(x =>
            {
                int i = 0;
                i++;
            });

            // 表示制御
            boxDisplayMode = BoxDisplayMode.System;
            boxDisplayTaskRef = null;

            // メイン画面
            Disp = new ReactivePropertySlim<string>("");
            Disp.AddTo(Disposables);
            //DispWidth = new ReactivePropertySlim<double>(0.0);
            //DispWidth.Subscribe(x =>
            //{
            //    int i = 0;
            //    i++;
            //});
            //DispWidth.AddTo(Disposables);
            ContentRenderedCommand = new ReactiveCommand();
            ContentRenderedCommand.Subscribe(x =>
            {
                // Window描画後イベント
                // ToDoアイテムゼロによる表示更新処理
                // Loadedイベント時はWindowが描画されてなくて表示更新できなかった
                if (ToDos.Count == 0)
                {
                    UpdateTotalState();
                }
            })
            .AddTo(Disposables);
            DispSizeChangedCommand = new ReactiveCommand();
            DispSizeChangedCommand.Subscribe(x =>
            {
                if (x is TextBlock obj)
                {
                    Width.Value = obj.ActualWidth + 16;
                }
            });
            DispSizeChangedCommand.AddTo(Disposables);
            // 
            State = new ReactivePropertySlim<ToDo.State>(ToDo.State.Done);
            State.Subscribe(state =>
            {
                updateBoxDisplay();
            })
            .AddTo(Disposables);

            SnapDistH = new ReactivePropertySlim<double>(1);
            SnapDistH.AddTo(Disposables);
            SnapDistV = new ReactivePropertySlim<double>(1);
            SnapDistV.AddTo(Disposables);

            Width = new ReactivePropertySlim<double>(50);
            Width.Subscribe(x =>
            {
                SnapDistH.Value = x / 2;
            })
            .AddTo(Disposables);
            Height = new ReactivePropertySlim<double>(30);
            Height.Subscribe(x =>
            {
                SnapDistV.Value = x / 2;
            })
            .AddTo(Disposables);

            AppExitCommand = new ReactiveCommand();
            AppExitCommand.Subscribe(() =>
            {
                // App終了
                System.Windows.Application.Current.Shutdown((int)System.Windows.ShutdownMode.OnExplicitShutdown);
            });

            // Color
            BrushBackMenu = new ReactivePropertySlim<SolidColorBrush>("#A0202040".ToSolidColorBrush());
            BrushBackMenu.AddTo(Disposables);
            BrushBaseFont = new ReactivePropertySlim<SolidColorBrush>("#FFFFFFFF".ToSolidColorBrush());
            BrushBaseFont.AddTo(Disposables);
            // Label
            BrushFontLabel = new ReactivePropertySlim<SolidColorBrush>("#FFC0C0C0".ToSolidColorBrush());
            BrushFontLabel.AddTo(Disposables);
            BrushBackLabel = new ReactivePropertySlim<SolidColorBrush>("#A0202020".ToSolidColorBrush());
            BrushBackLabel.AddTo(Disposables);
            // ToDo Stateに対応したカラー
            BrushFontReady = new ReactivePropertySlim<SolidColorBrush>("#FFFFFFFF".ToSolidColorBrush());
            BrushFontReady.AddTo(Disposables);
            BrushFontDoing = new ReactivePropertySlim<SolidColorBrush>("#FF202020".ToSolidColorBrush());
            BrushFontDoing.AddTo(Disposables);
            BrushFontDone = new ReactivePropertySlim<SolidColorBrush>("#FFFFFFFF".ToSolidColorBrush());
            BrushFontDone.AddTo(Disposables);
            BrushBackReady = new ReactivePropertySlim<SolidColorBrush>("#A0FF0000".ToSolidColorBrush());
            BrushBackReady.AddTo(Disposables);
            BrushBackDoing = new ReactivePropertySlim<SolidColorBrush>("#A0FFFF00".ToSolidColorBrush());
            BrushBackDoing.AddTo(Disposables);
            BrushBackDone = new ReactivePropertySlim<SolidColorBrush>("#A0202020".ToSolidColorBrush());
            BrushBackDone.AddTo(Disposables);
            BrushBackSystemHeader = new ReactivePropertySlim<SolidColorBrush>("#FF202020".ToSolidColorBrush());
            BrushBackSystemHeader.AddTo(Disposables);
            // デフォルトカラー
            BrushBackNone = new ReactivePropertySlim<SolidColorBrush>("#10202020".ToSolidColorBrush());
            BrushBackNone.AddTo(Disposables);
            BrushFontNone = new ReactivePropertySlim<SolidColorBrush>("#FFFFFFFF".ToSolidColorBrush());
            BrushFontNone.AddTo(Disposables);

            //
            BlockShutdown = new ReactivePropertySlim<bool>(false);
            BlockShutdown.AddTo(Disposables);
            BlockSleep = new ReactivePropertySlim<bool>(false);
            BlockSleep.AddTo(Disposables);

            //
            ToDos = new ReactiveCollection<ToDo.Item>();
            ToDos.ObserveElementProperty(x => x.State.Value).Subscribe(x =>
            {
                UpdateTotalStateByEachStateChange();
            });
            ToDos.ObserveElementProperty(x => x.DisplayInBox.Value).Subscribe(x =>
            {
                updateDisplayInBox(x.Instance, x.Value);
            });
            ToDos.AddTo(Disposables);

            //
            Clock = new ReactivePropertySlim<string>("xx:xx:xx");
            Clock.AddTo(Disposables);

            //
            cycleProc = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Normal);
            cycleProc.Interval = new TimeSpan(0, 0, 1);
            cycleProc.Tick += cycleProcHandler;
            cycleProc.Start();
        }

        public void Init(MainWindow wnd, Config.Config config_)
        {
            window = wnd;

            // Configへの参照を記憶
            config = config_;
            config_vm = config.DataContext as Config.ConfigViewModel;

            // Config.Gui.Colorを制御データに展開
            BrushFontReady.Value = config_vm.Gui.Value.Color.Value.BrushFontReady.Value;
            BrushFontDoing.Value = config_vm.Gui.Value.Color.Value.BrushFontDoing.Value;
            BrushFontDone.Value = config_vm.Gui.Value.Color.Value.BrushFontDone.Value;
            BrushBackReady.Value = config_vm.Gui.Value.Color.Value.BrushBackReady.Value;
            BrushBackDoing.Value = config_vm.Gui.Value.Color.Value.BrushBackDoing.Value;
            BrushBackDone.Value = config_vm.Gui.Value.Color.Value.BrushBackDone.Value;
            // Config.ToDoを制御用データに変換
            foreach (var c_todo in config_vm.ToDos)
            {
                var todo = new ToDo.Item(c_todo);
                todo.Init();
                ToDos.Add(todo);
            }

            // WindowMessageハンドル
            hwndSource = Utility.WindowsApi.GetHwndSource(wnd);
            hwndSource.AddHook(WndProcHandler);
        }

        private void UpdateTotalStateByEachStateChange()
        {
            // ToDo全体状態を更新する
            var block = new Config.BlockInfo();
            ToDo.State state = ToDo.State.Done;
            foreach (var todo in ToDos)
            {
                if (todo.State.Value != ToDo.State.None)
                {
                    if ((int)todo.State.Value < (int)state)
                    {
                        state = todo.State.Value;
                    }
                }
                //
                block.Shutdown |= todo.ActiveStateInfo.Value.StateInfoRef.Block.Shutdown;
                block.Sleep |= todo.ActiveStateInfo.Value.StateInfoRef.Block.Sleep;
            }
            //
            switch (boxDisplayMode)
            {
                case BoxDisplayMode.TaskState:
                    State.Value = boxDisplayTaskRef.State.Value;
                    break;

                case BoxDisplayMode.System:
                default:
                    State.Value = state;
                    break;
            }

            BlockShutdown.Value = block.Shutdown;
            BlockSleep.Value = block.Sleep;
        }

        private void UpdateTotalState()
        {
            if (ToDos.Count == 0)
            {
                State.Value = ToDo.State.None;
            }
        }

        private void updateBoxDisplay()
        {
            switch (boxDisplayMode)
            {
                case BoxDisplayMode.TaskState:
                    updateBoxDisplayTaskState();
                    break;

                case BoxDisplayMode.System:
                default:
                    updateBoxDisplaySystem();
                    break;
            }
        }
        private void updateBoxDisplaySystem()
        {
            switch (State.Value)
            {
                case ToDo.State.Ready:
                    Disp.Value = "Readyタスクあり";
                    break;

                case ToDo.State.Doing:
                    Disp.Value = "Doing";
                    break;

                case ToDo.State.Done:
                    Disp.Value = "全タスクDone";
                    break;

                default:
                    Disp.Value = "タスクなし";
                    break;
            }
        }
        private void updateBoxDisplayTaskState()
        {
            Disp.Value = boxDisplayTaskRef.ActiveStateInfo.Value.Name.Value;
        }

        private void updateDisplayInBox(ToDo.Item todo, bool value)
        {
            // todo: 複数チェックされたら、全部を流し表示する
            // 現状で一つだけチェック有効とする
            if (value)
            {
                // その他の項目のチェックを下す
                bool eventCheck = false;
                foreach (var item in ToDos)
                {
                    if (!Object.ReferenceEquals(todo, item) && item.DisplayInBox.Value == true)
                    {
                        // 注意:false設定によるイベントが発生する＝本関数のfalse側パスに入る
                        item.DisplayInBox.Value = false;
                        eventCheck = true;
                    }
                }
                // falseに変更していたらイベントが発生しているのでそちらで処置が入る
                // falseへの変更がなければここで処理する
                if (!eventCheck)
                {
                    boxDisplayTaskRef = todo;
                    boxDisplayMode = BoxDisplayMode.TaskState;
                }
            }
            else
            {
                boxDisplayTaskRef = null;
                boxDisplayMode = BoxDisplayMode.System;
                foreach (var item in ToDos)
                {
                    if (item.DisplayInBox.Value)
                    {
                        boxDisplayTaskRef = item;
                    }
                }
                if (!(boxDisplayTaskRef is null))
                {
                    boxDisplayMode = BoxDisplayMode.TaskState;
                }
                else
                {
                    UpdateTotalStateByEachStateChange();
                }
            }
            //
            updateBoxDisplay();
        }

        public void ExecAction()
        {
            switch (boxDisplayMode)
            {
                case BoxDisplayMode.TaskState:
                    boxDisplayTaskRef.StateAction();
                    break;

                case BoxDisplayMode.System:
                default:
                    foreach (var todo in ToDos)
                    {
                        todo.StateAction();
                    }
                    break;
            }
        }

        private void cycleProcHandler(object sender, EventArgs e)
        {
            Clock.Value = DateTime.Now.ToString("HH:mm:ss");
        }

        public IntPtr WndProcHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case (int)WindowMessage.WM_QUERYENDSESSION:
                    handled = true;
                    if (BlockShutdown.Value)
                    {
                        // シャットダウンをキャンセルする
                        return (IntPtr)SessionEnding.Cancel;
                    }
                    else
                    {
                        // シャットダウンをキャンセルしない
                        return (IntPtr)SessionEnding.Ok;
                    }

                default:
                    break;
            }

            return IntPtr.Zero;
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
