using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace yedaisler.Notifier
{
    /// <summary>
    /// Notifier.xaml の相互作用ロジック
    /// </summary>
    public partial class Notifier : Window
    {
        public Notifier()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (this.Log.Items is INotifyCollectionChanged log)
            {
                log.CollectionChanged += (object isender, NotifyCollectionChangedEventArgs ie) =>
                {
                    this.LogScrollbar.ScrollToEnd();
                };
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem)
            {
                if (listBoxItem.DataContext is NotifyItem item)
                {
                    if (!(item.Object is null))
                    {
                        //
                        switch (item.Type)
                        {
                            case NotifyType.ToDoAction:
                                if (item.Object is ToDo.Item todo)
                                {
                                    todo.StateAction();
                                }
                                break;

                            case NotifyType.None:
                            default:
                                // nothing
                                break;
                        }
                        //
                        item.IsNotified = true;
                    }
                    //
                    yedaisler.Notifier.Log.NotifierLog.Data.Remove(item);
                }
            }

            //
            if (yedaisler.Notifier.Log.NotifierLog.Data.Count == 0)
            {
                Hide();
            }
        }
    }
}
