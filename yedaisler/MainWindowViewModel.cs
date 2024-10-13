using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using yedaisler.Behaviors;
using yedaisler.Utility;
using static yedaisler.Utility.WindowsApi;

namespace yedaisler
{
    // 表示制御
    public enum BoxDisplayMode
    {
        System,     // システム共通表示
        TaskState,  // タスク状態表示
        MultiTaskState, // 複数タスク表示
    }

    internal class MainWindowViewModel : BindableBase, IDisposable
    {
        // 
        public ReactivePropertySlim<int> DispBoxFontSize { get; private set; }
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

        // 機能制御フラグ
        public ReactivePropertySlim<bool> BlockShutdown { get; private set; }
        public ReactivePropertySlim<bool> BlockSleep { get; private set; }



        // Window
        MainWindow window;
        // Notifier画面
        Notifier.Notifier notifier;


        // 表示BOX情報
        public ReactivePropertySlim<string> Disp {  get; set; }
        //public ReactivePropertySlim<double> DispWidth { get; set; }
        public ReactiveCommand DispSizeChangedCommand { get; private set; }
        public ReactiveCommand ContentRenderedCommand { get; private set; }

        public ReactivePropertySlim<BoxDisplayMode> DispBoxMode { get; set; }
        public ReactivePropertySlim<Visibility> DispBoxSingle {  get; set; }
        public ReactivePropertySlim<Visibility> DispBoxMulti { get; set; }

        public ReactivePropertySlim<double> DispBoxMultiWidth { get; set; }
        public ReactivePropertySlim<double> DispBoxMultiAnimeDuration { get; set; }
        public ReactivePropertySlim<bool> DispBoxMultiActive { get; set; }
        // BOX表示設定ToDoカウント
        private double dispBoxSingleWidth;
        private int dispInBoxCount;
        ToDo.Item boxDisplayTaskRef;

        public ReactivePropertySlim<double> Width { get; set; }
        public ReactivePropertySlim<double> Height { get; set; }

        public ReactivePropertySlim<double> SnapDistH { get; set; }
        public ReactivePropertySlim<double> SnapDistV { get; set; }
        public ReactivePropertySlim<Behaviors.SnapWnd2ScrLocation> SnapLocation { get; set; }

        public ReactiveCollection<ToDo.Item> ToDos { get; set; }
        public ReactivePropertySlim<ToDo.State> State { get; set; }

        // SystemCommandハンドラ
        public ReactiveCommand OnMenuSystemCommand { get; set; }
        public ReactiveCommand OnMenuAppExit { get; set; }

        public ObservableCollection<Utility.IConcatTextItem> MultiDispToDos {  get; set; }

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

            DispBoxFontSize = new ReactivePropertySlim<int>(14);
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

            SnapDistH = new ReactivePropertySlim<double>(1);
            SnapDistH.AddTo(Disposables);
            SnapDistV = new ReactivePropertySlim<double>(1);
            SnapDistV.AddTo(Disposables);
            SnapLocation = new ReactivePropertySlim<SnapWnd2ScrLocation>(SnapWnd2ScrLocation.Any);
            SnapLocation.AddTo(Disposables);

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


