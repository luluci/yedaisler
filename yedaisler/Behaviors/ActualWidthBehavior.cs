using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace yedaisler.Behaviors
{
    internal class ActualWidthBehavior : Behavior<TextBlock>
    {
        public double ActualWidth
        {
            get => (double)GetValue(ActualWidthProperty);
            set => SetValue(ActualWidthProperty, value);
        }
        public static readonly DependencyProperty ActualWidthProperty =
            DependencyProperty.Register("ActualWidth", typeof(double), typeof(ActualWidthBehavior), new PropertyMetadata(50.0));


        #region EnableResize 依存関係プロパティ
        public bool EnableResize
        {
            get { return (bool)GetValue(EnableResizeProperty); }
            set { SetValue(EnableResizeProperty, value); }
        }
        public static readonly DependencyProperty EnableResizeProperty =
            DependencyProperty.Register("EnableResize", typeof(bool), typeof(SnapWnd2ScrBehavior), new PropertyMetadata(true));
        #endregion

        protected override void OnAttached()
        {
            AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
        }

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ActualWidth = AssociatedObject.ActualWidth;
        }
    }

    internal class SizeChangedCommand : ICommand
    {
        MainWindowViewModel m_vm;
        public SizeChangedCommand(MainWindowViewModel vm)
        {
            m_vm = vm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
