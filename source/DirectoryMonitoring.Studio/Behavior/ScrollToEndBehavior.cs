using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace DirectoryMonitoring.Studio.Behavior
{
    internal class ScrollToEndBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty AutoScrollProperty = DependencyProperty.RegisterAttached(
            "AutoScroll",
            typeof(bool),
            typeof(ScrollToEndBehavior),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)

        );

        public static void SetAutoScroll(UIElement element, bool value)
        {
            element.SetValue(AutoScrollProperty, value);
        }

        public static bool GetAutoScroll(UIElement element)
        {
            return (bool)element.GetValue(AutoScrollProperty);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.LoadingRow += HandlerAddingANewItem;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.LoadingRow -= HandlerAddingANewItem;
        }

        private void HandlerAddingANewItem(object sender, DataGridRowEventArgs e)
        {
            if(sender is DataGrid control && control.Items.Count > 0)
            {
                if(GetAutoScroll(control) == false || control.Items.Count == 0)
                {
                    return;
                }

                var lastIndex = control.Items.Count - 1;

                control.ScrollIntoView(control.Items[lastIndex], null);
            }
        }
    }
}
