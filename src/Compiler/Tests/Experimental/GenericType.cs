using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Experimental
{
    public class GenericType<T>
    {
        public T MyProperty { get; set; }

        public int MyNonGenericProperty { get; set; }

        public static string MyField = "MyField";

        public static readonly DependencyProperty HasSomethingProperty =
            DependencyProperty.RegisterAttached(
                "HasSomething",
                typeof(bool), typeof(GenericType<T>), null);

        public static bool GetHasSomething(UIElement target) =>
            (bool)target.GetValue(HasSomethingProperty);

        public static void SetHasSomething(UIElement target, bool value) =>
            target.SetValue(HasSomethingProperty, value);

        public T MethodWithGenericReturnType()
        {
            return default;
        }
    }
}
