#nullable enable

using System;
using System.IO;
using Uft.AdvTools.Entities;
using UnityEngine;

namespace Uft.AdvTools.Commands
{
    public class CmdText : ICommand
    {
        // Nested types

        public enum PageCtrlType
        {
            InputBrPage,
            InputBrPageAndNoHide,
            InputBr,
            Input,
            Next,
        }

        // Static members

        public static bool IsNewPage(PageCtrlType lastPageCtrl)
        {
            return lastPageCtrl == PageCtrlType.InputBrPage || lastPageCtrl == PageCtrlType.InputBrPageAndNoHide;
        }

        public static readonly string  PATTERN_OFF = "<Off>";

        // Instance members

        public CommandCategory CommandCategory { get; } = CommandCategory.Text;

        protected string Name { get; set; }
        protected string Text { get; set; }
        protected PageCtrlType PageCtrl { get; set; }
        protected string? Voice { get; set; }
        protected string WindowType { get; set; } // NOTE: 対応予定なし

        // NOTE: Character値
        protected string? Pattern { get; set; } // NOTE: "<Off>"の場合、画像表示の抑制が可能
        protected int? ImageIndex { get; set; }
        protected float? PosX { get; set; }
        protected float? PosY { get; set; }
        protected float FadeSeconds { get; set; }

        public CmdText(string? name, string? text, string? pageCtrl, string? voice, string? windowType,
            string? pattern, int? imageIndex, float? posX, float? posY, float? fadeSeconds)
        {
            this.Name = name ?? "";
            this.Text = text ?? "";
            this.PageCtrl = Enum.TryParse<PageCtrlType>(pageCtrl, true, out var casted) ? casted : PageCtrlType.InputBrPage;
            this.Voice = voice;
            this.WindowType = windowType ?? "";

            // NOTE: Character値
            this.Pattern = pattern;
            this.ImageIndex = imageIndex is int idx ?
                Mathf.Clamp(idx, 0, SpriteManager.IMG_COUNT - 1) :
                null;
            this.PosX = posX;
            this.PosY = posY;
            this.FadeSeconds = fadeSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            scenarioExecutor.IsWaitingForInput = true;

            var characterOrNull = advRoot.CharacterDictionary.ContainsKey(this.Name) ? advRoot.CharacterDictionary[this.Name] : null;
            if (characterOrNull != null)
            {
                this.SetCharacter(advRoot, characterOrNull);
                this.PlayVoice(advRoot);
            }

            // NOTE: 本文がEmptyの場合、キャラクター名も表示せず、Next扱いにする
            var name
                = !string.IsNullOrEmpty(this.Text) ? (characterOrNull?.NameText ?? this.Name) : "";
            var pageCtrl
                = !string.IsNullOrEmpty(this.Text) ? this.PageCtrl : PageCtrlType.Next;
            if (pageCtrl == PageCtrlType.Next)
            {
                advRoot.IsAutoInputOnce = true;
            }
            advRoot.SetText(characterOrNull, name, this.Text, pageCtrl, this.WindowType);
            advRoot.AutoNext.ClearCounter();
            advRoot.AutoNext.SetIsAutoNextReadyTimeAdjust(this.Text.Length, string.IsNullOrWhiteSpace(this.Voice) ? AutoNext.DEFAULT_ADJUST_WEIGHT : 0.01f);
        }

        void SetCharacter(AdvRoot advRoot, Character character)
        {
            var pattern = string.IsNullOrWhiteSpace(this.Pattern) ? character.LastPattern : this.Pattern;
            var imageIndex = this.ImageIndex ?? character.LastImageIndex;
            if (pattern == PATTERN_OFF)
            {
                if (advRoot.SpriteManager.GetCharacterView(character) != null)
                {
                    advRoot.SpriteManager.SetCharacterOff(character, this.FadeSeconds);
                }
                character.ResetLastStatus();
            }
            else
            {
                var detail = character.CharacterDetailDictionary[pattern];
                advRoot.SpriteManager.SetCharacter(character, detail, imageIndex, this.PosX, this.PosY, this.FadeSeconds);
                if (character.Animator != null && !string.IsNullOrWhiteSpace(detail.AnimatorState))
                {
                    var info = character.Animator.GetCurrentAnimatorStateInfo(0);
                    // NOTE: ループアニメーションかつ同一パターンの場合、再プレイはしない
                    if (!info.loop || character.LastPattern != pattern)
                    {
                        character.Animator.Play(detail.AnimatorState);
                    }
                }
                character.LastPattern = pattern;
                character.LastImageIndex = imageIndex;
            }
        }

        void PlayVoice(AdvRoot advRoot)
        {
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
        }
    }
}
