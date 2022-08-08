namespace North.Shared
{
    partial class MainLayout
    {
        public bool IsExpanded { get; set; } = true;
        public void OnNavMenuStateChanged()
        {
            IsExpanded = !IsExpanded;
        }
    }
}
