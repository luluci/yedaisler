using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using yedaisler.ToDo;

namespace yedaisler.Config
{
    internal class ToDo
    {
        public ReactivePropertySlim<string> Name { get; set; }

        public ReactivePropertySlim<ToDoStateInfo> Ready { get; set; }
        public ReactivePropertySlim<ToDoStateInfo> Doing { get; set; }
        public ReactivePropertySlim<ToDoStateInfo> Done { get; set; }


        public ToDo()
        {
            Name = new ReactivePropertySlim<string>("");
            Ready = new ReactivePropertySlim<ToDoStateInfo>();
            Doing = new ReactivePropertySlim<ToDoStateInfo>();
            Done = new ReactivePropertySlim<ToDoStateInfo>();
        }
    }
    internal class ToDoStateInfo
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
    }

    internal class ToDoAction
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

    internal class ToDoActionShellExecute : IToDoAction
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
            foreach (var action in config.ToDos)
            {
                var todo = MakeToDo(action);
                ToDos.Add(todo);
            }
        }

        private ToDo MakeToDo(Model.ToDo m_todo)
        {
            var todo = new ToDo();
            // Name
            todo.Name.Value = m_todo.Name;
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
