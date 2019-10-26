using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace PedroLamas.WP7.UnitConverter.Controls
{
    public class PickerBox : ItemsControl
    {
        public event SelectionChangedEventHandler SelectionChanged;

        #region

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(PickerBox), new PropertyMetadata(0));

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(PickerBox), new PropertyMetadata(0));

        #endregion

        //public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty;
        //public static readonly DependencyProperty SelectedIndexProperty;
        //public static readonly DependencyProperty SelectedItemProperty;

        public PickerBox()
        {
        }
    }
}