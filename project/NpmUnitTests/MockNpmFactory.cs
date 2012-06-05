﻿// -----------------------------------------------------------------------
// <copyright file="MockNpmFactory.cs" company="Microsoft Open Technologies, Inc.">
// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0.
//
// THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT
// LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR
// A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache License, Version 2.0 for specific language governing
// permissions and limitations under the License.
// </copyright>
// -----------------------------------------------------------------------

namespace NpmUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NodeNpm;

    /// <summary>
    /// The class that acts as a class factory and returns a mock client for testing
    /// </summary>
    internal class MockNpmFactory : NpmFactory
    {
        /// <summary>
        /// Get MockNpmClient to support the version.
        /// </summary>
        /// <param name="version">npm version string or null for default</param>
        /// <returns>INpmClient instance</returns>
        public override INpmClient GetClient(string version)
        {
            return new MockNpmClient();
        }
    }
}
