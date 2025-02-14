using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading;
using System.Threading.Tasks;

namespace ColorMC.Gui.UI.Controls;

public partial class Info6Control : UserControl
{
    private readonly Semaphore semaphore = new(0, 2);
    private bool Display = false;

    public bool Cancel { get; private set; }

    public Info6Control()
    {
        InitializeComponent();

        Button_Confirm.Click += Button_Add_Click;
        Button_Cancel.Click += Button_Cancel_Click;
    }

    private void Button_Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Cancel = true;
        semaphore.Release();
        Close();
    }

    private void Button_Add_Click(object? sender, RoutedEventArgs e)
    {
        Cancel = false;
        semaphore.Release();
        Close();
    }

    public void Close()
    {
        if (!Display)
            return;

        App.CrossFade300.Start(this, null, CancellationToken.None);
    }

    public async Task<bool> ShowWait(string title, string data)
    {
        Display = true;

        Label1.Content = title;
        TextBox1.Text = data;

        App.CrossFade300.Start(null, this, CancellationToken.None);

        await Task.Run(() =>
        {
            semaphore.WaitOne();
        });

        return Cancel;
    }
}
