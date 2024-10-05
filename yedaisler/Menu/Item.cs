using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace yedaisler.Menu
{
    internal enum ItemType
    {
        None,
        Command,
        SystemMenu,
        SystemAppState,
        SystemAppExit,
        SystemCommand,
        Label,
        Separator,
        //
        RootMenuHeader,
        ToDoHeader,
        SystemHeader,
        ToDo,
        ToDoAction,
        ToDoManualHeader,
        ToDoManualAction,
        ToDoManualState,
        ToDoDispInBox,
    }

    internal class SystemMenu
    {
        public ReactivePropertySlim<ToDo.State> State { get; set; }

        public SystemMenu()
        {
            // Dummy
            State = new ReactivePropertySlim<ToDo.State>(ToDo.State.None);
        }
    }
    internal class SystemAppState
    {

    }
    internal class SystemAppExit
    {

    }
    internal class SystemCommand
    {
        public enum CommandMode
        {
            None,
            ShowNotifyWindow,
        }

        public string Header { get; set; } = "SystemCommand";

        public CommandMode Mode { get; set; } = CommandMode.None;
    }

    internal class Command
    {
        public string Name { get; set; } = string.Empty;
        public ItemType Type { get; set; } = ItemType.Separator;
        public bool IsInited { get; set; } = false;

        public ReactiveCommand Command1 { get; set; }

        public Command()
        {
            Name = "command";

            Command1 = new ReactiveCommand();
            Command1.Subscribe(x =>
            {
                int i = 0;
                i++;
            });
        }
    }

    internal class Separator
    {
        public string Name { get; set; } = string.Empty;
        public ItemType Type { get; set; } = ItemType.Separator;
    }

    internal class Label
    {
        public string Name { get; set; } = string.Empty;
    }

    internal class RootMenuHeader
    {
        public ItemType Type { get; set; } = ItemType.RootMenuHeader;

        public string Name { get; set; } = "Main Menu Header";

        public RootMenuHeader()
        {
            Name = "MainMenuHeader";
        }
    }
    internal class ToDoHeader
    {
        public ItemType Type { get; set; } = ItemType.ToDoHeader;

        public string Caption { get; set; } = "状態:";
    }



    internal class ToDoAction
    {
        public string Name { get; set; } = string.Empty;
        public ItemType Type { get; set; } = ItemType.ToDoAction;
    }

    internal class ToDoManualAction
    {
        public ReactivePropertySlim<ToDo.State> State { get; set; }

        public ToDoManualAction()
        {
            // Dummy
            State = new ReactivePropertySlim<ToDo.State>(ToDo.State.None);
        }
    }

    internal class ToDoManualState
    {
        public ReactivePropertySlim<ToDo.State> State { get; set; }

        public ToDoManualState()
        {
            // Dummy
            State = new ReactivePropertySlim<ToDo.State>(ToDo.State.None);
        }
    }

    internal class ToDoDispInBox
    {

    }

}
