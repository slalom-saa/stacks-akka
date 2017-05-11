/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System;

namespace Slalom.Stacks.Akka
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EndPointHostAttribute : Attribute
    {
        public EndPointHostAttribute(params string[] paths)
        {
            this.Paths = paths;
        }

        public string[] Paths { get; }
    }
}