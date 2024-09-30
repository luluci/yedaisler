using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using yedaisler.ToDo;
using yedaisler.Utility;

namespace yedaisler.Config
{
    internal class Gui : BindableBase
    {
        public ReactivePropertySlim<GuiColor> Color { get; set; }

        public Gui() {
            Color = new ReactivePropertySlim<GuiColor>(new GuiColor());
        }
    }

    internal class GuiColor : BindableBase
    {
        public ReactivePropertySlim<string> FontReady { get; set; }
        public ReactivePropertySlim<string> FontDoing { get; set; }
        public ReactivePropertySlim<string> FontDone { get; set; }
        public ReactivePropertySlim<string> BackReady { get; set; }
        public ReactivePropertySlim<string> BackDoing { get; set; }
        public ReactivePropertySlim<string> BackDone { get; set; }

        //
        public ReactivePropertySlim<SolidColorBrush> BrushFontReady { get; set; }
        public ReactivePropertySlim<SolidColorBrush> BrushFontDoing { get; set; }
        public ReactivePropertySlim<SolidColorBrush> BrushFontDone { get; set; }
        public ReactivePropertySlim<SolidColorBrush> BrushBackReady { get; set; }
        public ReactivePropertySlim<SolidColorBrush> BrushBackDoing { get; set; }
        public ReactivePropertySlim<SolidColorBrush> BrushBackDone { get; set; }


        public GuiColor() {
            FontReady = new ReactivePropertySlim<string>();
            FontDoing = new ReactivePropertySlim<string>();
            FontDone = new ReactivePropertySlim<string>();
            BackReady = new ReactivePropertySlim<string>();
            BackDoing = new ReactivePropertySlim<string>();
            BackDone = new ReactivePropertySlim<string>();
            //
            BrushFontReady = new ReactivePropertySlim<SolidColorBrush>();
            BrushFontDoing = new ReactivePropertySlim<SolidColorBrush>();
            BrushFontDone = new ReactivePropertySlim<SolidColorBrush>();
            BrushBackReady = new ReactivePropertySlim<SolidColorBrush>();
            BrushBackDoing = new ReactivePropertySlim<SolidColorBrush>();
            BrushBackDone = new ReactivePropertySlim<SolidColorBrush>();
        }
    }


    internal class ToDo : BindableBase
    {
        public ReactivePropertySlim<string> Name { get; set; }
        public ReactivePropertySlim<bool> DisplayInBox { get; set; }

        public ReactivePropertySlim<ToDoStateInfo> Ready { get; set; }
        public ReactivePropertySlim<ToDoStateInfo> Doing { get; set; }
        public ReactivePropertySlim<ToDoStateInfo> Done { get; set; }


        public ToDo()
        {
            Name = new ReactivePropertySlim<string>("");
            // 制御フラグ
            // デスクトップに表示するBOXに状態を表示する
            DisplayInBox = new ReactivePropertySlim<bool>();
            //
            Ready = new ReactivePropertySlim<ToDoStateInfo>();
            Doing = new ReactivePropertySlim<ToDoStateInfo>();
            Done = new ReactivePropertySlim<ToDoStateInfo>();
        }
    }
    internal class ToDoStateInfo : BindableBase
    {
        public ReactivePropertySlim<string> Name { get; set; }
        public ReactivePropertySlim<yedaisler.ToDo.ActionMode> Mode { get; set; }
        public ReactivePropertySlim<ToDoAction> Action { get; set; }
        public BlockInfo Block { get; set; }

        // Actionなし用共通オブジェクト
        private static ToDoAction ActionNone = new ToDoAction();

        public ToDoStateInfo()
        {
            Name = new ReactivePropertySlim<string>(string.Empty);
            Mode = new ReactivePropertySlim<yedaisler.ToDo.ActionMode>(yedaisler.ToDo.ActionMode.None);
            Action = new ReactivePropertySlim<ToDoAction>(ActionNone);
            Block = new BlockInfo();
        }

        public bool ExecAction()
        {
            return Action.Value.Action.Exec();
        }
    }

    internal class ToDoAction : BindableBase
    {
        public ReactivePropertySlim<string> Name { get; set; }
        public ReactivePropertySlim<yedaisler.ToDo.ActionType> Type { get; set; }
        public IToDoAction Action { get; set; }


        internal static ToDoActionNone ActionNone = new ToDoActionNone();

        public ToDoAction()
        {
            Name = new ReactivePropertySlim<string>(string.Empty);
            Type = new ReactivePropertySlim<yedaisler.ToDo.ActionType>(yedaisler.ToDo.ActionType.None);
            Action = ActionNone;
        }
    }

    internal interface IToDoAction
    {
        bool Exec();
    }
    internal class ToDoActionNone : IToDoAction
    {
        public bool Exec() { return true; }
    }

    internal class ToDoActionShellExecute : BindableBase, IToDoAction
    {
        public ReactivePropertySlim<string> Path { get; set; }
        public ToDoActionShellExecute()
        {
            Path = new ReactivePropertySlim<string>();
        }

