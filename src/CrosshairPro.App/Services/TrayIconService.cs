using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;

namespace CrosshairPro.App.Services;

public class TrayIconService
{
    private TrayIcon? _trayIcon;
    private Window? _mainWindow;
    private Func<bool>? _getCrosshairState;
    private Action? _toggleCrosshair;
    private Action? _showMainWindow;
    private Action? _exitApp;

    public void Initialize(
        Window mainWindow,
        Func<bool> getCrosshairState,
        Action toggleCrosshair,
        Action showMainWindow,
        Action exitApp)
    {
        _mainWindow = mainWindow;
        _getCrosshairState = getCrosshairState;
        _toggleCrosshair = toggleCrosshair;
        _showMainWindow = showMainWindow;
        _exitApp = exitApp;

        var icon = new WindowIcon(AssetLoader.Open(new Uri("avares://CrosshairPro.App/Assets/app_icon.png")));

        _trayIcon = new TrayIcon
        {
            Icon = icon,
            ToolTipText = "CrosshairPro"
        };

        var menu = new NativeMenu();

        var showItem = new NativeMenuItem("显示主窗口");
        showItem.Click += (s, e) => _showMainWindow?.Invoke();
        menu.Add(showItem);

        var toggleItem = new NativeMenuItem("开关准星");
        toggleItem.Click += (s, e) => _toggleCrosshair?.Invoke();
        menu.Add(toggleItem);

        menu.Add(new NativeMenuItemSeparator());

        var exitItem = new NativeMenuItem("退出");
        exitItem.Click += (s, e) => _exitApp?.Invoke();
        menu.Add(exitItem);

        _trayIcon.Menu = menu;

        _trayIcon.Clicked += OnTrayIconClicked;

        var icons = TrayIcon.GetIcons(Application.Current!);
        icons?.Add(_trayIcon);
    }

    private void OnTrayIconClicked(object? sender, EventArgs e)
    {
        _showMainWindow?.Invoke();
    }

    public void UpdateToolTip(string text)
    {
        if (_trayIcon != null)
        {
            _trayIcon.ToolTipText = text;
        }
    }

    public void Remove()
    {
        if (_trayIcon != null && Application.Current != null)
        {
            var icons = TrayIcon.GetIcons(Application.Current);
            if (icons != null && icons.Contains(_trayIcon))
            {
                icons.Remove(_trayIcon);
            }
            _trayIcon = null;
        }
    }
}
