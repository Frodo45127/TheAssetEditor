﻿using System;
using System.IO;
using System.Linq;
using CommonControls.BaseDialogs.ErrorListDialog;
using CommonControls.Common;
using CommonControls.Editors.AnimationPack;
using CommonControls.FileTypes.AnimationPack;
using CommonControls.FileTypes.AnimationPack.AnimPackFileTypes;
using CommonControls.FileTypes.PackFiles.Models;

namespace CommonControls.Services
{
    public class AnimPackUpdaterService
    {
        private readonly PackFileService _pfs;

        public AnimPackUpdaterService(PackFileService pfs)
        {
            _pfs = pfs;
        }

        public void Process(PackFileContainer packFileContainer, GameTypeEnum existingPackVersion = GameTypeEnum.Warhammer2, GameTypeEnum outputFormat = GameTypeEnum.Warhammer3)
        {
            var errorList = new ErrorListViewModel.ErrorList();

            if (outputFormat != GameTypeEnum.Warhammer3)
                throw new Exception($"{outputFormat} selected as output, only Warhammer 3 is currently supported");

            if (existingPackVersion != GameTypeEnum.Warhammer2)
                throw new Exception($"{outputFormat} selected as input, only Warhammer 2 is currently supported");

            var animPackFiles = _pfs.FindAllWithExtention(".animpack", packFileContainer);
            var animPacks = animPackFiles.Select(x => AnimationPackSerializer.Load(x, _pfs, GameTypeEnum.Warhammer2)).ToArray();

            if (animPacks.Length == 0)
                throw new Exception("No animation packs found in the packfile");

            foreach (var animPack in animPacks)
            {
                var outputWh3AnimPack = new AnimationPackFile();

                var unknownFilesCount = animPack.Files.Count(x => x is IMatchedCombatBin || x is UnknownAnimFile);
                if (unknownFilesCount != 0)
                    throw new Exception($"AnimPack {animPack.FileName} contains {unknownFilesCount} unknown files");

                var animFrags = animPack.Files.Where(x => x is AnimationFragmentFile).Cast<AnimationFragmentFile>();
                var animBins = animPack.Files.Where(x => x is AnimationBin).Cast<AnimationBin>();
                var animationBinEntries = animBins.SelectMany(x => x.AnimationTableEntries).ToArray();

                var processedFragments = 0;
                var processedBinEntries = 0;

                foreach (var binEntry in animationBinEntries)
                {
                    var wh3Bin = new FileTypes.AnimationPack.AnimPackFileTypes.Wh3.AnimationBinWh3(binEntry.Name);
                    wh3Bin.SkeletonName = binEntry.SkeletonName;
                    wh3Bin.MountBin = binEntry.MountName;
                    wh3Bin.LocomotionGraph = "animations/locomotion_graphs/entity_locomotion_graph.xml";

                    foreach (var fragment in animFrags)
                    {
                        //ProcessWhFragment(fragment, ref wh3Bin, ref errorList);
                        processedFragments++;
                    }

                    processedBinEntries++;
                    outputWh3AnimPack.AddFile(wh3Bin);
                }

                var animPackPathWithoutExtentions = Path.GetFileNameWithoutExtension(animPack.FileName);
                var outputAnimPackName = AnimationPackSampleDataCreator.GenerateWh3AnimPackName(animPackPathWithoutExtentions + "_wh3");
                SaveHelper.Save(_pfs, outputAnimPackName, null, AnimationPackSerializer.ConvertToBytes(outputWh3AnimPack), false);
            }
        }

        //void ProcessWhFragment(AnimationFragmentFile fragmentToProcess, ref FileTypes.AnimationPack.AnimPackFileTypes.Wh3.AnimationBinWh3 wh3Bin, ref ErrorListViewModel.ErrorList errorList)
        //{
        //    foreach (var animationSetEntry in fragmentToProcess.Fragments)
        //    {
        //        var newBinEntry = new FileTypes.AnimationPack.AnimPackFileTypes.Wh3.AnimationBinEntry();
        //
        //        var wh2Slot = DefaultAnimationSlotTypeHelper.GetFromId(animationSetEntry.Slot.Id);
        //        var wh3Slot = AnimationSlotTypeHelperWh3.GetfromValue(wh2Slot.Value);
        //
        //        newBinEntry.AnimationId = (uint)wh3Slot.Id;
        //        newBinEntry.BlendIn = animationSetEntry.BlendInTime;
        //        newBinEntry.SelectionWeight = animationSetEntry.SelectionWeight;
        //        newBinEntry.WeaponBools = animationSetEntry.WeaponBone;
        //        newBinEntry.AnimationRefs.Add(new FileTypes.AnimationPack.AnimPackFileTypes.Wh3.AnimationBinEntry.AnimationRef()
        //        {
        //            AnimationFile = "",
        //            AnimationMetaFile = "",
        //            AnimationSoundMetaFile = "",
        //        });
        //
        //        wh3Bin.AnimationTableEntries.Add(newBinEntry);
        //
        //        processedFragments++;
        //    }
        //}
    }
}
