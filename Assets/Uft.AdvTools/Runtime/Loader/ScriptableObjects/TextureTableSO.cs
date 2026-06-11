#nullable enable

using System;
using System.Collections.Generic;
using Uft.UnityUtils.UI;
using UnityEngine;

namespace Uft.AdvTools.Loader
{
    public class TextureTableSO : ScriptableObject
    {
        [Serializable]
        public class TextureData
        {
            public string label = "";
            public string type = "";
            public float x;
            public float y;
            public AnchorPreset pivot = AnchorPreset.MiddleCenter;
            public float scale = 1f;
            public string fileName = "";
            public string fileType = "";
        }

        public List<TextureData> entries = new();
    }
}
