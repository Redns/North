using FastEnumUtility;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using North.Web.Services;

namespace North.Web;

public partial class AppThemeProvider : ComponentBase, IDisposable
{
    private MudThemeProvider? _mudThemeProvider;
    private bool _isWatchingSystemTheme;
    private bool _hasRendered;

    [Inject]
    public required AppThemeState ThemeState { get; set; }

    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public bool PersistThemeModeIfMissing { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ThemeState.InitializeAsync(PersistThemeModeIfMissing);
        ThemeState.Changed += HandleThemeChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        _hasRendered = true;
        await SyncDocumentThemeAsync();

        if (!firstRender || _mudThemeProvider is null || _isWatchingSystemTheme)
        {
            return;
        }

        ThemeState.IsSystemDarkMode = await _mudThemeProvider.GetSystemDarkModeAsync();
        await _mudThemeProvider.WatchSystemDarkModeAsync(async isDarkMode =>
        {
            ThemeState.IsSystemDarkMode = isDarkMode;
        });

        _isWatchingSystemTheme = true;
    }

    private void HandleThemeChanged()
    {
        _ = InvokeAsync(async () =>
        {
            if (_hasRendered)
            {
                await SyncDocumentThemeAsync();
            }

            StateHasChanged();
        });
    }

    private async Task SyncDocumentThemeAsync()
    {
        await JSRuntime.InvokeVoidAsync(
            "northTheme.apply",
            ThemeState.ThemeMode.FastToString(),
            ThemeState.IsDarkMode
        );
    }

    public void Dispose()
    {
        ThemeState.Changed -= HandleThemeChanged;
        GC.SuppressFinalize(this);
    }
}
