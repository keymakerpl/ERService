using System.Windows;

namespace Smith.WPF.HtmlEditor.Helpers
{
    public static class IndexToInsert
    {
        public static string GetInsertPattern(DependencyObject obj)
        {
            return (string)obj.GetValue(InsertPatternProperty);
        }

        public static void SetInsertPattern(DependencyObject obj, string value)
        {
            obj.SetValue(InsertPatternProperty, value);
        }

        public static readonly DependencyProperty InsertPatternProperty =
            DependencyProperty.RegisterAttached("InsertPattern", typeof(string), typeof(IndexToInsert),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits, OnIndexPatternChanged));

        private static void OnIndexPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as Smith.WPF.HtmlEditor.HtmlEditor;
            if (editor != null && e.NewValue != null)
            {
                editor.Focus();
                editor.InsertText((string)e.NewValue);
            }
        }
    }
}
