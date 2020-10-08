using CustomControls.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace CustomControls
{
    public class CustomWindow : Window
    {
        #region Fields

        private const string TemplateContainerName = "PART_TemplateContainerBorder";
        private const string TitleBarContainerName = "PART_TitleBarContainerGrid";
        private const string IconPresenterName = "PART_IconImage";
        private const string TitlePresenterName = "PART_TitleTextBlock";
        private const string WindowCloseButtonName = "PART_WindowCloseButton";
        private const string WindowMaximizeButtonName = "PART_WindowMaximizeButton";
        private const string WindowMinimizeButtonName = "PART_WindowMinimizeButton";

        private Button _windowCloseButton;
        private Button _windowMaximizeButton;
        private Button _windowMinimizeButton;

        #endregion

        #region Constructors

        static CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow),
                new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            UnregisterClickHandlers();
            LoadButtonsTemplate();
            RegisterClickHandlers();
            CheckIconAndTitle();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (!(GetTemplateChild(TemplateContainerName) is Border templateContainerBorder))
                return;

            if (WindowState == WindowState.Maximized)
            {
                var removeOnePixel = true;

                if (IsTaskbarVisible())
                {
                    templateContainerBorder.Padding = new Thickness(7);
                    removeOnePixel = false;
                }

                AdjustWindowPosition(removeOnePixel);

            }
            else if (WindowState == WindowState.Normal)
            {
                templateContainerBorder.Padding = new Thickness(0);
            }
        }

        #endregion

        #region Methods

        protected virtual bool IsTaskbarVisible()
            => NativeMethods.GetTaskbarState() != NativeMethods.TaskbarStates.AutoHide;

        protected virtual NativeMethods.MonitorInfo GetMonitorInfo()
        {
            var monitorHandle = NativeMethods.GetCurrentMonitor();
            var monitorInfo = new NativeMethods.MonitorInfo();

            NativeMethods.GetMonitorInfo(monitorHandle, monitorInfo);

            return monitorInfo;
        }

        protected virtual void AdjustWindowPosition(bool removeOnePixel)
        {
            var currentWindowHandle = new WindowInteropHelper(this).EnsureHandle();
            var monitorInfo = GetMonitorInfo();
            var x = monitorInfo.rcWork.left;
            var y = monitorInfo.rcWork.top;
            var width = Math.Abs(monitorInfo.rcWork.right - monitorInfo.rcWork.left);
            var height = Math.Abs(monitorInfo.rcWork.bottom - monitorInfo.rcWork.top);

            // NOTE:
            //  To adjust window position when auto-hide taskbar enabled. 
            //  If we don't remove one pixel from bottom of the window,
            //  our custom window will block auto-hide taskbar.
            height = removeOnePixel ? height - 1 : height;

            NativeMethods.SetWindowPos(currentWindowHandle, 0, x, y, width, height, NativeMethods.SwpShowWindow);
        }

        protected virtual void LoadButtonsTemplate()
        {
            _windowCloseButton = (Button)GetTemplateChild(WindowCloseButtonName)
                ?? throw new NullReferenceException("Close button template not found.");

            _windowMinimizeButton = (Button)GetTemplateChild(WindowMinimizeButtonName)
                ?? throw new NullReferenceException("Minimize button template not found.");

            _windowMaximizeButton = (Button)GetTemplateChild(WindowMaximizeButtonName)
                ?? throw new NullReferenceException("Maximize button template not found.");
        }

        protected virtual void RegisterClickHandlers()
        {
            _windowCloseButton.Click += CloseWindow;
            _windowMinimizeButton.Click += MinimizeWindow;
            _windowMaximizeButton.Click += MaximizeWindow;
        }

        protected virtual void UnregisterClickHandlers()
        {
            if (_windowCloseButton != null)
                _windowCloseButton.Click -= CloseWindow;

            if (_windowMinimizeButton != null)
                _windowMinimizeButton.Click -= MinimizeWindow;

            if (_windowMaximizeButton != null)
                _windowMaximizeButton.Click -= MaximizeWindow;
        }

        protected virtual void CheckIconAndTitle()
        {
            var TitleBarContainer = GetTemplateChild(TitleBarContainerName) as Grid;

            if (Icon == null)
            {
                var iconImage = GetTemplateChild(IconPresenterName) as Image;
                TitleBarContainer?.Children.Remove(iconImage);
            }

            if (string.IsNullOrWhiteSpace(Title))
            {
                var titleTextBlock = GetTemplateChild(TitlePresenterName) as TextBlock;
                TitleBarContainer?.Children.Remove(titleTextBlock);
            }
        }

        #endregion

        #region Event Handlers

        protected virtual void CloseWindow(object sender, EventArgs args)
            => Close();

        protected virtual void MinimizeWindow(object sender, EventArgs args)
            => WindowState = WindowState.Minimized;

        protected virtual void MaximizeWindow(object sender, EventArgs args)
            => WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;

        #endregion

        #region DependencyProperty : ActiveTitleBarBackground

        public static readonly DependencyProperty ActiveTitleBarBackgroundProperty =
            DependencyProperty.Register(nameof(ActiveTitleBarBackground),
                typeof(Brush), typeof(Window),
                new PropertyMetadata(default(Brush)));

        public Brush ActiveTitleBarBackground
        {
            get => (Brush)GetValue(ActiveTitleBarBackgroundProperty);
            set => SetValue(ActiveTitleBarBackgroundProperty, value);
        }

        #endregion

        #region DependencyProperty : InactiveTitleBarBackground

        public static readonly DependencyProperty InactiveTitleBarBackgroundProperty =
            DependencyProperty.Register(nameof(InactiveTitleBarBackground),
                typeof(Brush), typeof(Window),
                new PropertyMetadata(default(Brush)));

        public Brush InactiveTitleBarBackground
        {
            get => (Brush)GetValue(InactiveTitleBarBackgroundProperty);
            set => SetValue(InactiveTitleBarBackgroundProperty, value);
        }

        #endregion

        #region DependencyProperty : WindowButtonMouseOverBackground

        public static readonly DependencyProperty WindowButtonMouseOverBackgroundProperty =
            DependencyProperty.Register(nameof(WindowButtonMouseOverBackground),
                typeof(Brush), typeof(Window),
                new PropertyMetadata(default(Brush)));

        public Brush WindowButtonMouseOverBackground
        {
            get => (Brush)GetValue(WindowButtonMouseOverBackgroundProperty);
            set => SetValue(WindowButtonMouseOverBackgroundProperty, value);
        }

        #endregion

        #region DependencyProperty : WindowButtonMouseOverForeground

        public static readonly DependencyProperty WindowButtonMouseOverForegroundProperty =
            DependencyProperty.Register(nameof(WindowButtonMouseOverForeground),
                typeof(Brush), typeof(Window),
                new PropertyMetadata(default(Brush)));

        public Brush WindowButtonMouseOverForeground
        {
            get => (Brush)GetValue(WindowButtonMouseOverForegroundProperty);
            set => SetValue(WindowButtonMouseOverForegroundProperty, value);
        }

        #endregion
    }
}
