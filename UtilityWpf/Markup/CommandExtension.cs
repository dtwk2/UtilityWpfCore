﻿using ReactiveUI;
using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Utility.Common;

namespace UtilityWpf.Markup
{
    public enum ConversionType
    {
        None,
        Default,
        SingleAdds
    }

    /// <summary>
    /// <a href="https://github.com/mobilemotion/event-binding/blob/master/MvvmEventBinding/WPF/MvvmEventBinding/EventBindingExtension.cs">Custom markup extension</a>
    /// that allows direct binding of commands to events.
    /// <example>
    /// <code>
    /// &lt;ListBox SelectionChanged=&quot;{utl:Command MyCommand}&quot;/&gt;
    /// </code>
    /// </example>
    /// </summary>
    [MarkupExtensionReturnType(typeof(Delegate))]
    public class CommandExtension : MarkupExtension
    {
        private IDisposable? disposable;
        private readonly string commandName;

        /// <summary>
        /// </summary>
        /// <param name="commandName">
        /// Name of the Command to be invoked when the event fires
        /// </param>
        public CommandExtension(string commandName)
        {
            this.commandName = commandName;
        }

        public IValueConverter? Converter { get; init; }

        public object? ConverterParameter { get; init; }

        public Type? ConverterType { get; init; }

        public ConversionType ConversionType { get; init; }

        /// <summary>
        /// Retrieves the context in which the markup extension is used, and (if used in the
        /// context of an event or a method) returns an event handler that executes the
        /// desired Command.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Retrieve a reference to the InvokeCommand helper method declared below, using reflection
            if (GetType().GetMethod(nameof(InvokeCommand), BindingFlags.Instance | BindingFlags.NonPublic) is MethodInfo methodInfo &&
                serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget { TargetProperty: { } info })
            {
                // Check if the current context is an event or a method call with two parameters
                var type = info switch
                {   // If the context is an event, simply return the helper method as delegate
                    // (this delegate will be invoked when the event fires)
                    EventInfo { EventHandlerType: { } e } => e,
                    MethodInfo property => GetMethodInfoType(property),
                    _ => throw new Exception("sfddfsdf333")
                };
                return methodInfo.CreateDelegate(type, this);

                Type GetMethodInfoType(MethodInfo property)
                {
                    // Some events are represented as method calls with 2 parameters:
                    // The first parameter is the control that acts as the event's sender,
                    // the second parameter is the actual event handler
                    var methodParameters = property.GetParameters();
                    if (methodParameters.Length == 2)
                    {
                        return methodParameters[1].ParameterType;
                    }
                    throw new Exception("f&&&sddffds");
                }
            }
            throw new InvalidOperationException("The EventBinding markup extension is valid only in the context of events.");
        }

        /// <summary>
        /// Helper method that retrieves a control's ViewModel, searches the ViewModel for a
        /// Command with given name, and invokes this Command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void InvokeCommand(object sender, EventArgs args)
        {
            if (string.IsNullOrEmpty(commandName) == false &&
                sender is FrameworkElement frameworkElement)
            {
                disposable?.Dispose();
                disposable =
                    frameworkElement
                    .WhenAnyValue(a => a.DataContext)
                    .WhereNotNull()
                    .Select(context => (context, context.GetType().GetProperty(commandName)?.GetValue(context)))
                    .Subscribe(a =>
                    {
                        if (a.Item2 is ICommand command)
                        {
                            if (command.CanExecute(args))
                            {
                                var conversion = ConversionType switch
                                {
                                    ConversionType.None => NoConversion(args, command),
                                    ConversionType.Default => Conversion(args, command),
                                    ConversionType.SingleAdds => SingleSelectorConversion(args, frameworkElement),
                                    _ => throw new NotImplementedException(),
                                };
                                command.Execute(conversion);
                            }
                            return;
                        }

                        throw new Exception($"{commandName} does not correspond to a {nameof(ICommand)} on {a.Item1.GetType().Name}");
                    });
            }
            else
                throw new Exception("55jjsdsd");
        }

        private object? SingleSelectorConversion(EventArgs args, FrameworkElement element)
        {
            if (args is SelectionChangedEventArgs selectionArgs)
                if (element.GetValue(ListBox.SelectionModeProperty) is SelectionMode.Single)
                {
                    return selectionArgs.AddedItems.Cast<object>().FirstOrDefault();
                }
            return null;
        }

        protected virtual object? NoConversion(EventArgs args, ICommand cmd)
        {
            switch (cmd)
            {
                case ICommand when cmd.GetType().BaseType?.Name == typeof(ReactiveCommandBase<,>).Name:
                    {
                        var type = cmd.GetType().GenericTypeArguments.First();
                        return Activator.CreateInstance(type);
                    }
                default:
                    throw new Exception("s33333dfsdsd");
            }
        }

        protected virtual object? Conversion(EventArgs args, ICommand cmd)
        {
            switch (cmd)
            {
                case ICommand when cmd.GetType().BaseType?.Name == typeof(ReactiveCommandBase<,>).Name:
                    if (Converter != null)
                    {
                        return Converter?.Convert(args, null, ConverterParameter, CultureInfo.CurrentCulture);
                    }
                    else if (cmd.GetType().GetGenericArguments().First() is Type type)
                    {
                        if (type != typeof(object) && AutoMapperSingleton.Instance.ConfigurationProvider.ResolveTypeMap(args.GetType(), type) is { } map)
                        {
                            return AutoMapperSingleton.Instance.Map(args, args.GetType(), type);
                        }
                        else
                        {
                            return args;
                        }

                        //throw new Exception($"The generic-argument, object, of the type of ReactiveCommand used, is too broad to map.");
                    }
                    else
                    {
                        throw new Exception("d33fgssdfgfeee");
                    }
                case ICommand:
                    if (Converter != null)
                    {
                        return Converter?.Convert(args, null, ConverterParameter, CultureInfo.CurrentCulture);
                    }
                    else if (ConverterType != null)
                    {
                        return AutoMapperSingleton.Instance.Map(args, args.GetType(), ConverterType);
                    }
                    else
                        return args;

                default:
                    throw new Exception("s33333dfsdsd");
            }
        }
    }
}