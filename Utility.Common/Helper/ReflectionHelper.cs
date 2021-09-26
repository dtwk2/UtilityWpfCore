﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

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
    }
}
