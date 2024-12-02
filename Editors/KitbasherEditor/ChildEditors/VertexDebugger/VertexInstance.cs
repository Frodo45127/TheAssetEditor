﻿using Microsoft.Xna.Framework;

namespace KitbasherEditor.ViewModels.VertexDebugger
{

    public class VertexInstance
    {
        public int Id { get; set; }
        public Vector4 AnimIndecies { get; set; }
        public Vector4 AnimWeights { get; set; }
        public float TotalWeight { get; set; }

        public Vector3 Normal { get; set; }
        public float NormalLength { get; set; }
        public Vector3 BiNormal { get; set; }
        public float BiNormalLength { get; set; }
        public Vector3 Tangent { get; set; }
        public float TangentLength { get; set; }

        public Vector4 Position { get; set; }

    }

}
