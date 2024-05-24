﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Shared.GameFormats.AnimationMeta.Parsing;

namespace Shared.GameFormats.AnimationMeta.Definitions
{
    [MetaData("RESCALE", 10)]
    public class ReScale : DecodedMetaEntryBase
    {
        [MetaDataTag(5, "")]
        public float Scale { get; set; }
    }
}