            // BOX表示モード
            dispBoxSingleWidth = 0.0;
            dispInBoxCount = 0;
            boxDisplayTaskRef = null;
            DispBoxMultiWidth = new ReactivePropertySlim<double>(120);
            DispBoxMultiWidth.AddTo(Disposables);
            DispBoxMultiAnimeDuration = new ReactivePropertySlim<double>(4.0);
            DispBoxMultiAnimeDuration.AddTo(Disposables);
            DispBoxMultiActive = new ReactivePropertySlim<bool>(false);
            DispBoxMultiActive.AddTo(Disposables);
            DispBoxSingle = new ReactivePropertySlim<Visibility>(Visibility.Visible);
            DispBoxSingle.AddTo(Disposables);
            DispBoxMulti = new ReactivePropertySlim<Visibility>(Visibility.Collapsed);
            DispBoxMulti.AddTo(Disposables);
            DispBoxMode = new ReactivePropertySlim<BoxDisplayMode>(BoxDisplayMode.System);
            DispBoxMode.Subscribe(x =>
            {
                switch (x)
                {
                    case BoxDisplayMode.System:
                    case BoxDisplayMode.TaskState:
                        DispBoxSingle.Value = Visibility.Visible;
                        DispBoxMulti.Value = Visibility.Collapsed;
                        break;

                    case BoxDisplayMode.MultiTaskState:
                        DispBoxSingle.Value = Visibility.Hidden;
                        DispBoxMulti.Value = Visibility.Visible;
                        break;

                    default:
                        // できる？
                        DispBoxMode.Value = BoxDisplayMode.System;
                        break;
                }
                //
                UpdateDispBoxWidth();
            })
            .AddTo(Disposables);

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
                if (x is TextBlock obj && DispBoxMode.Value != BoxDisplayMode.MultiTaskState)
                {
                    // ウインドウサイズ更新
                    dispBoxSingleWidth = obj.ActualWidth + 16;
                    UpdateDispBoxWidth();
                }
            });
            DispSizeChangedCommand.AddTo(Disposables);
            // 
            State = new ReactivePropertySlim<ToDo.State>(ToDo.State.Done);
            State.Subscribe(state =>
            {
                UpdateBoxDisplay();
            })
            .AddTo(Disposables);

            OnMenuSystemCommand = new ReactiveCommand();
            OnMenuSystemCommand.Subscribe(x =>
            {
                if (x is Menu.SystemCommand.CommandMode mode)
                {
                    switch (mode)
                    {
                        case Menu.SystemCommand.CommandMode.ShowNotifyWindow:
                            ShowNotifier();
                            break;

                        case Menu.SystemCommand.CommandMode.None:
                        default:
                            // nothing
                            break;
                    }
                }
            });
            OnMenuAppExit = new ReactiveCommand();
            OnMenuAppExit.Subscribe(() =>
            {
                // App終了
                System.Windows.Application.Current.Shutdown((int)System.Windows.ShutdownMode.OnExplicitShutdown);
            });

            // 
            MultiDispToDos = new ObservableCollection<IConcatTextItem>();
            //
            ToDos = new ReactiveCollection<ToDo.Item>();
            ToDos.ObserveElementProperty(x => x.State.Value).Subscribe(x =>
            {
                if (x.Instance is ToDo.Item item)
                {
                    // Notify処理
                    if (item.ActiveStateInfo.Value.StateInfoRef.Notify.Active)
                    {
                        Notifier.Log.NotifierLog.Data.Add(new Notifier.NotifyItem
                        {
                            Text = $"Exec Action: {item.ActiveStateInfo.Value.Name.Value}",
                            Object = item,
                            Type = Notifier.NotifyType.ToDoAction,
                        });
                    }
                    // MultiDisp処理
                    item.Text.Value = item.ActiveStateInfo.Value.Name.Value;
                    //
                    switch (item.State.Value)
                    {
                        case ToDo.State.Ready:
                            item.Background.Value = BrushBackReady.Value;
                            item.Foreground.Value = BrushFontReady.Value;
                            break;

                        case ToDo.State.Doing:
                            item.Background.Value = BrushBackDoing.Value;
                            item.Foreground.Value = BrushFontDoing.Value;
                            break;

                        case ToDo.State.Done:
                            item.Background.Value = BrushBackDone.Value;
                            item.Foreground.Value = BrushFontDone.Value;
                            break;

                        default:
                            item.Background.Value = BrushBackNone.Value;
                            item.Foreground.Value = BrushFontNone.Value;
                            break;
                    }
                }
                //
                UpdateTotalStateByEachStateChange();
            });
            ToDos.ObserveElementProperty(x => x.DisplayInBox.Value).Subscribe(x =>
            {
                UpdateDisplayInBox(x.Instance, x.Value);
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

        public void Init(MainWindow wnd, Notifier.Notifier notifier_, Config.Config config_)
        {
            window = wnd;

            //
            notifier = notifier_;
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
                // todo作成
                var todo = new ToDo.Item(c_todo);
                todo.Init();
                ToDos.Add(todo);
            }

            // WindowMessageハンドル
            hwndSource = Utility.WindowsApi.GetHwndSource(wnd);
            hwndSource.AddHook(WndProcHandler);
        }

        public void UpdateSnapLocation(Behaviors.SnapWnd2ScrLocation loc)
        {
            // 上下左右辺でなければ更新無し
            if (loc == SnapWnd2ScrLocation.Any) return;
            // 指定されたSnap先に合わせてウインドウ座標を更新
            // ウインドウが存在するスクリーンを取得
            // DPIから物理ピクセルへ変換する行列を取得してそれぞれの長さを変換
            var mat = PresentationSource.FromVisual(window).CompositionTarget.TransformToDevice;
            var wndPos = mat.Transform(new System.Windows.Point(window.Left, window.Top));
            var wndScr = System.Windows.Forms.Screen.GetWorkingArea(new System.Drawing.Point((int)wndPos.X, (int)wndPos.Y));
            // 位置調整
            if ((loc & SnapWnd2ScrLocation.Top) != 0)
            {
                window.Top = wndScr.Top / mat.M22;
            }
            if ((loc & SnapWnd2ScrLocation.Bottom) != 0)
            {
                window.Top = wndScr.Bottom / mat.M22 - window.Height;
            }
            if ((loc & SnapWnd2ScrLocation.Left) != 0)
            {
                window.Left = wndScr.Left / mat.M11;
            }
            if ((loc & SnapWnd2ScrLocation.Right) != 0)
            {
                window.Left = wndScr.Right / mat.M11 - window.Width;
            }

        }

        private void UpdateTotalStateByEachStateChange()
        {
            // ToDo全体状態を更新する
            var notify = false;
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
                // Notify
                if (todo.ActiveStateInfo.Value.StateInfoRef.Notify.Active)
                {
                    notify = true;
                }
                //
                block.Shutdown |= todo.ActiveStateInfo.Value.StateInfoRef.Block.Shutdown;
                block.Sleep |= todo.ActiveStateInfo.Value.StateInfoRef.Block.Sleep;
            }
            // 表示情報更新
            MultiDispToDos.Clear();
            boxDisplayTaskRef = null;
            foreach (var item in ToDos)
            {
                if (CheckIsDisplayInBox(item))
                {
                    MultiDispToDos.Add((IConcatTextItem)item);
                    boxDisplayTaskRef = item;
                }
            }
            //
            switch (DispBoxMode.Value)
            {
                case BoxDisplayMode.TaskState:
                    // SingleToDo表示ではそのToDoの表示情報に従う
                    State.Value = boxDisplayTaskRef.State.Value;
                    break;

                case BoxDisplayMode.MultiTaskState:
                    State.Value = ToDo.State.None;
                    break;

                case BoxDisplayMode.System:
                default:
                    // System表示ではReady>Doing>Doneの優先順位
                    // MultiToDo表示はこの情報を使わないので適当に更新
                    State.Value = state;
                    break;
            }

            //
            if (notify)
            {
                ShowNotifier();
            }
            //
            BlockShutdown.Value = block.Shutdown;
            BlockSleep.Value = block.Sleep;
        }

        private void UpdateDispBoxWidth()
        {
            switch (DispBoxMode.Value)
            {
                case BoxDisplayMode.MultiTaskState:
                    Width.Value = DispBoxMultiWidth.Value;
                    break;

                case BoxDisplayMode.System:
                case BoxDisplayMode.TaskState:
                default:
                    Width.Value = dispBoxSingleWidth;
                    break;

            }

            // ウインドウサイズとSnapLocationに合わせて表示位置調整
            UpdateSnapLocation(SnapLocation.Value);
        }

        private void ShowNotifier()
        {
            notifier.Owner = window;
            notifier.Show();
            //notifier.ShowDialog();
        }

        private void UpdateTotalState()
        {
            if (ToDos.Count == 0)
            {
                State.Value = ToDo.State.None;
            }
        }

        private void UpdateBoxDisplay()
        {
            switch (DispBoxMode.Value)
            {
                case BoxDisplayMode.TaskState:
                    UpdateBoxDisplayTaskState();
                    break;

                case BoxDisplayMode.MultiTaskState:
                    // MultiToDo表示では
                    break;

                case BoxDisplayMode.System:
                default:
                    UpdateBoxDisplaySystem();
                    break;
            }
        }
        private void UpdateBoxDisplaySystem()
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
        private void UpdateBoxDisplayTaskState()
        {
            Disp.Value = boxDisplayTaskRef.ActiveStateInfo.Value.Name.Value;
        }

        private void UpdateDisplayInBox(ToDo.Item todo, bool value)
        {
            // 表示ToDo数を事前にチェック
            //if (value) dispInBoxCount++;
            //else dispInBoxCount--;
            dispInBoxCount = 0;
            foreach (var item in ToDos)
            {
                if (CheckIsDisplayInBox(item)) dispInBoxCount++;
            }

            switch (dispInBoxCount)
            {
                case 0:
                    DispBoxMode.Value = BoxDisplayMode.System;
                    DispBoxMultiActive.Value = false;
                    break;
                case 1:
                    DispBoxMode.Value = BoxDisplayMode.TaskState;
                    DispBoxMultiActive.Value = false;
                    break;
                default:
                    DispBoxMode.Value = BoxDisplayMode.MultiTaskState;
                    DispBoxMultiActive.Value = true;
                    break;
            }
            // System表示とSingleToDo表示で表示内容が異なるケースがあるため更新
            UpdateTotalStateByEachStateChange();
            UpdateBoxDisplay();
        }
        private bool CheckIsDisplayInBox(ToDo.Item item)
        {
            bool result = false;

            if (item.DisplayInBox.Value)
            {
                // Doneは表示しない
                switch (item.State.Value)
                {
                    case ToDo.State.Ready:
                    case ToDo.State.Doing:
                        result = true;
                        break;

                    case ToDo.State.Done:
                    case ToDo.State.None:
                    default:
                        break;
                }
            }

            return result;
        }

        public void ExecAction()
        {
            switch (DispBoxMode.Value)
            {
                case BoxDisplayMode.TaskState:
                    boxDisplayTaskRef.StateAction();
                    break;

                case BoxDisplayMode.MultiTaskState:
                    foreach (var item in MultiDispToDos)
                    {
                        var todo = item as ToDo.Item;
                        todo.StateAction();
                    }
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

            //window.Topmost = false;
            //window.Topmost = true;
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
