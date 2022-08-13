using Microsoft.AspNetCore.Components;

namespace North.Events.PasteMultimediaEvent
{
    [EventHandler("onpastemultimedia", typeof(PasteMultimediaEventsArgs),
    enableStopPropagation: true, enablePreventDefault: true)]
    public static class EventHandlers
    {
    }
}
