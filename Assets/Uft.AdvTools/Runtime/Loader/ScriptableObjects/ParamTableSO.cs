#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Uft.AdvTools.Loader
{
    public class ParamTableSO : ScriptableObject
    {
        [Serializable]
        public class ParamData
        {
            public string label = "";
            public int value;
        }

        public List<ParamData> entries = new();
    }
}
