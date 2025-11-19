#nullable enable

using System.Collections.Generic;
using Uft.UnityUtils.UI;
using UnityEngine;

namespace Uft.AdvTools.Entities
{
    public class Character
    {
        // Parameters

        public string CharacterName { get; protected set; } // key
        public string NameText { get; protected set; }
        public CharacterDetail DefaultDetail { get; protected set; }
        public Dictionary<string, CharacterDetail> CharacterDetailDictionary { get; protected set; } = new Dictionary<string, CharacterDetail>();

        // Status

        /// <summary>初期値、CharacterOff後はDefaultDetailの値になる想定。</summary>
        public string LastPattern { get; set; }
        /// <summary>初期値、CharacterOff後はDefaultDetailの値になる想定。</summary>
        public int LastImageIndex { get; set; }
        /// <summary>初期値、CharacterOff後はDefaultDetailの値になる想定。</summary>
        public float LastOffsetX { get; set; }
        /// <summary>初期値、CharacterOff後はDefaultDetailの値になる想定。</summary>
        public float LastOffsetY { get; set; }

        // Methods

        public Character(string characterName, string nameText, CharacterDetail defaultDetail)
        {
            this.CharacterName = characterName;
            this.NameText = nameText;
            this.DefaultDetail = defaultDetail;
            this.CharacterDetailDictionary.Add(defaultDetail.Pattern, defaultDetail);

            this.LastPattern = this.DefaultDetail.Pattern; // NOTE: [MemberNotNull(nameof(LastPattern))]の代替。Unityでは.NET5以降のRoslynに対応できないため妥協
            this.ResetLastStatus();
        }

        public void ResetLastStatus()
        {
            this.LastPattern = this.DefaultDetail.Pattern;
            this.LastImageIndex = 0;
            this.LastOffsetX = this.DefaultDetail.OffsetX;
            this.LastOffsetY = this.DefaultDetail.OffsetY;
        }

        public override string ToString() => $"{this.CharacterName}, Count={this.CharacterDetailDictionary.Count}";
    }

    public class CharacterDetail
    {
        public string Pattern { get; protected set; } // key
        public float OffsetX { get; protected set; }
        public float OffsetY { get; protected set; }
        public AnchorPreset Pivot { get; protected set; }
        public float Scale { get; protected set; }
        public Sprite? Sprite { get; protected set; }

        public CharacterDetail(
            string pattern,
            float? offsetX,
            float? offsetY,
            AnchorPreset? pivot,
            float? scale,
            Sprite? sprite)
        {
            this.Pattern = pattern;
            this.OffsetX = offsetX ?? 0;
            this.OffsetY = offsetY ?? 0;
            this.Pivot = pivot ?? AnchorPreset.MiddleCenter;
            this.Scale = scale ?? 1.0f;
            this.Sprite = sprite;
        }

        public override string ToString() =>
            $"{this.Pattern},{this.OffsetX},{this.OffsetY},{this.Pivot},{this.Scale},{this.Sprite}";
    }
}
