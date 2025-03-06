using Microsoft.JSInterop;

namespace North.Razor;

public class ExampleJsInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask =
        new(
            () =>
                jsRuntime
                    .InvokeAsync<IJSObjectReference>(
                        "import",
                        "./_content/North.Razor/exampleJsInterop.js"
                    )
                    .AsTask()
        );

    public async ValueTask<string> Prompt(string message)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("showPrompt", message);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
