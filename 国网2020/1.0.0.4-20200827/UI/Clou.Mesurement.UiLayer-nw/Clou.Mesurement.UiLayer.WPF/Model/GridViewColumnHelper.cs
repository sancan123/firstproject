﻿using System.Reflection;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF.Model
{
    internal static class GridViewColumnHelper
    {
        private static PropertyInfo DesiredWidthProperty =
            typeof(GridViewColumn).GetProperty("DesiredWidth", BindingFlags.NonPublic | BindingFlags.Instance);

        public static double GetColumnWidth(this GridViewColumn column)
        {
            return (double.IsNaN(column.Width)) ? (double)DesiredWidthProperty.GetValue(column, null) : column.Width;
        }
    }
}
