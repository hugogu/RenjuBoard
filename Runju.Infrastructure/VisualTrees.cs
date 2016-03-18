using System.Windows;
using System.Windows.Media;

namespace Runju.Infrastructure
{
    public static class VisualTrees
    {
        public static T FindAncestor<T>(this DependencyObject element)
            where T : DependencyObject
        {
            if (element == null)
                return null;

            if (element is T)
                return element as T;

            var parent = VisualTreeHelper.GetParent(element);
            if (parent == null)
            {
                var framework = element as FrameworkElement;
                if (framework != null)
                {
                    parent = framework.Parent ?? framework.TemplatedParent;
                }
            }

            return parent.FindAncestor<T>();
        }
    }
}
