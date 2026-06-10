#nullable enable

using System;
using System.Collections.Generic;
using Uft.UnityUtils.UI;
using UnityEngine;

namespace Uft.AdvTools.Loader
{
    public class CharacterTableSO : ScriptableObject
    {
        [Serializable]
        public class DetailData
        {
            public string pattern = "";
            public float x;
            public float y;
            public float z;
            public AnchorPreset pivot = AnchorPreset.MiddleCenter;
            public float scale = 1f;
            public Sprite? sprite;
            public string animationState = "";
        }

        [Serializable]
        public class CharacterData
        {
            public string characterName = "";
            public string nameText = "";
            public string fileTypeValue = "";
            public GameObject? prefab;
            public List<DetailData> details = new();
        }

        public List<CharacterData> characters = new();
    }
}
