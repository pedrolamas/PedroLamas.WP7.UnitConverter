namespace PedroLamas.WP7.UnitConverter.Framework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Shapes;

#if !SILVERLIGHT
    using System.Windows.Documents;
#endif

    /// <summary>
    /// Used to configure the conventions used by the framework to apply bindings and create actions.
    /// </summary>
    public static class ConventionManager
    {
        static readonly ILog Log = LogManager.GetLog(typeof(ConventionManager));

        /// <summary>
        /// Converters <see cref="bool"/> to/from <see cref="Visibility"/>.
        /// </summary>
        public static IValueConverter BooleanToVisibilityConverter = new BooleanToVisibilityConverter();

        /// <summary>
        /// The default DataTemplate used for ItemsControls when required.
        /// </summary>
        public static DataTemplate DefaultItemTemplate = (DataTemplate)
#if SILVERLIGHT
        XamlReader.Load(
#else
XamlReader.Parse(
#endif
"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:cal='clr-namespace:PedroLamas.WP7.UnitConverter.Framework;assembly=PedroLamas.WP7.UnitConverter'> " +
                "<ContentControl cal:View.Model=\"{Binding}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" />" +
            "</DataTemplate>"
            );

        /// <summary>
        /// The default DataTemplate used for Headered controls when required.
        /// </summary>
        public static DataTemplate DefaultHeaderTemplate = (DataTemplate)
#if SILVERLIGHT
        XamlReader.Load(
#else
XamlReader.Parse(
#endif
"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><TextBlock Text=\"{Binding DisplayName, Mode=TwoWay}\" /></DataTemplate>"
        );

        static readonly Dictionary<Type, ElementConvention> ElementConventions = new Dictionary<Type, ElementConvention>();

        /// <summary>
        /// Changes the provided word from a plural form to a singular form.
        /// </summary>
        public static Func<string, string> Singularize = original =>
        {
            return original.TrimEnd('s');
        };

        /// <summary>
        /// Derives the SelectedItem property name.
        /// </summary>
        public static Func<string, IEnumerable<string>> DerivePotentialSelectionNames = name =>
        {
            var singular = Singularize(name);
            return new[] {
                "Active" + singular,
                "Selected" + singular,
                "Current" + singular
            };
        };

        /// <summary>
        /// Applies the appropriate binding mode to the binding.
        /// </summary>
        public static Action<Binding, PropertyInfo> ApplyBindingMode = (binding, property) =>
        {
            var setMethod = property.GetSetMethod();
            binding.Mode = (property.CanWrite && setMethod != null && setMethod.IsPublic) ? BindingMode.TwoWay : BindingMode.OneWay;
        };

        /// <summary>
        /// Determines whether or not and what type of validation to enable on the binding.
        /// </summary>
        public static Action<Binding, Type, PropertyInfo> ApplyValidation = (binding, viewModelType, property) =>
        {
#if SILVERLIGHT && !WP7
            if(typeof(INotifyDataErrorInfo).IsAssignableFrom(viewModelType))
                binding.ValidatesOnNotifyDataErrors = true;
#endif
#if !WP7
            if (typeof(IDataErrorInfo).IsAssignableFrom(viewModelType))
                binding.ValidatesOnDataErrors = true;
#endif
        };

        /// <summary>
        /// Determines whether a value converter is is needed and applies one to the binding.
        /// </summary>
        public static Action<Binding, DependencyProperty, PropertyInfo> ApplyValueConverter = (binding, bindableProperty, property) =>
        {
            if (bindableProperty == UIElement.VisibilityProperty && typeof(bool).IsAssignableFrom(property.PropertyType))
                binding.Converter = BooleanToVisibilityConverter;
        };

        /// <summary>
        /// Determines whether a custom string format is needed and applies it to the binding.
        /// </summary>
        public static Action<Binding, ElementConvention, PropertyInfo> ApplyStringFormat = (binding, convention, property) =>
        {
#if !WP7
            if (typeof(DateTime).IsAssignableFrom(property.PropertyType))
                binding.StringFormat = "{0:MM/dd/yyyy}";
#endif
        };

        /// <summary>
        /// Determines whether a custom update source trigger should be applied to the binding.
        /// </summary>
        public static Action<DependencyProperty, DependencyObject, Binding> ApplyUpdateSourceTrigger = (bindableProperty, element, binding) =>
        {
#if SILVERLIGHT
            ApplySilverlightTriggers(element, bindableProperty, x => x.GetBindingExpression(bindableProperty));
#else
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
#endif
        };

        /// <summary>
        /// Determines whether a particular dependency property already has a binding on the provided element.
        /// </summary>
        public static Func<FrameworkElement, DependencyProperty, bool> HasBinding = (element, property) =>
        {
            var exists = element.GetBindingExpression(property) != null;

            if (exists)
                Log.Info("Binding exists on {0}.", element.Name);

            return exists;
        };

        /// <summary>
        /// Creates a binding and sets it on the element.
        /// </summary>
        public static Func<Type, string, PropertyInfo, FrameworkElement, ElementConvention, bool> SetBinding =
            (viewModelType, path, property, element, convention) =>
            {
                var bindableProperty = convention.GetBindableProperty(element);
                if (HasBinding(element, bindableProperty))
                    return false;

                var binding = new Binding(path);

                ApplyBindingMode(binding, property);
                ApplyValueConverter(binding, bindableProperty, property);
                ApplyStringFormat(binding, convention, property);
                ApplyValidation(binding, viewModelType, property);
                ApplyUpdateSourceTrigger(bindableProperty, element, binding);

                BindingOperations.SetBinding(element, bindableProperty, binding);

                return true;
            };

        static ConventionManager()
        {
#if SILVERLIGHT
            AddElementConvention<HyperlinkButton>(HyperlinkButton.ContentProperty, "DataContext", "Click");
            AddElementConvention<PasswordBox>(PasswordBox.PasswordProperty, "Password", "PasswordChanged");
#else
            AddElementConvention<PasswordBox>(PasswordBox.DataContextProperty, "DataContext", "PasswordChanged");
            AddElementConvention<Hyperlink>(Hyperlink.DataContextProperty, "DataContext", "Click");
            AddElementConvention<RichTextBox>(RichTextBox.DataContextProperty, "DataContext", "TextChanged");
            AddElementConvention<Menu>(Menu.ItemsSourceProperty, "DataContext", "Click");
            AddElementConvention<MenuItem>(MenuItem.ItemsSourceProperty, "DataContext", "Click");
            AddElementConvention<Label>(Label.ContentProperty, "Content", "DataContextChanged");
            AddElementConvention<Slider>(Slider.ValueProperty, "Value", "ValueChanged");
            AddElementConvention<Expander>(Expander.IsExpandedProperty, "IsExpanded", "Expanded");
            AddElementConvention<StatusBar>(StatusBar.ItemsSourceProperty, "DataContext", "Loaded");
            AddElementConvention<ToolBar>(ToolBar.ItemsSourceProperty, "DataContext", "Loaded");
            AddElementConvention<ToolBarTray>(ToolBarTray.VisibilityProperty, "DataContext", "Loaded");
            AddElementConvention<TreeView>(TreeView.ItemsSourceProperty, "SelectedItem", "SelectedItemChanged");
            AddElementConvention<TabControl>(TabControl.ItemsSourceProperty, "ItemsSource", "SelectionChanged")
                .ApplyBinding = (viewModelType, path, property, element, convention) =>
                {
                    if (!SetBinding(viewModelType, path, property, element, convention))
                        return;

                    var tabControl = (TabControl)element;
                    if (tabControl.ContentTemplate == null && tabControl.ContentTemplateSelector == null && property.PropertyType.IsGenericType)
                    {
                        var itemType = property.PropertyType.GetGenericArguments().First();
                        if (!itemType.IsValueType && !typeof(string).IsAssignableFrom(itemType))
                            tabControl.ContentTemplate = DefaultItemTemplate;
                    }

                    ConfigureSelectedItem(element, Selector.SelectedItemProperty, viewModelType, path);

                    if (string.IsNullOrEmpty(tabControl.DisplayMemberPath))
                        ApplyHeaderTemplate(tabControl, TabControl.ItemTemplateProperty, viewModelType);
                };
            AddElementConvention<TabItem>(TabItem.ContentProperty, "DataContext", "DataContextChanged");
            AddElementConvention<Window>(Window.DataContextProperty, "DataContext", "Loaded");
#endif
            AddElementConvention<UserControl>(UserControl.VisibilityProperty, "DataContext", "Loaded");
            AddElementConvention<Image>(Image.SourceProperty, "Source", "Loaded");
            AddElementConvention<ToggleButton>(ToggleButton.IsCheckedProperty, "IsChecked", "Click");
            AddElementConvention<ButtonBase>(ButtonBase.ContentProperty, "DataContext", "Click");
            AddElementConvention<TextBox>(TextBox.TextProperty, "Text", "TextChanged");
            AddElementConvention<TextBlock>(TextBlock.TextProperty, "Text", "DataContextChanged");
            AddElementConvention<Selector>(Selector.ItemsSourceProperty, "SelectedItem", "SelectionChanged")
                .ApplyBinding = (viewModelType, path, property, element, convention) =>
                {
                    if (!SetBinding(viewModelType, path, property, element, convention))
                        return;

                    ConfigureSelectedItem(element, Selector.SelectedItemProperty, viewModelType, path);
                    ConfigureItemsControl((ItemsControl)element, property);
                };
            AddElementConvention<ItemsControl>(ItemsControl.ItemsSourceProperty, "DataContext", "Loaded")
                .ApplyBinding = (viewModelType, path, property, element, convention) =>
                {
                    if (!SetBinding(viewModelType, path, property, element, convention))
                        return;

                    ConfigureItemsControl((ItemsControl)element, property);
                };
            AddElementConvention<ContentControl>(ContentControl.ContentProperty, "DataContext", "Loaded").GetBindableProperty =
                delegate(DependencyObject foundControl)
                {
                    var element = (ContentControl)foundControl;
#if SILVERLIGHT
                    return element.ContentTemplate == null && !(element.Content is DependencyObject)
                        ? View.ModelProperty
                        : ContentControl.ContentProperty;
#else
                    return element.ContentTemplate == null && element.ContentTemplateSelector == null && !(element.Content is DependencyObject)
                        ? View.ModelProperty
                        : ContentControl.ContentProperty;
#endif
                };
            AddElementConvention<Shape>(Shape.VisibilityProperty, "DataContext", "MouseLeftButtonUp");
            AddElementConvention<FrameworkElement>(FrameworkElement.VisibilityProperty, "DataContext", "Loaded");
        }

        /// <summary>
        /// Adds an element convention.
        /// </summary>
        /// <typeparam name="T">The type of element.</typeparam>
        /// <param name="bindableProperty">The default property for binding conventions.</param>
        /// <param name="parameterProperty">The default property for action parameters.</param>
        /// <param name="eventName">The default event to trigger actions.</param>
        public static ElementConvention AddElementConvention<T>(DependencyProperty bindableProperty, string parameterProperty, string eventName)
        {
            return AddElementConvention(new ElementConvention
            {
                ElementType = typeof(T),
                GetBindableProperty = element => bindableProperty,
                ParameterProperty = parameterProperty,
                CreateTrigger = () => new System.Windows.Interactivity.EventTrigger { EventName = eventName }
            });
        }

        /// <summary>
        /// Adds an element convention.
        /// </summary>
        /// <param name="convention"></param>
        public static ElementConvention AddElementConvention(ElementConvention convention)
        {
            return ElementConventions[convention.ElementType] = convention;
        }

        /// <summary>
        /// Gets an element convention for the provided element type.
        /// </summary>
        /// <param name="elementType">The type of element to locate the convention for.</param>
        /// <returns>The convention if found, null otherwise.</returns>
        /// <remarks>Searches the class hierarchy for conventions.</remarks>
        public static ElementConvention GetElementConvention(Type elementType)
        {
            if (elementType == null)
                return null;

            ElementConvention propertyConvention;
            ElementConventions.TryGetValue(elementType, out propertyConvention);
            return propertyConvention ?? GetElementConvention(elementType.BaseType);
        }

        static void ConfigureItemsControl(ItemsControl itemsControl, PropertyInfo property)
        {
            if (string.IsNullOrEmpty(itemsControl.DisplayMemberPath)
                && !HasBinding(itemsControl, ItemsControl.DisplayMemberPathProperty)
                    && itemsControl.ItemTemplate == null
                        && property.PropertyType.IsGenericType)
            {
#if !WP7
                var itemType = property.PropertyType.GetGenericArguments().First();
                if (!itemType.IsValueType && !typeof(string).IsAssignableFrom(itemType))
#endif
#if !SILVERLIGHT && !WP7
                    if (itemsControl.ItemTemplateSelector == null)
                        itemsControl.ItemTemplate = DefaultItemTemplate;
#else
                    itemsControl.ItemTemplate = DefaultItemTemplate;
#endif

            }
        }

        /// <summary>
        /// Configures the selected item convention.
        /// </summary>
        /// <param name="selector">The element that has a SelectedItem property.</param>
        /// <param name="selectedItemProperty">The SelectedItem property.</param>
        /// <param name="viewModelType">The view model type.</param>
        /// <param name="path">The property path.</param>
        public static void ConfigureSelectedItem(FrameworkElement selector, DependencyProperty selectedItemProperty, Type viewModelType, string path)
        {
            if (HasBinding(selector, selectedItemProperty))
                return;

            var index = path.LastIndexOf('.');
            index = index == -1 ? 0 : index + 1;
            var baseName = path.Substring(index);

            foreach (var potentialName in DerivePotentialSelectionNames(baseName))
            {
                if (viewModelType.GetPropertyCaseInsensitive(potentialName) != null)
                {
                    var selectionPath = path.Replace(baseName, potentialName);
                    BindingOperations.SetBinding(selector, selectedItemProperty, new Binding(selectionPath) { Mode = BindingMode.TwoWay });
                    return;
                }
            }
        }

        /// <summary>
        /// Applies a header template based on <see cref="IHaveDisplayName"/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="headerTemplateProperty"></param>
        /// <param name="viewModelType"></param>
        public static void ApplyHeaderTemplate(FrameworkElement element, DependencyProperty headerTemplateProperty, Type viewModelType)
        {
            var template = element.GetValue(headerTemplateProperty);

            if (template != null
                || !typeof(IHaveDisplayName).IsAssignableFrom(viewModelType))
                return;

            element.SetValue(headerTemplateProperty, DefaultHeaderTemplate);
        }

#if SILVERLIGHT
        /// <summary>
        /// Accounts for the lack of UpdateSourceTrigger in silverlight.
        /// </summary>
        /// <param name="element">The element to wire for change events on.</param>
        /// <param name="dependencyProperty">The rproperty that is being bound.</param>
        /// <param name="expressionSource">The source of the binding expression tht needs to be updated.</param>
        public static void ApplySilverlightTriggers(DependencyObject element, DependencyProperty dependencyProperty, Func<FrameworkElement, BindingExpression> expressionSource)
        {
            var textBox = element as TextBox;
            if (textBox != null && dependencyProperty == TextBox.TextProperty)
            {
                textBox.TextChanged += delegate { expressionSource(textBox).UpdateSource(); };
                return;
            }

            var passwordBox = element as PasswordBox;
            if (passwordBox != null && dependencyProperty == PasswordBox.PasswordProperty)
            {
                passwordBox.PasswordChanged += delegate { expressionSource(passwordBox).UpdateSource(); };
                return;
            }
        }
#endif
    }
}
