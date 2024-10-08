﻿using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace yedaisler.Behaviors
{
    internal enum SnapWnd2ScrLocation
    {
        Any         = 0,                // 任意座標
        Top         = 0x01,             // 上辺(左端右端以外)
        Bottom      = 0x02,             // 下辺(左端右端以外)
        Left        = 0x04,             // 左辺(上端下端以外)
        Right       = 0x08,             // 右辺(上端下端以外)
        TopLeft     = Top | Left,       // 左上
        TopRight    = Top | Right,      // 右上
        BottomLeft  = Bottom | Left,    // 左下
        BottomRight = Bottom | Right,   // 右下
    }

    internal class SnapWnd2ScrBehavior : Behavior<Window>
    {
        #region SnapDistanceHorizontal 依存関係プロパティ
        public double SnapDistanceHorizontal
        {
            get { return (double)GetValue(SnapDistanceHorizontalProperty); }
            set { SetValue(SnapDistanceHorizontalProperty, value); }
        }
        public static readonly DependencyProperty SnapDistanceHorizontalProperty =
            DependencyProperty.Register("SnapDistanceHorizontal", typeof(double), typeof(SnapWnd2ScrBehavior), new PropertyMetadata(50.0));
        #endregion

        #region SnapDistanceVertical 依存関係プロパティ
        public double SnapDistanceVertical
        {
            get { return (double)GetValue(SnapDistanceVerticalProperty); }
            set { SetValue(SnapDistanceVerticalProperty, value); }
        }
        public static readonly DependencyProperty SnapDistanceVerticalProperty =
            DependencyProperty.Register("SnapDistanceVertical", typeof(double), typeof(SnapWnd2ScrBehavior), new PropertyMetadata(50.0));
        #endregion

        #region EnableSnap 依存関係プロパティ
        public bool EnableSnap
        {
            get { return (bool)GetValue(EnableSnapProperty); }
            set { SetValue(EnableSnapProperty, value); }
        }
        public static readonly DependencyProperty EnableSnapProperty =
            DependencyProperty.Register("EnableSnap", typeof(bool), typeof(SnapWnd2ScrBehavior), new PropertyMetadata(true));
        #endregion

        #region SnapLocation
        public SnapWnd2ScrLocation SnapLocation
        {
            get { return (SnapWnd2ScrLocation)GetValue(SnapLocationProperty); }
            set { SetValue(SnapLocationProperty, value); }
        }
        public static readonly DependencyProperty SnapLocationProperty =
            DependencyProperty.Register("SnapLocation", typeof(SnapWnd2ScrLocation), typeof(SnapWnd2ScrBehavior), new PropertyMetadata(SnapWnd2ScrLocation.Any));
        #endregion

        protected override void OnAttached()
        {
            AssociatedObject.LocationChanged += AssociatedObject_LocationChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.LocationChanged -= AssociatedObject_LocationChanged;
        }

        void AssociatedObject_LocationChanged(object sender, EventArgs e)
        {
            if (!EnableSnap) { return; }

            var window = AssociatedObject;
            if (window.WindowState != WindowState.Normal) { return; }

            // DPIから物理ピクセルへ変換する行列を取得してそれぞれの長さを変換
            var mat = PresentationSource.FromVisual(window).CompositionTarget.TransformToDevice;
            var scaledTopLeft = mat.Transform(new Point(window.Left, window.Top));
            var scaledBottomRight = mat.Transform(new Point(window.Left + window.ActualWidth, window.Top + window.ActualHeight));
            var scaledSnapDisatance = mat.Transform(new Point(SnapDistanceHorizontal, SnapDistanceVertical));
            var scaledSize = scaledBottomRight - scaledTopLeft;

            //var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)scaledTopLeft.X, (int)scaledTopLeft.Y));
            var screen = System.Windows.Forms.Screen.GetWorkingArea(new System.Drawing.Point((int)scaledTopLeft.X, (int)scaledTopLeft.Y));
            var newTop = scaledTopLeft.Y;
            var newLeft = scaledTopLeft.X;

            //
            SnapWnd2ScrLocation snapLoc = SnapWnd2ScrLocation.Any;

            // 横方向の調整
            if (Math.Abs(screen.Left - scaledTopLeft.X) <= scaledSnapDisatance.X)
            {
                newLeft = screen.Left;
                snapLoc |= SnapWnd2ScrLocation.Left;
            }
            else if (Math.Abs(screen.Right - scaledBottomRight.X) <= scaledSnapDisatance.X)
            {
                newLeft = screen.Right - scaledSize.X;
                snapLoc |= SnapWnd2ScrLocation.Right;
            }

            // 縦方向の調整
            if (Math.Abs(screen.Top - scaledTopLeft.Y) <= scaledSnapDisatance.Y)
            {
                newTop = screen.Top;
                snapLoc |= SnapWnd2ScrLocation.Top;
            }
            else if (Math.Abs(screen.Bottom - scaledBottomRight.Y) <= scaledSnapDisatance.Y)
            {
                newTop = screen.Bottom - scaledSize.Y;
                snapLoc |= SnapWnd2ScrLocation.Bottom;
            }

            window.Left = newLeft / mat.M11;
            window.Top = newTop / mat.M22;
            SnapLocation = snapLoc;
        }
    }
}
