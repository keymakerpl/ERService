using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace ERService.Infrastructure.Helpers
{
    //TODO: Trzeba by to do jakiego generyka przenieść, bardziej abstract
    public class DropDownButtonBehavior : Behavior<Grid>
    {
        private long _attachedCount;
        private bool _isContextMenuOpened;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(Grid.MouseLeftButtonUpEvent, new RoutedEventHandler(AssociatedObject_Click), true);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(Grid.MouseLeftButtonUpEvent, new RoutedEventHandler(AssociatedObject_Click));
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null && element.ContextMenu != null)
            {
                if (!_isContextMenuOpened)
                {
                    element.ContextMenu.AddHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed), true);
                    Interlocked.Increment(ref _attachedCount);
                    element.ContextMenu.PlacementTarget = element;
                    element.ContextMenu.Placement = PlacementMode.Bottom;
                    element.ContextMenu.IsOpen = true;
                    _isContextMenuOpened = true;
                }
            }
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            _isContextMenuOpened = false;
            var contextMenu = sender as ContextMenu;
            if(contextMenu != null)
            {
                contextMenu.RemoveHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed));
                Interlocked.Decrement(ref _attachedCount);
            }
        }
    }
}
