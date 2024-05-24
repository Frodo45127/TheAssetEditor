﻿using Editors.Shared.Core.Services;
using Shared.Core.Misc;
using Shared.Core.PackFiles;
using Shared.Ui.Common;
using Shared.Ui.Editors.BoneMapping;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using View3D.SceneNodes;
using View3D.Utility;

namespace KitbasherEditor.ViewModels.SceneExplorerNodeViews.Rmv2
{
    public class AnimationViewModel : NotifyPropertyChangedImpl
    {
        private readonly KitbasherRootScene _kitbasherRootScene;
        Rmv2MeshNode _meshNode;

        public NotifyAttr<string> SkeletonName { get; set; } = new NotifyAttr<string>("");
        public List<AnimatedBone> AnimatedBones { get; set; }
        public FilterCollection<AnimatedBone> AttachableBones { get; set; } = new FilterCollection<AnimatedBone>(null);

        public AnimationViewModel(KitbasherRootScene kitbasherRootScene, Rmv2MeshNode meshNode, PackFileService pfs, SkeletonAnimationLookUpHelper animLookUp)
        {
            _kitbasherRootScene = kitbasherRootScene;
            _meshNode = meshNode;

            SkeletonName.Value = _meshNode.Geometry.ParentSkeletonName;

            var skeletonFile = animLookUp.GetSkeletonFileFromName(pfs, SkeletonName.Value);
            var bones = _meshNode.Geometry.GetUniqeBlendIndices();

            if (skeletonFile == null)
                SkeletonName.Value = SkeletonName.Value + "[MISSING from packs]";

            // Make sure the bones are valid, mapping can cause issues! 
            if (bones.Count != 0 && skeletonFile != null)
            {
                var activeBonesMin = bones.Min(x => x);
                var activeBonesMax = bones.Max(x => x);
                var skeletonBonesMax = skeletonFile.Bones.Max(x => x.Id);

                bool hasValidBoneMapping = activeBonesMin >= 0 && skeletonBonesMax >= activeBonesMax;
                if (!hasValidBoneMapping)
                    MessageBox.Show("Mesh an invalid bones, this might cause issues. Its a result of an invalid re-rigging");

                var boneList = AnimatedBoneHelper.CreateFlatSkeletonList(skeletonFile);

                if (skeletonFile != null && hasValidBoneMapping)
                {
                    AnimatedBones = boneList
                        .OrderBy(x => x.BoneIndex.Value)
                        .ToList();
                }
            }

            var existingSkeletonMeshNode = _meshNode.GetParentModel();
            var existingSkeltonName = existingSkeletonMeshNode.Model.Header.SkeletonName;
            var existingSkeletonFile = animLookUp.GetSkeletonFileFromName(pfs, existingSkeltonName);
            if (existingSkeletonFile != null)
                AttachableBones.UpdatePossibleValues(AnimatedBoneHelper.CreateFlatSkeletonList(existingSkeletonFile), new AnimatedBone(-1, "none"));

            AttachableBones.SelectedItemChanged += ModelBoneList_SelectedItemChanged;
            AttachableBones.SearchFilter = (value, rx) => { return rx.Match(value.Name.Value).Success; };
            AttachableBones.SelectedItem = AttachableBones.PossibleValues.FirstOrDefault(x => x.Name.Value == _meshNode.AttachmentPointName);
        }

        private void ModelBoneList_SelectedItemChanged(AnimatedBone newValue)
        {
            MainEditableNode mainNode = _meshNode.GetParentModel() as MainEditableNode;
            if (mainNode == null)
                return;

            if (newValue != null && newValue.BoneIndex.Value != -1)
            {
                _meshNode.AttachmentPointName = newValue.Name.Value;
                _meshNode.AttachmentBoneResolver = new SkeletonBoneAnimationResolver(_kitbasherRootScene, newValue.BoneIndex.Value);
            }
            else
            {
                _meshNode.AttachmentPointName = null;
                _meshNode.AttachmentBoneResolver = null;
            }
        }

    }
}
