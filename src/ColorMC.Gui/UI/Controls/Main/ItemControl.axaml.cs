﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ColorMC.Core.Objs;
using ColorMC.Core.Objs.Login;
using ColorMC.Core.Utils;
using ColorMC.Gui.Objs;
using ColorMC.Gui.UIBinding;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Threading;

namespace ColorMC.Gui.UI.Controls.Main;

public partial class ItemControl : UserControl
{
    private MainControl Window;
    private LoginObj? Obj1;
    private GameSettingObj? Obj;
    private bool Launch;
    private bool isplay = true;
    public ItemControl()
    {
        InitializeComponent();

        Button_Launch.Click += Button_Launch_Click;
        Button_Edit.Click += Button_Edit_Click;
        Button_Add1.Click += Button_Add1_Click;

        Button_Switch.Click += Button_Switch_Click;

        Button_Setting.Click += Button_Setting_Click;

        Image1.PointerPressed += Image1_PointerPressed;

        Button1.Click += Button1_Click;
        Button2.Click += Button2_Click;

        App.SkinLoad += App_SkinLoad;
        App.PicUpdate += Update;

        Update();
    }

    private void Button2_Click(object? sender, RoutedEventArgs e)
    {
        var window = App.FindRoot(VisualRoot);
        if (isplay)
        {
            BaseBinding.MusicPause();

            window.SetTitle(App.GetLanguage("MainWindow.Title"));
        }
        else
        {
            BaseBinding.MusicPlay();

            window.SetTitle(App.GetLanguage("MainWindow.Title") + " " + App.GetLanguage("MainWindow.Info33"));
        }

        isplay = !isplay;
    }

    private void Update()
    {
        var config = ConfigBinding.GetAllConfig();
        if (config.Item2 != null)
        {
            if (config.Item2.CornerRadius == true)
            {
                Border1.CornerRadius = new CornerRadius(0, 0, config.Item2.Radius, 0);
            }
            else
            {
                Border1.CornerRadius = new CornerRadius(0);
            }
        }
    }

    private void Button1_Click(object? sender, RoutedEventArgs e)
    {
        Window.Button1.IsVisible = true;
        App.CrossFade100.Start(Grid1, null, CancellationToken.None);
        Button1.IsVisible = false;
    }

    public void Display()
    {
        Window.Button1.IsVisible = false;
        App.CrossFade100.Start(null, Grid1, CancellationToken.None);
        Button1.IsVisible = true;
    }

    private void App_SkinLoad()
    {
        Image1.Source = UserBinding.HeadBitmap!;

        ProgressBar1.IsVisible = false;
    }

    private void Image1_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            App.ShowUser();
        }
    }

    private void Button_Setting_Click(object? sender, RoutedEventArgs e)
    {
        App.ShowSetting(SettingType.Normal);
    }

    private void Button_Add1_Click(object? sender, RoutedEventArgs e)
    {
        if (BaseBinding.IsDownload)
        {
            var window = App.FindRoot(VisualRoot);
            window.Info.Show(App.GetLanguage("MainWindow.Control.Info3"));
            return;
        }
        App.ShowAddGame();
    }

    private void Button_Switch_Click(object? sender, RoutedEventArgs e)
    {
        App.ShowSkin();
    }

    private void Button_Edit_Click(object? sender, RoutedEventArgs e)
    {
        if (Obj != null)
        {
            App.ShowGameEdit(Obj);
        }
    }

    private void Button_Launch_Click(object? sender, RoutedEventArgs e)
    {
        Window.Launch(false);
    }

    public void SetGame(GameSettingObj? obj)
    {
        Obj = obj;
        if (obj == null)
        {
            Button_Launch.IsEnabled = false;
            Button_Edit.IsEnabled = false;
        }
        else
        {
            if (BaseBinding.IsGameRun(obj) || Launch)
            {
                Button_Launch.IsEnabled = false;
            }
            else
            {
                Button_Launch.IsEnabled = true;
            }
            Button_Edit.IsEnabled = true;
        }
    }

    public void SetLaunch(bool launch)
    {
        Launch = launch;
    }

    public void SetWindow(MainControl window)
    {
        Window = window;
    }

    public async void SetUser(LoginObj? obj)
    {
        Obj1 = obj;

        if (Obj1 == null)
        {
            TextBlock_Type.Text = App.GetLanguage("MainWindow.Control.Info1");
            TextBlock_Name.Text = App.GetLanguage("MainWindow.Control.Info2");
        }
        else
        {
            TextBlock_Name.Text = Obj1.UserName;
            TextBlock_Type.Text = Obj1.AuthType.GetName();
        }

        ProgressBar1.IsVisible = true;

        await UserBinding.LoadSkin();
    }

    public void Load()
    {
        var config = ConfigBinding.GetAllConfig();
        if (config.Item2?.ServerCustom?.LockGame == true)
        {
            Button_Add1.IsVisible = false;
        }
        else
        {
            Button_Add1.IsVisible = true;
        }

        if (config.Item2?.ServerCustom?.PlayMusic == true)
        {
            var window = App.FindRoot(VisualRoot);
            window.SetTitle(App.GetLanguage("MainWindow.Title") + " " + App.GetLanguage("MainWindow.Info33")) ;
            Button2.IsVisible = true;
        }
        else
        {
            Button2.IsVisible = false;
        }
    }
}
