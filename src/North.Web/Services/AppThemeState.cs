using Blazored.LocalStorage;
using FastEnumUtility;
using North.RCL.ToolBars;
using North.Web.Common;

namespace North.Web.Services;

public sealed class AppThemeState(ILocalStorageService localStorageService)
{
    private readonly ILocalStorageService _localStorageService = localStorageService;
    private bool _isInitialized;
    public event Action? Changed;

    /// <summary>
    /// 系统是否为深色模式
    /// </summary>
    public bool IsSystemDarkMode
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }
            field = value;

            if (ThemeMode is ThemeMode.System)
            {
                NotifyChanged();
            }
        }
    }

    /// <summary>
    /// 主题模式
    /// </summary>
    public ThemeMode ThemeMode { get; private set; } = ThemeMode.System;

    /// <summary>
    /// 是否为深色模式
    /// </summary>
    public bool IsDarkMode =>
        ThemeMode is ThemeMode.Dark || (ThemeMode is ThemeMode.System && IsSystemDarkMode);

    public async Task InitializeAsync(bool persistThemeModeIfMissing = false)
    {
        if (_isInitialized)
        {
            return;
        }

        // 加载本地主题模式设置
        var themeModeString = await _localStorageService.GetItemAsStringAsync(
            GlobalValues.LOCAL_STORAGE_KEY_THEME_MODE
        );

        if (string.IsNullOrWhiteSpace(themeModeString))
        {
            if (persistThemeModeIfMissing)
            {
                await PersistThemeModeIfMissingAsync();
            }
        }
        else
        {
            ThemeMode = FastEnum.Parse<ThemeMode>(themeModeString);
        }

        _isInitialized = true;
    }

    public async Task SwitchThemeAsync()
    {
        // 系统 -> 深色 -> 浅色 -> 系统
        ThemeMode = ThemeMode switch
        {
            ThemeMode.System => ThemeMode.Dark,
            ThemeMode.Dark => ThemeMode.Light,
            ThemeMode.Light => ThemeMode.System,
            _ => ThemeMode.System,
        };

        await PersistThemeModeIfMissingAsync();

        NotifyChanged();
    }

    private async Task PersistThemeModeIfMissingAsync()
    {
        await _localStorageService.SetItemAsStringAsync(
            GlobalValues.LOCAL_STORAGE_KEY_THEME_MODE,
            ThemeMode.FastToString()
        );
    }

    private void NotifyChanged()
    {
        Changed?.Invoke();
    }
}
