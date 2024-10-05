using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace yedaisler.Notifier
{
    using LogInstance = yedaisler.Notifier.Log;

    internal class NotifierViewModel
    {

        // Logコンテナ参照
        public ReactiveCollection<string> Log { get; set; }

        //
        public ReactiveCommand OnClickLogCopy { get; set; }
        public ReactiveCommand OnClickLogClear { get; set; }

        public NotifierViewModel()
        {
            //
            Log = LogInstance.NotifierLog.Data;
            //
            OnClickLogCopy = new ReactiveCommand();
            OnClickLogCopy.Subscribe(x =>
            {
                if (x is string log)
                {
                    Clipboard.SetText(log);
                }
            });
            OnClickLogClear = new ReactiveCommand();
            OnClickLogClear.Subscribe(x =>
            {
                Log.Clear();
            });
        }
    }
}
