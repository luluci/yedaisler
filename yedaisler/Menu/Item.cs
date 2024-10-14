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
        // Dummy
        public ReactivePropertySlim<ToDo.State> State { get; set; } = new ReactivePropertySlim<ToDo.State>(ToDo.State.None);

        static public ReactiveCommand OnMenuStateAction { get; set; } = new ReactiveCommand();
    }
    internal class SystemAppState
    {
        // Dummy
        static public ReactiveCommand OnMenuStateAction { get; set; } = new ReactiveCommand();
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
            ShowConfig,
        }

        public string Header { get; set; } = "SystemCommand";

        public CommandMode Mode { get; set; } = CommandMode.None;
    }

    internal class Command
    {
        public string Name { get; set; } = "command";
    }

    internal class Separator
    {
    }

    internal class Label
    {
        public string Name { get; set; } = string.Empty;
    }

    internal class RootMenuHeader
    {
        public string Name { get; set; } = "MainMenuHeader";

    }
    internal class ToDoHeader
    {
    }



    internal class ToDoAction
    {
        // dummy
        static public ReactivePropertySlim<ToDoActionDummy> ActiveStateInfo { get; set; } = new ReactivePropertySlim<ToDoActionDummy>(new ToDoActionDummy());
    }
    internal class ToDoActionDummy
    {
        // dummy
        static public ReactivePropertySlim<string> ActionName { get; set; } = new ReactivePropertySlim<string>(string.Empty);
    }

    internal class ToDoManualAction
    {
        // Dummy
        public ReactivePropertySlim<ToDo.State> State { get; set; } = new ReactivePropertySlim<ToDo.State>(ToDo.State.None);

        static public ReactiveCommand OnMenuStateAction { get; set; } = new ReactiveCommand();
    }

    internal class ToDoManualState
    {
        // dummy
        public ReactivePropertySlim<ToDo.State> State { get; set; } = new ReactivePropertySlim<ToDo.State>(ToDo.State.None);

        static public ReactiveCommand OnMenuStateAction { get; set; } = new ReactiveCommand();
    }

    internal class ToDoDispInBox
    {

    }

}
