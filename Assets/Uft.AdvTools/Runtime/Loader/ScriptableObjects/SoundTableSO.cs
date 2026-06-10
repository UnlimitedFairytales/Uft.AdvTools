#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Uft.AdvTools.Loader
{
    public class SoundTableSO : ScriptableObject
    {
        [Serializable]
        public class SoundData
        {
            public string label = "";
            public string type = "";
            public AudioClip? clip;
        }

        public List<SoundData> entries = new();
    }
}
