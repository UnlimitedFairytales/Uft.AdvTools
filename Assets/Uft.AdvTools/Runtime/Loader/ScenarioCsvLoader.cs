#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using Uft.AdvTools.Commands;
using Uft.UnityUtils;
using Uft.UnityUtils.Common;

namespace Uft.AdvTools.Loader
{
    /// <summary>「必須チェック」「キャスト」までが責任範囲</summary>
    public class ScenarioCsvLoader
    {
        // static

        // Text
        public const string Text = "text";

        // Object
        public const string CharacterOff = "characteroff";
        public const string Bg = "bg";
        public const string BgOff = "bgoff";
        public const string Sprite = "sprite";
        public const string SpriteOff = "spriteoff";

        // Sound
        public const string Se = "se";
        public const string Bgm = "bgm";
        public const string StopBgm = "stopbgm";

        // Effect
        public const string Wait = "wait";
        public const string FadeOut = "fadeout";
        public const string FadeIn = "fadein";
        public const string Tween = "tween";
        public const string ImageEffect = "imageeffect";
        public const string ImageEffectOff = "imageeffectoff";

        // UI
        public const string HideMessageWindow = "hidemessagewindow";
        public const string ShowMessageWindow = "showmessagewindow";
        public const string ChangeMessageWindow = "changemessagewindow";

        // Logic
        public const string Param = "param";
        public const string Jump = "jump";
        public const string SelectionTitle = "selectiontitle"; // 宴にない非互換機能
        public const string Selection = "selection";

        // instance

        public IReadOnlyList<ICommand> Load(FileInfo fileInfo, string sheetName)
        {
            var csvDtoList = ScenarioCsvDto.Load(fileInfo);
            return this.LoadInner(csvDtoList, sheetName);
        }

        public IReadOnlyList<ICommand> Load(string csvText, string sheetName)
        {
            var csvDtoList = ScenarioCsvDto.Load(csvText);
            return this.LoadInner(csvDtoList, sheetName);
        }

