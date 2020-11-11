using CustomControls.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CustomControls
{
    [TemplatePart(Name = TemplateContainerName, Type = typeof(Border))]
    [TemplatePart(Name = TitleBarBackgroundName, Type = typeof(Rectangle))]
    [TemplatePart(Name = TitleBarContainerName, Type = typeof(Grid))]
    [TemplatePart(Name = IconPresenterName, Type = typeof(Image))]
    [TemplatePart(Name = TitlePresenterName, Type = typeof(TextBlock))]
    public class CustomWindow : Window
    {
        #region Fields

        private const string TemplateContainerName = "PART_TemplateContainerBorder";
        private const string TitleBarBackgroundName = "PART_TitleBarBackgroundRectangle";
        private const string TitleBarContainerName = "PART_TitleBarContainerGrid";
        private const string IconPresenterName = "PART_IconImage";
        private const string TitlePresenterName = "PART_TitleTextBlock";

        #endregion

        #region Constructors

        static CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow),
                new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        public CustomWindow()
        {
            CommandBindings.Add(new CommandBinding(CloseCommand, HandleClsoeCommand));
            CommandBindings.Add(new CommandBinding(MaximizeCommand, HandleMaximizeCommand));
            CommandBindings.Add(new CommandBinding(MinimizeCommand, HandleMinimizeCommand));
        }

        #endregion

        #region Commands

        public static RoutedCommand CloseCommand = new RoutedCommand();
        public static RoutedCommand MaximizeCommand = new RoutedCommand();
        public static RoutedCommand MinimizeCommand = new RoutedCommand();

        #endregion

        #region Command Handlers

        private void HandleClsoeCommand(object sender, ExecutedRoutedEventArgs e)
            => SystemCommands.CloseWindow(this);

        private void HandleMaximizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                SystemCommands.MaximizeWindow(this);
            else
                SystemCommands.RestoreWindow(this);
        }

        private void HandleMinimizeCommand(object sender, ExecutedRoutedEventArgs e)
            => SystemCommands.MinimizeWindow(this);

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CheckIconAndTitle();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

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

        #region Helper methods

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
            //  If we don't remove one pixel from height of the window,
            //  our custom window will block auto-hide taskbar.
            //  Not works properly when GlassFrameThickness set to 0
            height = removeOnePixel ? height - 1 : height;

            NativeMethods.SetWindowPos(currentWindowHandle, 0, x, y, width, height, NativeMethods.SwpShowWindow);
        }

        protected virtual void CheckIconAndTitle()
        {
            var titlebarContainer = GetTemplateChild(TitleBarContainerName) as Grid;

            if (Icon == null)
            {
                var iconImage = GetTemplateChild(IconPresenterName) as Image;
                titlebarContainer?.Children.Remove(iconImage);
            }

            if (string.IsNullOrWhiteSpace(Title))
            {
                var titleTextBlock = GetTemplateChild(TitlePresenterName) as TextBlock;
                titlebarContainer?.Children.Remove(titleTextBlock);
            }
        }

        #endregion

        #region DependencyProperty : ActiveTitleBarBackground

        public static readonly DependencyProperty ActiveTitleBarBackgroundProperty =
            DependencyProperty.Register(nameof(ActiveTitleBarBackground),
                typeof(Brush), typeof(CustomWindow),
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
                typeof(Brush), typeof(CustomWindow),
                new PropertyMetadata(default(Brush)));

        public Brush InactiveTitleBarBackground
        {
            get => (Brush)GetValue(InactiveTitleBarBackgroundProperty);
            set => SetValue(InactiveTitleBarBackgroundProperty, value);
        }

        #endregion

        #region DependencyProperty : ButtonForeground

        public static readonly DependencyProperty ButtonForegroundProperty =
            DependencyProperty.Register(nameof(ButtonForeground),
                typeof(Brush), typeof(CustomWindow),
                new PropertyMetadata(default(Brush)));

        public Brush ButtonForeground
        {
            get => (Brush)GetValue(ButtonForegroundProperty);
            set => SetValue(ButtonForegroundProperty, value);
        }

        #endregion

        #region DependencyProperty : ButtonMouseOverBackground

        public static readonly DependencyProperty ButtonMouseOverBackgroundProperty =
            DependencyProperty.Register(nameof(ButtonMouseOverBackground),
                typeof(Brush), typeof(CustomWindow),
                new PropertyMetadata(default(Brush)));

        public Brush ButtonMouseOverBackground
        {
            get => (Brush)GetValue(ButtonMouseOverBackgroundProperty);
            set => SetValue(ButtonMouseOverBackgroundProperty, value);
        }

        #endregion

        #region DependencyProperty : ButtonMouseOverForeground

        public static readonly DependencyProperty ButtonMouseOverForegroundProperty =
            DependencyProperty.Register(nameof(ButtonMouseOverForeground),
                typeof(Brush), typeof(CustomWindow),
                new PropertyMetadata(default(Brush)));

        public Brush ButtonMouseOverForeground
        {
            get => (Brush)GetValue(ButtonMouseOverForegroundProperty);
            set => SetValue(ButtonMouseOverForegroundProperty, value);
        }

        #endregion
    }
}
