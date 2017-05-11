﻿/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

namespace Slalom.Stacks.Akka.EndPoints
{
    public class GetInventoryCommand
    {
        public GetInventoryCommand(string remotePath)
        {
            this.RemotePath = remotePath;
        }

        public string RemotePath { get; }
    }
}