        IReadOnlyList<ICommand> LoadInner(IReadOnlyList<ScenarioCsvDto> csvDtoList, string sheetName)
        {
            var commandList = new List<ICommand>();
            int i = 0;
            ScenarioCsvDto? dto = null;
            try
            {
                for (i = 0; i < csvDtoList.Count; i++)
                {
                    dto = csvDtoList[i];
                    if (dto.IsAllNullOrWhiteSpace()) continue;
                    var cmd = dto.Command!.ToLowerInvariant();

                    // カスタム用
                    var loaded = this.CustomCommand(dto, cmd);
                    if (loaded != null)
                    {
                        commandList.Add(loaded);
                        continue;
                    }

                    switch (cmd)
                    {
                        // Text
                        case "":
                        case Text:
                            {
                                commandList.Add(new CmdText(dto.Arg1, dto.Text, dto.PageCtrl, dto.Voice, dto.WindowType,
                                    dto.Arg2,
                                    int.TryParse(dto.Arg3, out var index) ? index : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg4, out var x) ? x : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg5, out var y) ? y : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg6, out var fadeSeconds) ? fadeSeconds : null));
                            }
                            break;
                        // Object
                        case CharacterOff:
                            {
                                if (string.IsNullOrWhiteSpace(dto.Arg1)) throw new Exception($"{nameof(CmdCharacterOff)} : Arg1 is required.");
                                commandList.Add(new CmdCharacterOff(
                                    dto.Arg1,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg6, out var fadeSeconds) ? fadeSeconds : null));
                            }
                            break;
                        case Bg:
                            {
                                if (string.IsNullOrWhiteSpace(dto.Arg1)) throw new Exception($"{nameof(CmdBg)} : Arg1 is required.");
                                commandList.Add(new CmdBg(
                                    dto.Arg1,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg4, out var x) ? x : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg5, out var y) ? y : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg6, out var fadeSeconds) ? fadeSeconds : null));
                            }
                            break;
                        case BgOff:
                            {
                                commandList.Add(new CmdBgOff(
                                    InvariantCultureUtil.FloatTryParse(dto.Arg6, out var fadeSeconds) ? fadeSeconds : null));
                            }
                            break;
                        case Sprite:
                            {
                                if (string.IsNullOrWhiteSpace(dto.Arg1)) throw new Exception($"{nameof(CmdSprite)} : Arg1 is required.");
                                commandList.Add(new CmdSprite(
                                    dto.Arg1,
                                    int.TryParse(dto.Arg3, out var index) ? index : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg4, out var x) ? x : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg5, out var y) ? y : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg6, out var fadeSeconds) ? fadeSeconds : null));
                            }
                            break;
                        case SpriteOff:
                            {
                                if (string.IsNullOrWhiteSpace(dto.Arg1)) throw new Exception($"{nameof(CmdSpriteOff)} : Arg1 is required.");
                                commandList.Add(new CmdSpriteOff(
                                    dto.Arg1,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg6, out var fadeSeconds) ? fadeSeconds : null));
                            }
                            break;
                        // Sound
                        case Se:
                            {
                                if (string.IsNullOrWhiteSpace(dto.Arg1)) throw new Exception($"{nameof(CmdSe)} : Arg1 is required.");
                                commandList.Add(new CmdSe(
                                    dto.Arg1,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg3, out var volume) ? volume : null));
                            }
                            break;
                        case Bgm:
                            {
                                if (string.IsNullOrWhiteSpace(dto.Arg1)) throw new Exception($"{nameof(CmdBgm)} : Arg1 is required.");
                                commandList.Add(new CmdBgm(
                                    dto.Arg1,
                                    bool.TryParse(dto.Arg2, out var isLoop) ? isLoop : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg3, out var volume) ? volume : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg5, out var prevFadeOutSeconds) ? prevFadeOutSeconds : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg6, out var fadeInSeconds) ? fadeInSeconds : null));
                            }
                            break;
                        case StopBgm:
                            {
                                commandList.Add(new CmdStopBgm(InvariantCultureUtil.FloatTryParse(dto.Arg6, out var fadeOutSeconds) ? fadeOutSeconds : null));
                            }
                            break;
                        // Effect
                        case Wait:
                            {
                                if (!InvariantCultureUtil.FloatTryParse(dto.Arg6, out var waitSeconds)) throw new Exception($"{nameof(CmdWait)} : Arg6 is required.");
                                commandList.Add(new CmdWait(waitSeconds));
                            }
                            break;
                        case FadeOut:
                            {
                                commandList.Add(new CmdFadeOut(dto.Arg1, dto.Arg2,
                                    dto.Arg3,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg4, out var softness) ? softness : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg6, out var fadeSeconds) ? fadeSeconds : null));
                            }
                            break;
                        case FadeIn:
                            {
                                commandList.Add(new CmdFadeIn(dto.Arg1, dto.Arg2,
                                    dto.Arg3,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg4, out var softness) ? softness : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg6, out var fadeSeconds) ? fadeSeconds : null));
                            }
                            break;
                        case Tween:
                            {
                                var arg4IsSpecified = !string.IsNullOrWhiteSpace(dto.Arg4);
                                if (string.IsNullOrWhiteSpace(dto.Arg1) ||
                                    string.IsNullOrWhiteSpace(dto.Arg2) ||
                                    string.IsNullOrWhiteSpace(dto.Arg3)) throw new Exception($"{nameof(CmdTween)} : Arg1,Arg2,Arg3 is required.");
                                if (!CmdTween.TweenTypeTryParse(dto.Arg2, out var tween)) throw new Exception($"{nameof(CmdTween)} : Arg2 unsupported tweenType. {dto.Arg2}");
                                if (!CmdTween.TweenParameter.TryParse(tween, dto.Arg3, out var tweenParameter)) throw new Exception($"{nameof(CmdTween)} : Arg3 unsupported tweenParameter. : {dto.Arg3}");
                                if (!CmdTween.EaseTryParse(dto.Arg4, out var ease) && arg4IsSpecified) throw new Exception($"{nameof(CmdTween)} : Arg4 unsupported ease. : {dto.Arg4}");
                                commandList.Add(new CmdTween(dto.Arg1, tween, tweenParameter, arg4IsSpecified ? ease : null));
                            }
                            break;
                        case ImageEffect:
                        case ImageEffectOff:
                            {
                                var isOn = cmd == ImageEffect;
                                if (string.IsNullOrWhiteSpace(dto.Arg2)) throw new Exception($"{nameof(CmdImageEffect)} or Off : Arg2 is required.");
                                commandList.Add(new CmdImageEffect(isOn, dto.Arg2, InvariantCultureUtil.FloatTryParse(dto.Arg6, out var fadeSeconds) ? fadeSeconds : null));
                            }
                            break;
                        // UI
                        case HideMessageWindow:
                            commandList.Add(new CmdHideMessageWindow());
                            break;
                        case ShowMessageWindow:
                            commandList.Add(new CmdShowMessageWindow());
                            break;
                        case ChangeMessageWindow:
                            if (string.IsNullOrWhiteSpace(dto.Arg1)) throw new Exception($"{nameof(CmdChangeMessageWindow)} : Arg1 is required.");
                            commandList.Add(new CmdChangeMessageWindow(dto.Arg1));
                            break;
                        // Logic
                        case Param:
                            if (string.IsNullOrWhiteSpace(dto.Arg1)) throw new Exception($"{nameof(CmdParam)} : Arg1 is required.");
                            commandList.Add(new CmdParam(dto.Arg1));
                            break;
                        case Jump:
                            if (string.IsNullOrWhiteSpace(dto.Arg1)) throw new Exception($"{nameof(CmdJump)} : Arg1 are required.");
                            commandList.Add(new CmdJump(dto.Arg1, dto.Arg2));
                            break;
                        case SelectionTitle:
                            commandList.Add(new CmdSelectionTitle(dto.Text));
                            break;
                        case Selection:
                            {
                                if (string.IsNullOrWhiteSpace(dto.Arg1) || string.IsNullOrWhiteSpace(dto.Text)) throw new Exception($"{nameof(CmdSelection)} : Arg1,Text are required.");
                                commandList.Add(new CmdSelection(
                                    dto.Arg1,
                                    dto.Arg2,
                                    dto.Arg3,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg5, out var x) ? x : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Arg6, out var y) ? y : null,
                                    dto.Text));
                            }
                            break;
                        default:
                            if (dto.Command.StartsWith("*"))
                            {
                                commandList.Add(new CmdLabel(dto.Command, sheetName));
                                break;
                            }
                            throw new Exception($"Unsupported command : {dto.Command}");
                    }
                }
                DevLog.Log($"[{nameof(ScenarioCsvLoader)}] {nameof(Load)} done. commandList.Count={commandList.Count}");
                return commandList;
            }
            catch (Exception ex)
            {
                DevLog.LogError($"[{nameof(ScenarioCsvLoader)}] Invalid format : i={i}, dto=({dto})\n{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 自作コマンドに対応するにはこちらをoverride。<br/>Commandをnewして返すとリストに追加されます。dtoに対応するカスタムコマンドが見つからない場合はnullを返してください。
        /// </summary>
        /// <param name="dto">csvから読み取られた情報</param>
        /// <param name="lowerCmd">Command列をToLowerしたもの</param>
        /// <returns></returns>
        protected virtual ICommand? CustomCommand(ScenarioCsvDto? dto, string lowerCmd)
        {
            return null;
        }
    }
}