        public bool Exec()
        {
            try
            {
                ProcessStartInfo pi = new ProcessStartInfo()
                {
                    FileName = Path.Value,
                    UseShellExecute = true,
                };

                var proc = Process.Start(pi);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    internal class BlockInfo
    {
        public bool Shutdown { get; set; }
        public bool Sleep { get; set; }

        public BlockInfo()
        {
            Shutdown = false;
            Sleep = false;
        }
    }


    internal partial class ConfigViewModel
    {
        public void LoadModel(Model.Config config)
        {
            // Model(json)からViewModel(Reactive)を作成する
            // Gui
            MakeGui(config.Gui);
            // ToDos
            foreach (var action in config.ToDos)
            {
                var todo = MakeToDo(action);
                ToDos.Add(todo);
            }
        }

        private void MakeGui(Model.Gui gui)
        {
            if (gui is null)
            {
                gui = new Model.Gui();
            }

            // Color
            MakeGuiColor(gui.Color);
        }
        private void MakeGuiColor(Model.Color color)
        {
            if (color is null)
            {
                color = new Model.Color();
            }

            //
            var colorRef = Gui.Value.Color.Value;
            colorRef.FontReady.Value = color.FontReady;
            colorRef.FontDoing.Value = color.FontDoing;
            colorRef.FontDone.Value = color.FontDone;
            colorRef.BackReady.Value = color.BackReady;
            colorRef.BackDoing.Value = color.BackDoing;
            colorRef.BackDone.Value = color.BackDone;
            //
            colorRef.BrushFontReady.Value = colorRef.FontReady.Value.ToSolidColorBrush();
            colorRef.BrushFontDoing.Value = colorRef.FontDoing.Value.ToSolidColorBrush();
            colorRef.BrushFontDone.Value = colorRef.FontDone.Value.ToSolidColorBrush();
            colorRef.BrushBackReady.Value = colorRef.BackReady.Value.ToSolidColorBrush();
            colorRef.BrushBackDoing.Value = colorRef.BackDoing.Value.ToSolidColorBrush();
            colorRef.BrushBackDone.Value = colorRef.BackDone.Value.ToSolidColorBrush();
        }

        private ToDo MakeToDo(Model.ToDo m_todo)
        {
            var todo = new ToDo();
            // Name
            todo.Name.Value = m_todo.Name;
            // Flags
            todo.DisplayInBox.Value = m_todo.DisplayInBox;
            // Actions
            todo.Ready.Value = MakeToDoStateInfo(m_todo.Ready, "Ready");
            todo.Doing.Value = MakeToDoStateInfo(m_todo.Doing, "Doing");
            todo.Done.Value = MakeToDoStateInfo(m_todo.Done, "Done");

            return todo;
        }

        private ToDoStateInfo MakeToDoStateInfo(Model.ToDoStateInfo m_info, string name)
        {
            var info = new ToDoStateInfo();

            if (m_info is null)
            {
                m_info = new Model.ToDoStateInfo();
            }

            // 表示名
            if (System.Object.ReferenceEquals(m_info.Name, string.Empty))
            {
                info.Name.Value = name;
            }
            else
            {
                info.Name.Value = m_info.Name;
            }
            // Mode設定
            switch (m_info.Mode)
            {
                case "exec":
                    info.Mode.Value = yedaisler.ToDo.ActionMode.Exec;
                    break;

                default:
                    // 
                    break;
            }
            // Action
            info.Action.Value = MakeToDoAction(m_info.Action);
            // Block
            if (!(m_info.Block is null))
            {
                info.Block.Shutdown = m_info.Block.Shutdown;
                info.Block.Sleep = m_info.Block.Sleep;
            }

            return info;
        }

        private ToDoAction MakeToDoAction(Model.ToDoAction m_action)
        {
            var action = new ToDoAction();

            //
            if (m_action is null)
            {
                m_action = new Model.ToDoAction();
            }

            // Name設定
            if (!System.Object.ReferenceEquals(m_action.Name, string.Empty))
            {
                action.Name.Value = m_action.Name;
            }
            // Type設定
            switch (m_action.Type)
            {
                case "openUrl":
                    if (m_action.OpenUrl != null)
                    {
                        action.Type.Value = yedaisler.ToDo.ActionType.OpenUrl;
                        action.Action = MakeToDoActionShellExecute(m_action.OpenUrl.Url);
                    }
                    if (System.Object.ReferenceEquals(action.Name.Value, string.Empty))
                    {
                        action.Name.Value = "OpenURL";
                    }
                    break;

                case "shellExecute":
                    if (m_action.ShellExecute != null)
                    {
                        action.Type.Value = yedaisler.ToDo.ActionType.shellExecute;
                        action.Action = MakeToDoActionShellExecute(m_action.ShellExecute.Path);
                    }
                    if (System.Object.ReferenceEquals(action.Name.Value, string.Empty))
                    {
                        action.Name.Value = "ShellExe";
                    }
                    break;

                default:
                    break;
            }

            return action;
        }

        private IToDoAction MakeToDoActionShellExecute(string path)
        {
            var act = new ToDoActionShellExecute();
            act.Path.Value = path;
            return act;
        }
    }
}
