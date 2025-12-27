#nullable enable

using System.Collections.Generic;
using Uft.UnityUtils;
using Uft.UnityUtils.UI;
using UnityEngine;

namespace Uft.AdvTools.Entities
{
    public class Character
    {
        // Parameters

        public string CharacterName { get; protected set; } // key
        public string NameText { get; protected set; }
        public FileType FileType { get; protected set; }

        /// <summary>単一キャラクター内のPrefabは全て同一な想定しかしない</summary>
        public GameObject? Prefab { get; protected set; }
        GameObject? _instantiated = null;
        public GameObject? Instantiated
        {
            get
            {
                this._instantiated = CacheUtil.GetCreatedObject(this._instantiated, this.Prefab);
                return this._instantiated;
            }
        }
        bool _hasTriedToGetAnimator;
        Animator? _animator = null;
        public Animator? Animator
        {
            get
            {
                if (this.Instantiated == null) return null;
                if (this._hasTriedToGetAnimator) return this._animator;
                this._hasTriedToGetAnimator = true;
                this._animator = CacheUtil.GetCachedChildComponent(this._animator, this.Instantiated.transform, true);
                return this._animator;
            }
        }


        public CharacterDetail DefaultDetail { get; protected set; }
        public Dictionary<string, CharacterDetail> CharacterDetailDictionary { get; protected set; } = new Dictionary<string, CharacterDetail>();

        // Status

        /// <summary>初期値、CharacterOff後はDefaultDetailの値になる想定。</summary>
        public string LastPattern { get; set; }
        /// <summary>初期値、CharacterOff後はDefaultDetailの値になる想定。</summary>
        public int LastImageIndex { get; set; }

        // Methods

        public Character(string characterName, string nameText, FileType fileType, GameObject? prefab, CharacterDetail defaultDetail)
        {
            this.CharacterName = characterName;
            this.NameText = nameText;
            this.FileType = fileType;
            this.Prefab = prefab;
            this.DefaultDetail = defaultDetail;
            this.CharacterDetailDictionary.Add(defaultDetail.Pattern, defaultDetail);

            this.LastPattern = this.DefaultDetail.Pattern; // NOTE: [MemberNotNull(nameof(LastPattern))]の代替。Unityでは.NET5以降のRoslynに対応できないため妥協
            this.ResetLastStatus();
        }

        public void ResetLastStatus()
        {
            this.LastPattern = this.DefaultDetail.Pattern;
            this.LastImageIndex = 0;
        }

        public override string ToString() => $"{this.CharacterName}, Count={this.CharacterDetailDictionary.Count}";
    }

    public class CharacterDetail
    {
        // Parameters

        public string Pattern { get; protected set; } // key
        public float OffsetX { get; protected set; }
        public float OffsetY { get; protected set; }
        public float OffsetZ { get; protected set; }
        public AnchorPreset Pivot { get; protected set; }
        public float Scale { get; protected set; }
        public Sprite? Sprite { get; protected set; }
        public string? AnimatorState { get; protected set; }

        // Status

        public CharacterDetail(
            string pattern,
            float? offsetX,
            float? offsetY,
            float? offsetZ,
            AnchorPreset? pivot,
            float? scale,
            Sprite? sprite,
            string? animatorState)
        {
            this.Pattern = pattern;
            this.OffsetX = offsetX ?? 0;
            this.OffsetY = offsetY ?? 0;
            this.OffsetZ = offsetZ ?? 0;
            this.Pivot = pivot ?? AnchorPreset.MiddleCenter;
            this.Scale = scale ?? 1.0f;
            this.Sprite = sprite;
            this.AnimatorState = animatorState;
        }

        public override string ToString() =>
            $"{this.Pattern},{this.OffsetX},{this.OffsetY},{this.OffsetZ},{this.Pivot},{this.Scale},{this.Sprite},{this.AnimatorState}";
    }
}
