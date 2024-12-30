using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace yedaisler.Utility
{
    /// <summary>
    /// Screen(a.k.a. ディスプレイ,モニタ)に関する情報を取得する
    /// </summary>
    public class Screen
    {

        static public Point GetElemPosOnWindow(Window wnd, UIElement ui)
        {
            var pt = ui.PointToScreen(new Point(0.0d, 0.0d));
            var transform = PresentationSource.FromVisual(wnd).CompositionTarget.TransformFromDevice;
            return transform.Transform(pt);
        }

        /// <summary>
        /// wnd内のuiを基準にpopupを表示する位置を計算する
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="ui"></param>
        /// <returns></returns>
        static public Point GetPopupPos(Window wnd, FrameworkElement ui, Window popup)
        {
            var scr = GetScreenFromWindow(wnd);
            var pt = ui.PointToScreen(new Point(0.0d, 0.0d));
            var transform = PresentationSource.FromVisual(wnd).CompositionTarget.TransformFromDevice;
            var pos = transform.Transform(pt);

            // X軸チェック
            // はみ出すなら右基準に表示
            if (pos.X + popup.Width > scr.Right)
            {
                pos.X = (pos.X + ui.ActualWidth) - popup.Width;
            }
            // それでもはみ出すなら画面右端合わせ
            if (pos.X < scr.Left)
            {
                pos.X = scr.Right - popup.Width;
            }

            // Y軸チェック
            if (pos.Y + ui.ActualHeight + popup.Height <= scr.Bottom)
            {
                // 下側に表示して収まる
                // 下側に表示
                pos.Y = pos.Y + ui.ActualHeight;
            }
            else if (pos.Y - popup.Height >= scr.Top)
            {
                // 上側に表示して収まる
                // 上側に表示
                pos.Y = pos.Y - popup.Height;
            }
            else
            {
                // それでもはみ出すなら画面下端合わせ
                pos.Y = scr.Bottom - popup.Height;
            }

            return pos;
        }

        static public Rect GetScreenFromWindow(Window wnd)
        {
            var rect = new System.Drawing.Rectangle((int)wnd.Left, (int)wnd.Top, (int)wnd.Width, (int)wnd.Height);
            var screen = System.Windows.Forms.Screen.FromRectangle(rect);
            return new Rect(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);
        }
    }
}
