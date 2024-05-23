﻿using SharedCore.ByteParsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Audio.FileFormats.WWise.Hirc.V122
{
    public class CAkEvent_v122 : HircItem, ICAkEvent
    {
        public class Action
        {
            public uint ActionId { get; set; }
        }

        public List<Action> Actions { get; set; } = new List<Action>();

        protected override void CreateSpecificData(ByteChunk chunk)
        {
            var actionCount = chunk.ReadUInt32();
            for (int i = 0; i < actionCount; i++)
                Actions.Add(new Action() { ActionId = chunk.ReadUInt32() });
        }

        public List<uint> GetActionIds()
        {
            return Actions.Select(x => x.ActionId).ToList();
        }

        public override void UpdateSize() => throw new NotImplementedException();
        public override byte[] GetAsByteArray() => throw new NotImplementedException();
    }
}
