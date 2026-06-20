using System.Windows;
using System.Windows.Forms;

namespace MediTrack.UI.Services;

public class TrayService : IDisposable
{
    private readonly Window _mainWindow;
    private readonly NotifyIcon _notifyIcon;

    public TrayService(Window mainWindow)
    {
        _mainWindow = mainWindow;

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Open MediTrack", null, (_, _) => ShowWindow());
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("Exit", null, (_, _) => Shutdown());

        _notifyIcon = new NotifyIcon
        {
            Icon = System.Drawing.SystemIcons.Application,
            Text = "MediTrack Reminder",
            ContextMenuStrip = contextMenu,
            Visible = true
        };
        _notifyIcon.Click += (_, _) => ShowWindow();

        mainWindow.Closing += OnWindowClosing;
    }

    private void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        _mainWindow.Hide();
    }

    private void ShowWindow()
    {
        _mainWindow.Show();
        _mainWindow.WindowState = WindowState.Normal;
        _mainWindow.Activate();
    }

    private void Shutdown()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _mainWindow.Closing -= OnWindowClosing;
        _mainWindow.Close();
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _mainWindow.Closing -= OnWindowClosing;
    }
}
