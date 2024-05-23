﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CommonControls.FileTypes.RigidModel;
using CommonControls.FileTypes.RigidModel.Types;
using SharedCore.PackFiles.Models;

namespace CommonControls.FileTypes.WsModel
{
    public class WsModelMaterialFile
    {
        public bool Alpha { get; set; } = false;
        public Dictionary<TextureType, string> Textures { get; set; } = new Dictionary<TextureType, string>();
        public UiVertexFormat VertexType { get; set; } = UiVertexFormat.Unknown;
        public string Name { get; set; }
        public string FullPath { get; set; }

        public WsModelMaterialFile(PackFile pf, string fullPath)
        {
            FullPath = fullPath;

            var buffer = pf.DataSource.ReadData();
            string xmlString = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            var nameNode = doc.SelectSingleNode(@"/material/name");
            if (nameNode != null)
            {
                Name = nameNode.InnerText;
                if (Name.Contains("alpha_on", StringComparison.InvariantCultureIgnoreCase))
                    Alpha = true;

                if (Name.Contains("weighted4", StringComparison.InvariantCultureIgnoreCase))
                    VertexType = UiVertexFormat.Cinematic;
                else if (Name.Contains("weighted2", StringComparison.InvariantCultureIgnoreCase))
                    VertexType = UiVertexFormat.Weighted;
                else
                    VertexType = UiVertexFormat.Static;
            }

            var textureNodes = doc.SelectNodes(@"/material/textures/texture");
            foreach (XmlNode node in textureNodes)
            {
                var slotNode = node.SelectNodes("slot");
                var pathNode = node.SelectNodes("source");

                if (pathNode.Count == 0 || slotNode.Count == 0)
                    continue;

                var textureSlotName = slotNode[0].InnerText;
                var texturePath = pathNode[0].InnerText;

                if (textureSlotName == "s_diffuse")
                    Textures[TextureType.Diffuse] = texturePath;

                if (textureSlotName == "s_gloss")
                    Textures[TextureType.Gloss] = texturePath;

                if (textureSlotName == "s_mask")
                    Textures[TextureType.Mask] = texturePath;
                else if (textureSlotName == "s_mask1")
                    Textures[TextureType.Mask] = texturePath;
                else if (textureSlotName == "t_xml_mask")
                    Textures[TextureType.Mask] = texturePath;

                if (textureSlotName == "s_normal")
                    Textures[TextureType.Normal] = texturePath;
                else if (textureSlotName == "t_xml_normal")
                    Textures[TextureType.Normal] = texturePath;

                if (textureSlotName == "s_specular")
                    Textures[TextureType.Specular] = texturePath;

                if (textureSlotName == "t_xml_base_colour")
                    Textures[TextureType.BaseColour] = texturePath;
                else if (textureSlotName == "s_base_colour")
                    Textures[TextureType.BaseColour] = texturePath;

                if (textureSlotName == "t_xml_material_map")
                    Textures[TextureType.MaterialMap] = texturePath;
            }
        }
    }
}
