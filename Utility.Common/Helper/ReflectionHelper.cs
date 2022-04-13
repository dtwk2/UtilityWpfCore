﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;

namespace Utility.Common
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Determines if this property is marked as init-only.
        /// </summary>
        /// <remarks>
        /// <a href="https://alistairevans.co.uk/2020/11/01/detecting-init-only-properties-with-reflection-in-c-9/"/></a>
        /// </remarks>
        /// <param name="property">The property.</param>
        /// <returns>True if the property is init-only, false otherwise.</returns>
        public static bool IsInitOnly(this PropertyInfo property)
        {
            if (!property.CanWrite)
            {
                return false;
            }

            var setMethod = property.SetMethod;

            // Get the modifiers applied to the return parameter.
            var setMethodReturnParameterModifiers = setMethod.ReturnParameter.GetRequiredCustomModifiers();

            // Init-only properties are marked with the IsExternalInit type.
            return setMethodReturnParameterModifiers.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
        }

        public static IEnumerable<Assembly> LoadedUtilitySolutionAssemblies()
        {
            return Store.LoadedSolutionAssemblies(a => a.Name?.StartsWith(Meta.Constants.GeneralAssemblyName) ?? false);
        }

        public static string AsString(this MethodInfo mi)
        {
            StringBuilder sb = new();
            // Get method body information.
            //MethodInfo mi = typeof(Example).GetMethod("MethodBodyExample");
            MethodBody mb = mi.GetMethodBody();
            sb.AppendLine($"Method: {mi}");

            // Display the general information included in the
            // MethodBody object.
            sb.AppendLine($"Local variables are initialized: {mb.InitLocals}");
            sb.AppendLine($"Maximum number of items on the operand stack: {mb.MaxStackSize}");

            // Display information about the local variables in the
            // method body.
            sb.AppendLine();
            foreach (LocalVariableInfo lvi in mb.LocalVariables)
            {
                sb.AppendLine($"Local variable: {lvi}");
            }

            // Display exception handling clauses.
            sb.AppendLine();
            foreach (ExceptionHandlingClause ehc in mb.ExceptionHandlingClauses)
            {
                sb.AppendLine(ehc.Flags.ToString());

                // The FilterOffset property is meaningful only for Filter
                // clauses. The CatchType property is not meaningful for
                // Filter or Finally clauses.
                switch (ehc.Flags)
                {
                    case ExceptionHandlingClauseOptions.Filter:
                        sb.AppendLine($"Filter Offset: {ehc.FilterOffset}"
                            );
                        break;

                    case ExceptionHandlingClauseOptions.Finally:
                        break;

                    default:
                        sb.AppendLine($"Type of exception: {ehc.CatchType}");
                        break;
                }

                sb.AppendLine($"Handler Length: {ehc.HandlerLength}");
                sb.AppendLine($"Handler Offset: {ehc.HandlerOffset}");
                sb.AppendLine($"Try Block Length: {ehc.TryLength}");
                sb.AppendLine($"Try Block Offset: {ehc.TryOffset}");
            }
            return sb.ToString();
        }

        // This code example produces output similar to the following:
        //
        //Method: Void MethodBodyExample(System.Object)
        //    Local variables are initialized: True
        //    Maximum number of items on the operand stack: 2
        //
        //Local variable: System.Int32 (0)
        //Local variable: System.String (1)
        //Local variable: System.Exception (2)
        //Local variable: System.Boolean (3)
        //
        //Filter
        //      Filter Offset: 71
        //      Handler Length: 23
        //      Handler Offset: 116
        //      Try Block Length: 61
        //      Try Block Offset: 10
        //Clause
        //    Type of exception: System.Exception
        //       Handler Length: 21
        //       Handler Offset: 70
        //     Try Block Length: 61
        //     Try Block Offset: 9
        //Finally
        //       Handler Length: 14
        //       Handler Offset: 94
        //     Try Block Length: 85
        //     Try Block Offset: 9

        private class Store
        {
            public List<Assembly> assemblies = new List<Assembly>();
            public HashSet<string> assemblyNames = new HashSet<string>();

            public static IEnumerable<Assembly> LoadedSolutionAssemblies(Predicate<AssemblyName> predicate)
            {
                var x = new Store();
                foreach (var assembly in UtilityHelper.ReflectionHelper.GetAssemblies(predicate))
                {
                    Recursive(assembly, predicate, ref x);
                }
                return x.assemblies;

                static void Recursive(Assembly assembly, Predicate<AssemblyName> predicate, ref Store x)
                {
                    if (!x.assemblies.Contains(assembly))
                        x.assemblies.Add(assembly);
                    var references = assembly.GetReferencedAssemblies().ToArray();
                    foreach (var assemblyName in from a in references
                                                 where predicate(a)
                                                 select a.Name)
                    {
                        if (x.assemblyNames.Add(assemblyName) && Assembly.Load(assemblyName) is Assembly refAssembly)
                        {
                            Recursive(refAssembly, predicate, ref x);
                        }
                    }
                }
            }
        }
    }
}