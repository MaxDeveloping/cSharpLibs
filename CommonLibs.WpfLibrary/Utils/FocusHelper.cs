using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CommonLibs.WpfLibrary.Utils
{
    public static class FocusHelper
    {
        public static bool FocusElementOrFirstFocusableChild(UIElement pElement)
        {
            if (pElement.Focusable)
                return pElement.Focus();

            var childrenCount = VisualTreeHelper.GetChildrenCount(pElement);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(pElement, i);
                if (child is UIElement uiElement)
                {
                    if (uiElement.Focusable)
                    {
                        return uiElement.Focus();
                    }
                    else
                    {
                        var success = FocusElementOrFirstFocusableChild(uiElement);
                        if (success)
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
