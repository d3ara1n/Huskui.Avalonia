using Avalonia.Interactivity;

namespace Huskui.Avalonia
{
    public class PropertyChangedRoutedEventArgs<T>(RoutedEvent evt, object source, T oldValue, T newValue)
        : RoutedEventArgs(evt, source)
    {
        public T OldValue => oldValue;
        public T NewValue => newValue;
    }
}
