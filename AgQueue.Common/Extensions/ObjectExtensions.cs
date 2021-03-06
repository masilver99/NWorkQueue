﻿// <copyright file="ObjectExtensions.cs" company="Michael Silver">
// Copyright (c) Michael Silver. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace AgQueue.Common.Extensions
{
    /// <summary>
    /// Extensions methods for use on Objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Throws ArgumentNullException if object is null.
        /// </summary>
        /// <param name="obj">Object to perform null check on.</param>
        /// <param name="name">Param name to include in exception message.</param>
        public static void ThrowIfNull(this object? obj, string? name = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
