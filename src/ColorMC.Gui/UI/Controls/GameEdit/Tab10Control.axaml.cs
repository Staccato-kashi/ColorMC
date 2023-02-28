using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ColorMC.Core.Objs;
using ColorMC.Core.Objs.Minecraft;
using ColorMC.Gui.UI.Windows;
using ColorMC.Gui.UIBinding;
using DynamicData;
using System;
using System.Collections.ObjectModel;

namespace ColorMC.Gui.UI.Controls.GameEdit;

public partial class Tab10Control : UserControl
{
    private readonly ObservableCollection<ServerInfoObj> List = new();
    private GameSettingObj Obj;

    public Tab10Control()
    {
        InitializeComponent();

        Button_A1.PointerExited += Button_A1_PointerLeave;
        Button_A.PointerEntered += Button_A_PointerEnter;

        Button_R1.PointerExited += Button_R1_PointerLeave;
        Button_R.PointerEntered += Button_R_PointerEnter;

        Button_R1.Click += Button_R1_Click;
        Button_A1.Click += Button_A1_Click;

        DataGrid1.Items = List;

        DataGrid1.SelectionChanged += DataGrid1_SelectionChanged;
        DataGrid1.CellPointerPressed += DataGrid1_CellPointerPressed;
        LayoutUpdated += Tab10Control_LayoutUpdated1;
    }

    private void DataGrid1_CellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        if (e.PointerPressedEventArgs.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (DataGrid1.SelectedItem is not ServerInfoObj obj)
                    return;

                new GameEditFlyout5(this, obj).ShowAt(this, true);
            });
        }
    }

    private void DataGrid1_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataGrid1.SelectedItem is not ServerInfoObj obj)
            return;

        ServerMotd.Load(obj.IP);
    }

    private void Button_R1_Click(object? sender, RoutedEventArgs e)
    {
        Load();
    }

    private async void Button_A1_Click(object? sender, RoutedEventArgs e)
    {
        var window = (VisualRoot as GameEditWindow)!;
        await window.Info3.ShowInput(App.GetLanguage("GameEditWindow.Tab10.Info1"),
            App.GetLanguage("GameEditWindow.Tab10.Info2"), false);
        var res = window.Info3.Read();

        if (string.IsNullOrWhiteSpace(res.Item1) || string.IsNullOrWhiteSpace(res.Item2))
        {
            window.Info.Show(App.GetLanguage("GameEditWindow.Tab10.Error1"));
            return;
        }

        GameBinding.AddServer(Obj, res.Item1, res.Item2);
        window.Info2.Show(App.GetLanguage("GameEditWindow.Tab10.Info3"));
        Load();
    }

    private void Tab10Control_LayoutUpdated1(object? sender, EventArgs e)
    {
        Expander_A.MakePadingNull();
        Expander_R.MakePadingNull();
    }

    private void Button_A1_PointerLeave(object? sender, PointerEventArgs e)
    {
        Expander_A.IsExpanded = false;
    }

    private void Button_A_PointerEnter(object? sender, PointerEventArgs e)
    {
        Expander_A.IsExpanded = true;
    }
    private void Button_R1_PointerLeave(object? sender, PointerEventArgs e)
    {
        Expander_R.IsExpanded = false;
    }

    private void Button_R_PointerEnter(object? sender, PointerEventArgs e)
    {
        Expander_R.IsExpanded = true;
    }

    private void Load()
    {
        var window = (VisualRoot as GameEditWindow)!;
        window.Info1.Show(App.GetLanguage("GameEditWindow.Tab10.Info4"));
        List.Clear();
        List.AddRange(GameBinding.GetServers(Obj));
        window.Info1.Close();
    }

    public void Update()
    {
        if (Obj == null)
            return;

        Load();
    }

    public void SetGame(GameSettingObj obj)
    {
        Obj = obj;
    }

    public void Delete(ServerInfoObj obj)
    {
        var window = (VisualRoot as GameEditWindow)!;
        GameBinding.DeleteServer(Obj, obj);
        window.Info2.Show(App.GetLanguage("GameEditWindow.Tab10.Info5"));
        Load();
    }
}
