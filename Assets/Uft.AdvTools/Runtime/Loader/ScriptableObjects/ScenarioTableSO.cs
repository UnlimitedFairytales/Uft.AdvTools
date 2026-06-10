#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Uft.AdvTools.Loader
{
    public class ScenarioTableSO : ScriptableObject
    {
        [Serializable]
        public class RowData
        {
            public string command = "";
            public string arg1 = "";
            public string arg2 = "";
            public string arg3 = "";
            public string arg4 = "";
            public string arg5 = "";
            public string arg6 = "";
            public string waitType = "";
            public string text = "";
            public string pageCtrl = "";
            public string voice = "";
            public string windowType = "";
        }

        public List<RowData> rows = new();
    }
}
