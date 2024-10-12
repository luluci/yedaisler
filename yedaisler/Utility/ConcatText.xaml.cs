using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static yedaisler.Utility.DummyTextItem;

namespace yedaisler.Utility
{
    public interface IConcatTextItem
    {
        ReactivePropertySlim<string> Text { get; set; }
        ReactivePropertySlim<SolidColorBrush> Background { get; set; }
        ReactivePropertySlim<SolidColorBrush> Foreground { get; set; }
    }

    /// <summary>
    /// ConcatText.xaml の相互作用ロジック
    /// </summary>
    public partial class ConcatText : UserControl
    {
        public ObservableCollection<IConcatTextItem> ItemsSource
        {
            get { return (ObservableCollection<IConcatTextItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<IConcatTextItem>), typeof(ConcatText), new FrameworkPropertyMetadata(default(ObservableCollection<IConcatTextItem>), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool Active
        {
            get { return (bool)GetValue(ActiveProperty); }
            set { SetValue(ActiveProperty, value); }
        }
        public static readonly DependencyProperty ActiveProperty =
            DependencyProperty.Register(nameof(Active), typeof(bool), typeof(ConcatText), new PropertyMetadata(false, ActiveChangedCallback));

        private static void ActiveChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ConcatText self)
            {
                self.OnActiveChanged();
            }
        }

        bool hasUpdateReq;
        double animeWidth;
        Storyboard storyboard;

        public ConcatText()
        {
            InitializeComponent();

            //ItemsSource = new ObservableCollection<IConcatTextItem>();
            //ItemsSource.Add(new DummyTextItem { Text = "dummy1" });
            //ItemsSource.Add(new DummyTextItem { Text = "dummy2" });

            hasUpdateReq = false;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!Active)
            {
                hasUpdateReq = true;
                return;
            }

            // 表示画面のサイズ変更によるアニメーション更新
            UpdateAnime();

            hasUpdateReq = false;
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!Active)
            {
                hasUpdateReq = true;
                return;
            }

            // 表示内容変更によるアニメーション更新
            if (sender is StackPanel tgt && tgt.Tag is ObservableCollection<IConcatTextItem> collection)
            {
                // 先頭要素と同じ内容を末尾に追加して、
                // GUIとしては必ず2つ以上のTextBlockを生成する構成になっている
                if (tgt.Children.Count > 0)
                {
                    // 表示内容からアニメーションする内容のWidthを取得
                    animeWidth = 0.0;
                    for (var i = 0; i < collection.Count; i++)
                    {
                        if (tgt.Children[i] is FrameworkElement txt)
                        {
                            animeWidth += txt.ActualWidth;
                        }
                    }
                    // アニメーション設定更新
                    UpdateAnime();
                }
            }

            hasUpdateReq = false;
        }

        private void UpdateAnime()
        {
            var dispWidth = canvas.ActualWidth;
            // 
            // TextBlockセットがcanvasサイズより大きいときにアニメーションする
            if (dispWidth < animeWidth)
            {
                //末尾要素が左端に到達したタイミングで表示位置をリセットする
                var anime = new DoubleAnimation
                {
                    From = 0,
                    To = -animeWidth,
                    Duration = TimeSpan.FromSeconds(5),
                    RepeatBehavior = RepeatBehavior.Forever,
                };
                Storyboard.SetTarget(anime, disp);
                Storyboard.SetTargetProperty(anime, new PropertyPath("(Canvas.Left)"));
                storyboard = new Storyboard();
                storyboard.Children.Add(anime);
                if (Active)
                {
                    storyboard.Begin();
                }
            }
            else
            {
                if (!(storyboard is null))
                {
                    storyboard.Stop();
                }
            }
        }

        private void OnActiveChanged()
        {
            if (Active)
            {
                if (!(storyboard is null))
                {
                    storyboard.Begin();
                }
            }
            else
            {
                if (!(storyboard is null))
                {
                    storyboard.Stop();
                }
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }

    internal class DummyTextItem : IConcatTextItem
    {
        public static System.Windows.Media.Color DefaultColor = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#FF000000");
        public static SolidColorBrush DefaultBrush = new SolidColorBrush(DefaultColor);

        public ReactivePropertySlim<string> Text { get; set; } = new ReactivePropertySlim<string>(string.Empty);
        public ReactivePropertySlim<SolidColorBrush> Background { get; set; } = new ReactivePropertySlim<SolidColorBrush>(DefaultBrush);
        public ReactivePropertySlim<SolidColorBrush> Foreground { get; set; } = new ReactivePropertySlim<SolidColorBrush>(DefaultBrush);
    }

    internal enum DispType
    {
        Null,
        Plus1,
        Twice,
    }
    internal class DispTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<IConcatTextItem> cont)
            {
                if (cont.Count < 100)
                {
                    return DispType.Twice;
                }
                else
                {
                    return DispType.Plus1;
                }
            }

            return DispType.Null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = 0;
            i++;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
