#nullable enable

using System;
using System.IO;
using UnityEngine;

namespace Uft.AdvTools.Commands
{
    public class CmdText : ICommand
    {
        public static bool IsNewPage(PageCtrlType lastPageCtrl)
        {
            return lastPageCtrl == PageCtrlType.InputBrPage || lastPageCtrl == PageCtrlType.InputBrPageAndNoHide;
        }

        public static readonly string  PATTERN_OFF = "<Off>";

        public enum PageCtrlType
        {
            InputBrPage,
            InputBrPageAndNoHide,
            InputBr,
            Input,
            Next,
        }

        public CommandCategory CommandCategory { get; } = CommandCategory.Text;

        protected string Name { get; set; }
        protected string Text { get; set; }
        protected PageCtrlType PageCtrl { get; set; }
        protected string? Voice { get; set; }
        protected string WindowType { get; set; } // NOTE: 対応予定なし

        // NOTE: Character値
        protected string? Pattern { get; set; } // NOTE: "<Off>"の場合、画像表示の抑制が可能
        protected int? ImageIndex { get; set; }
        protected float? OffsetX { get; set; }
        protected float? OffsetY { get; set; }
        protected float FadeSeconds { get; set; }

        public CmdText(string? name, string? text, string? pageCtrl, string? voice, string? windowType,
            string? pattern, int? imageIndex, float? offsetX, float? offsetY, float? fadeSeconds)
        {
            this.Name = name ?? "";
            this.Text = text ?? "";
            this.PageCtrl = Enum.TryParse<PageCtrlType>(pageCtrl, true, out var casted) ? casted : PageCtrlType.InputBrPage;
            this.Voice = voice;
            this.WindowType = windowType ?? "";

            // NOTE: Character値
            this.Pattern = pattern;
            this.ImageIndex = imageIndex is int idx ?
                Mathf.Clamp(idx, 0, 7) :
                null;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
            this.FadeSeconds = fadeSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            scenarioExecutor.IsWaitingForInput = true;

            // NOTE: 本文がEmptyの場合、キャラクター名も表示せず、Next扱いにする
            var character = advRoot.CharacterDictionary.ContainsKey(this.Name) ? advRoot.CharacterDictionary[this.Name] : null;
            var name = "";
            var pageCtrl = PageCtrlType.Next;
            if (!string.IsNullOrEmpty(this.Text))
            {
                name = character?.NameText ?? this.Name;
                pageCtrl = this.PageCtrl;
            }
            if (pageCtrl == PageCtrlType.Next)
            {
                advRoot.IsAutoInputOnce = true;
            }

            if (character != null)
            {
                // 1. Character sprite
                // NOTE: Arg2～Arg5（Pattern、ImageIndex、OffsetX、OffsetY）が空欄の場合は、デフォルトまたは現在の表示を継続する
                var pattern = string.IsNullOrWhiteSpace(this.Pattern) ? character.LastPattern : this.Pattern;
                var imageIndex = character.LastImageIndex;
                var x = character.LastOffsetX;
                var y = character.LastOffsetY;
                if (pattern == PATTERN_OFF)
                {
                    if (advRoot.SpriteManager.IsCharacterDisplayed(character))
                    {
                        advRoot.SpriteManager.SetCharacterOff(character, this.FadeSeconds);
                    }
                }
                else
                {
                    var detail = character.CharacterDetailDictionary[pattern];

                    var sprite = detail.Sprite;
                    imageIndex = this.ImageIndex ?? character.LastImageIndex;
                    x = this.OffsetX ?? character.LastOffsetX;
                    y = this.OffsetY ?? character.LastOffsetY;
                    var pivot = detail.Pivot;
                    var scale = detail.Scale;

                    advRoot.SpriteManager.SetCharacter(character, sprite, imageIndex, x, y, pivot, scale, this.FadeSeconds);
                }
                character.LastPattern = pattern;
                character.LastImageIndex = imageIndex;
                character.LastOffsetX = x;
                character.LastOffsetY = y;

                // 2. Text
                advRoot.SetText(character, name, this.Text, pageCtrl, this.WindowType);

                // 3. Voice
                if (!string.IsNullOrWhiteSpace(this.Voice))
                {
                    // NOTE: 宴と異なりSoundシートでのType=Voiceに対応してある (本来の宴4はファイル直接記入のみ)
                    var voiceClip = advRoot.AllowsVoiceLabel && advRoot.VoiceDictionary.ContainsKey(this.Voice) ?
                            advRoot.VoiceDictionary[this.Voice] :
                            Resources.Load<AudioClip>(advRoot.VoiceRoot + Path.ChangeExtension(this.Voice, null));
                    advRoot.SoundManager.PlayVoice(voiceClip, false, 1.0f);
                }
                else
                {
                    advRoot.SoundManager.StopVoice();
                }
                advRoot.AutoNext.ClearCounter();
                advRoot.AutoNext.SetIsAutoNextReadyTimeAdjust(this.Text.Length, string.IsNullOrWhiteSpace(this.Voice) ? AutoNext.DEFAULT_ADJUST_WEIGHT : 0.01f);
            }
            else
            {
                advRoot.SetText(null, name, this.Text, pageCtrl, this.WindowType);
                advRoot.AutoNext.ClearCounter();
                advRoot.AutoNext.SetIsAutoNextReadyTimeAdjust(this.Text.Length);
            }
        }
    }
}
