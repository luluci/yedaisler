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
    // 表示BOX:表示モード
    public enum DisplayBoxMode
    {
        System,     // システム共通表示
        SingleTask,  // タスク状態表示
        MultiTask, // 複数タスク表示
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

        // 表示BOXウインドウ設定
        public ReactivePropertySlim<double> Width { get; set; }
        public ReactivePropertySlim<double> Height { get; set; }

        public ReactivePropertySlim<double> SnapDistH { get; set; }
        public ReactivePropertySlim<double> SnapDistV { get; set; }
        public ReactivePropertySlim<Behaviors.SnapWnd2ScrLocation> SnapLocation { get; set; }

        // 表示BOX情報
        public ReactivePropertySlim<ToDo.State> DispBoxState { get; set; }
        public ReactivePropertySlim<string> DispBoxText {  get; set; }
        public ReactiveCommand DispSizeChangedCommand { get; private set; }
        public ReactiveCommand ContentRenderedCommand { get; private set; }

        public ReactivePropertySlim<DisplayBoxMode> DispBoxMode { get; set; }
        public ReactivePropertySlim<Visibility> DispBoxSingle {  get; set; }
        public ReactivePropertySlim<Visibility> DispBoxMulti { get; set; }

        public ReactivePropertySlim<double> DispBoxMultiWidth { get; set; }
        public ReactivePropertySlim<double> DispBoxMultiAnimeDuration { get; set; }
        public ReactivePropertySlim<bool> DispBoxMultiActive { get; set; }
        // BOX表示設定ToDoカウント
        private double dispBoxSingleWidth;
        ToDo.Item boxDisplayTaskRef;

        public ReactiveCollection<ToDo.Item> ToDos { get; set; }

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
            DispBoxMode = new ReactivePropertySlim<DisplayBoxMode>(DisplayBoxMode.System);
            DispBoxMode.Subscribe(x =>
            {
                switch (x)
                {
                    case DisplayBoxMode.System:
                    case DisplayBoxMode.SingleTask:
                        DispBoxSingle.Value = Visibility.Visible;
                        DispBoxMulti.Value = Visibility.Collapsed;
                        break;

                    case DisplayBoxMode.MultiTask:
                        DispBoxSingle.Value = Visibility.Hidden;
                        DispBoxMulti.Value = Visibility.Visible;
                        break;

                    default:
                        // できる？
                        DispBoxMode.Value = DisplayBoxMode.System;
                        break;
                }
                //
                UpdateDispBoxWidth();
            })
            .AddTo(Disposables);

            // メイン画面
            DispBoxText = new ReactivePropertySlim<string>("");
            DispBoxText.AddTo(Disposables);
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
                    // ウインドウサイズ更新
                    dispBoxSingleWidth = obj.ActualWidth + 16;

                    if (DispBoxMode.Value != DisplayBoxMode.MultiTask)
                    {
                        UpdateDispBoxWidth();
                    }
                }
            });
            DispSizeChangedCommand.AddTo(Disposables);
            // 
            DispBoxState = new ReactivePropertySlim<ToDo.State>(ToDo.State.Done);
            DispBoxState.Subscribe(state =>
            {
                // UpdateDisplayBox();
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

                        case Menu.SystemCommand.CommandMode.ShowConfig:
                            ShowConfig();
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
                // ToDoリスト内のある要素の状態が変化した
                // ToDo要素単体の情報更新を実行
                if (x.Instance is ToDo.Item item)
                {
                    UpdateEachStateInfo(item);
                }
                // ToDoリスト全体としての情報更新を実行
                UpdateTotalStateInfo();
                UpdateDisplayBox();
            });
            ToDos.ObserveElementProperty(x => x.DisplayInBox.Value).Subscribe(x =>
            {
                // System表示とSingleToDo表示で表示内容が異なるケースがあるため更新
                UpdateTotalStateInfo();
                UpdateDisplayBox();
            });
            ToDos.AddTo(Disposables);

            //
            Clock = new ReactivePropertySlim<string>("xx:xx:xx");
            Clock.AddTo(Disposables);

            //
            cycleProc = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Normal);
            cycleProc.Interval = new TimeSpan(0, 0, 1);
            cycleProc.Tick += CycleProcHandler;
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
            BrushFontReady.Value = config_vm.Gui.Color.Value.BrushFontReady.Value;
            BrushFontDoing.Value = config_vm.Gui.Color.Value.BrushFontDoing.Value;
            BrushFontDone.Value = config_vm.Gui.Color.Value.BrushFontDone.Value;
            BrushBackReady.Value = config_vm.Gui.Color.Value.BrushBackReady.Value;
            BrushBackDoing.Value = config_vm.Gui.Color.Value.BrushBackDoing.Value;
            BrushBackDone.Value = config_vm.Gui.Color.Value.BrushBackDone.Value;
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

        private void UpdateEachStateInfo(ToDo.Item item)
        {
            // Notify有効ならNotify表示作成
            if (item.ActiveStateInfo.Value.StateInfoRef.Notify.Active)
            {
                Notifier.Log.NotifierLog.Data.Add(new Notifier.NotifyItem
                {
                    Text = $"Exec Action: {item.ActiveStateInfo.Value.Name.Value}",
                    Object = item,
                    Type = Notifier.NotifyType.ToDoAction,
                });
                ShowNotifier();
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

        private void UpdateTotalStateInfo()
        {
            // ToDo全体状態を更新する

            // DisplayBoxMode: モード決定にToDoリスト全体が影響するのでこの中で判定する
            // DisplayInBox有効なToDoの数と、Multi表示有効なToDoの数をカウントする
            // Done状態のToDoは表示しないため個別にカウントする必要あり
            int dispBoxSimpleCount = 0;
            int dispBoxActualCount = 0;

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
                    // DisplayBoxMode
                    if (todo.DisplayInBox.Value)
                    {
                        dispBoxSimpleCount++;
                    }
                    if (CheckIsDisplayInBox(todo))
                    {
                        dispBoxActualCount++;
                    }
                }
                //
                block.Shutdown |= todo.ActiveStateInfo.Value.StateInfoRef.Block.Shutdown;
                block.Sleep |= todo.ActiveStateInfo.Value.StateInfoRef.Block.Sleep;
            }
            // DisplayBoxModeを先に更新
            // Multi表示のときにはMultiDispToDosの更新がアニメ表示要素に反映されるようにActiveにしておく
            switch (dispBoxActualCount)
            {
                case 0:
                    // 実カウント0のとき
                    // DisplayInBox設定は有効だが除外条件により除外されているケースがある
                    // DisplayInBox設定が1つだけならそれを表示する
                    // 0か2つ以上は共通表示にしておく
                    if (dispBoxSimpleCount == 1)
                    {
                        DispBoxMode.Value = DisplayBoxMode.SingleTask;
                        DispBoxMultiActive.Value = false;
                    }
                    else
                    {
                        DispBoxMode.Value = DisplayBoxMode.System;
                        DispBoxMultiActive.Value = false;
                    }
                    break;

                case 1:
                    // 実カウント1は単独表示
                    DispBoxMode.Value = DisplayBoxMode.SingleTask;
                    DispBoxMultiActive.Value = false;
                    break;

                default:
                    // 実カウント2以上はマルチ表示
                    DispBoxMode.Value = DisplayBoxMode.MultiTask;
                    DispBoxMultiActive.Value = true;
                    break;
            }
            // 表示情報更新
            MultiDispToDos.Clear();
            ToDo.Item singleDispRef = null;
            ToDo.Item multiDispRef = null;
            boxDisplayTaskRef = null;
            foreach (var item in ToDos)
            {
                if (item.DisplayInBox.Value)
                {
                    singleDispRef = item;
                }
                if (CheckIsDisplayInBox(item))
                {
                    MultiDispToDos.Add((IConcatTextItem)item);
                    multiDispRef = item;
                }
            }
            // DisplayInBox除外条件により何も選択されなかったときに
            // 単純条件で選択したToDoを単独表示するケースがある
            if (multiDispRef is null)
            {
                boxDisplayTaskRef = singleDispRef;
            }
            else
            {
                boxDisplayTaskRef = multiDispRef;
            }

            //
            switch (DispBoxMode.Value)
            {
                case DisplayBoxMode.SingleTask:
                    // SingleToDo表示ではそのToDoの表示情報に従う
                    DispBoxState.Value = boxDisplayTaskRef.State.Value;
                    break;

                case DisplayBoxMode.MultiTask:
                    DispBoxState.Value = ToDo.State.None;
                    break;

                case DisplayBoxMode.System:
                default:
                    // System表示ではReady>Doing>Doneの優先順位
                    // MultiToDo表示はこの情報を使わないので適当に更新
                    DispBoxState.Value = state;
                    break;
            }

            //
            BlockShutdown.Value = block.Shutdown;
            BlockSleep.Value = block.Sleep;
        }

        private void UpdateDispBoxWidth()
        {
            switch (DispBoxMode.Value)
            {
                case DisplayBoxMode.MultiTask:
                    Width.Value = DispBoxMultiWidth.Value;
                    break;

                case DisplayBoxMode.System:
                case DisplayBoxMode.SingleTask:
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

        private void ShowConfig()
        {
            config.Owner = window;
            config.Show();
        }

        private void UpdateTotalState()
        {
            if (ToDos.Count == 0)
            {
                DispBoxState.Value = ToDo.State.None;
            }
        }

        private void UpdateDisplayBox()
        {
            switch (DispBoxMode.Value)
            {
                case DisplayBoxMode.SingleTask:
                    UpdateBoxDisplayTaskState();
                    break;

                case DisplayBoxMode.MultiTask:
                    // MultiDispToDosに構築済み
                    break;

                case DisplayBoxMode.System:
                default:
                    UpdateBoxDisplaySystem();
                    break;
            }
        }
        private void UpdateBoxDisplaySystem()
        {
            switch (DispBoxState.Value)
            {
                case ToDo.State.Ready:
                    DispBoxText.Value = "Readyタスクあり";
                    break;

                case ToDo.State.Doing:
                    DispBoxText.Value = "Doing";
                    break;

                case ToDo.State.Done:
                    DispBoxText.Value = "全タスクDone";
                    break;

                default:
                    DispBoxText.Value = "タスクなし";
                    break;
            }
        }
        private void UpdateBoxDisplayTaskState()
        {
            DispBoxText.Value = boxDisplayTaskRef.ActiveStateInfo.Value.Name.Value;
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
                case DisplayBoxMode.SingleTask:
                    boxDisplayTaskRef.StateAction();
                    break;

                case DisplayBoxMode.MultiTask:
                    foreach (var item in MultiDispToDos)
                    {
                        var todo = item as ToDo.Item;
                        todo.StateAction();
                    }
                    break;

                case DisplayBoxMode.System:
                default:
                    foreach (var todo in ToDos)
                    {
                        todo.StateAction();
                    }
                    break;
            }
        }

        private void CycleProcHandler(object sender, EventArgs e)
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
