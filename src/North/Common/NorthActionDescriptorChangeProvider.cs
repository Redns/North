using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace North.Common
{
    public class NorthActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public static NorthActionDescriptorChangeProvider Instance { get; } = new();

        public CancellationTokenSource TokenSource { get; private set; }

        public bool HasChanged { get; set; }

        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }

    }
}
