﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace CommonControls.FileTypes.RigidModel.Transforms
{
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    [Serializable]
    public struct RvmBoundingBox
    {
        public float MinimumX;
        public float MinimumY;
        public float MinimumZ;
        public float MaximumX;
        public float MaximumY;
        public float MaximumZ;

        public void UpdateBoundingBox(BoundingBox newBB)
        {
            MinimumX = newBB.Min.X;
            MinimumY = newBB.Min.Y;
            MinimumZ = newBB.Min.Z;

            MaximumX = newBB.Max.X;
            MaximumY = newBB.Max.Y;
            MaximumZ = newBB.Max.Z;
        }

        public float Width { get => Math.Abs(MinimumX - MaximumX); }
        public float Height { get => Math.Abs(MinimumY - MaximumY); }
        public float Depth { get => Math.Abs(MinimumZ - MaximumZ); }
    }
}
