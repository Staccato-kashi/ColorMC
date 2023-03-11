using Avalonia.Controls;
using Avalonia.Input;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UI.Controls;
using System;

namespace ColorMC.Gui.UI.Windows;

public partial class SelfBaseWindow : Window, IBaseWindow
{
    Info3Control IBaseWindow.Info3 => Info3;
    Info1Control IBaseWindow.Info1 => Info1;
    Info4Control IBaseWindow.Info => Info;
    Info2Control IBaseWindow.Info2 => Info2;
    Info6Control IBaseWindow.Info6 => Info6;
    HeadControl IBaseWindow.Head => Head;
    Info5Control IBaseWindow.Info5 => Info5;
    UserControl IBaseWindow.Con => (Main as UserControl)!;

    public IUserControl? Main;

    public SelfBaseWindow() : this(null)
    {

    }

    public SelfBaseWindow(IUserControl? con)
    {
        Main = con;

        InitializeComponent();

        if (SystemInfo.Os == OsType.Linux)
        {
            SystemDecorations = SystemDecorations.BorderOnly;
            var rectangle = Border1;
            var window = this;
            Border1.PointerPressed += (sender, e) =>
            {
                if (e.GetCurrentPoint(rectangle).Properties.IsLeftButtonPressed)
                {
                    var point = e.GetPosition(rectangle);
                    var arg1 = point.X / rectangle.Bounds.Width;
                    var arg2 = point.Y / rectangle.Bounds.Height;
                    if (arg1 > 0.95)
                    {
                        if (arg2 > 0.95)
                        {
                            window.BeginResizeDrag(WindowEdge.SouthEast, e);
                        }
                        else if (arg2 <= 0.95)
                        {
                            window.BeginResizeDrag(WindowEdge.East, e);
                        }
                    }
                    else if (arg1 < 0.05)
                    {
                        if (arg2 <= 0.95)
                        {
                            window.BeginResizeDrag(WindowEdge.West, e);
                        }
                        else if (arg2 > 0.95)
                        {
                            window.BeginResizeDrag(WindowEdge.SouthWest, e);
                        }
                    }
                    else if (arg2 > 0.95)
                    {
                        window.BeginResizeDrag(WindowEdge.South, e);
                    }
                }
            };
        }

        if (SystemInfo.Os == OsType.MacOS)
        {
            KeyDown += Window_KeyDown;
        }

        Icon = App.Icon;

        if (App.BackBitmap != null)
        {
            Image_Back.Source = App.BackBitmap;
        }

        if (Main is UserControl con1)
        {
            MainControl.Children.Add(con1);
        }

        Closed += UserWindow_Closed;
        Opened += UserWindow_Opened;
        Activated += Window_Activated;

        App.PicUpdate += Update;

        FindGoodPos();

        Update();
    }

    public void SetTitle(string temp)
    {
        Head.Title = Title = temp;
    }

    private void FindGoodPos()
    {
        var basewindow = App.LastWindow;

        if (basewindow == null || basewindow.PlatformImpl == null)
            return;

        var pos = basewindow.Position;
        var sec = basewindow.Screens.ScreenFromWindow(basewindow.PlatformImpl);
        if (sec == null)
            return;
        var area = sec.WorkingArea;
        int x, y;
        if (pos.X > area.Width / 2)
        {
            x = pos.X - 100;
        }
        else
        {
            x = pos.X + 100;
        }

        if (pos.Y > area.Height / 2)
        {
            y = pos.Y - 40;
        }
        else
        {
            y = pos.Y + 40;
        }

        Position = new(x, y);
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        var window = (sender as Window)!;
        if (e.KeyModifiers == KeyModifiers.Control)
        {
            switch (e.Key)
            {
                case Key.OemComma:
                    App.ShowSetting(SettingType.Normal);
                    break;
                case Key.Q:
                    App.Close();
                    break;
                case Key.M:
                    window.WindowState = WindowState.Minimized;
                    break;
                case Key.W:
                    window.Close();
                    break;
            }
        }
    }

    private void UserWindow_Opened(object? sender, EventArgs e)
    {
        Main?.Opened();
    }
    private void UserWindow_Closed(object? sender, EventArgs e)
    {
        App.PicUpdate -= Update;

        Main?.Closed();

        Main = null;
        MainControl.Children.Clear();

        if (App.LastWindow == this)
        {
            App.LastWindow = null;
        }

        Funtcions.RunGC();
    }
    private void Window_Activated(object? sender, EventArgs e)
    {
        App.LastWindow = this;
    }
    private void Update()
    {
        App.Update(this, Image_Back, Border1, Border2);

        Main?.Update();
    }
}
