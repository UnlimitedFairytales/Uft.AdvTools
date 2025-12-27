#nullable enable

using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Uft.UnityUtils;
using Uft.UnityUtils.Common;
using UnityEngine;

namespace Uft.AdvTools.Commands
{
    public class CmdTween : ICommand
    {
        public enum TweenType
        {
            MoveTo,
            MoveFrom,
            MoveBy,
            PunchPosition,
            ShakePosition,
            PunchScale,
            ShakeScale,
        }

        public struct TweenParameter
        {
            /// <summary>islocal=trueが指定可能な場合は指定必須(グローバルが許容されない)、色関連は非対応、=の左右に空白は許容されない制限あり</summary>
            public static bool TryParse(TweenType tweenType, string? s, out TweenParameter tweenParameter)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    tweenParameter = default;
                    return false;
                }
                if (!s.Contains("islocal=true") && (
                    tweenType == TweenType.MoveTo || tweenType == TweenType.MoveFrom || tweenType == TweenType.ShakePosition))
                {
                    tweenParameter = default;
                    return false;
                }

                try
                {
                    tweenParameter = new TweenParameter();
                    var expressions = Regex.Split(s, @"\s+");
                    for (int i = 0; i < expressions.Length; i++)
                    {
                        var lhsRhs = expressions[i].Split('=');
                        var errMsg = $"invalid expression : {expressions[i]}";
                        if (lhsRhs.Length != 2 || string.IsNullOrWhiteSpace(lhsRhs[0]) || string.IsNullOrWhiteSpace(lhsRhs[1])) throw new Exception(errMsg);
                        switch (lhsRhs[0])
                        {
                            case "time":
                                tweenParameter.time = InvariantCultureUtil.FloatTryParse(lhsRhs[1], out float time) ? time : throw new Exception(errMsg);
                                break;
                            case "speed":
                                tweenParameter.speed = InvariantCultureUtil.FloatTryParse(lhsRhs[1], out float speed) ? speed : throw new Exception(errMsg);
                                break;
                            case "delay":
                                tweenParameter.delay = InvariantCultureUtil.FloatTryParse(lhsRhs[1], out float delay) ? delay : throw new Exception(errMsg);
                                break;
                            case "x":
                                tweenParameter.x = InvariantCultureUtil.FloatTryParse(lhsRhs[1], out float x) ? x : throw new Exception(errMsg);
                                break;
                            case "y":
                                tweenParameter.y = InvariantCultureUtil.FloatTryParse(lhsRhs[1], out float y) ? y : throw new Exception(errMsg);
                                break;
                            case "z":
                                tweenParameter.z = InvariantCultureUtil.FloatTryParse(lhsRhs[1], out float z) ? z : throw new Exception(errMsg);
                                break;
                            case "islocal":
                                tweenParameter.islocal = bool.TryParse(lhsRhs[1], out bool islocal) ? islocal : throw new Exception(errMsg);
                                break;
                            default:
                                break;
                        }
                    }
                    if (tweenParameter.time == null && tweenParameter.speed == null) throw new Exception("Either time or speed is required.");
                    if (tweenType == TweenType.PunchPosition && tweenParameter.time == null) throw new Exception($"{nameof(TweenType.PunchPosition)} requires time.");
                    if (tweenType == TweenType.ShakePosition && tweenParameter.time == null) throw new Exception($"{nameof(TweenType.ShakePosition)} requires time.");
                    if (tweenType == TweenType.PunchScale && tweenParameter.time == null) throw new Exception($"{nameof(TweenType.PunchScale)} requires time.");
                    if (tweenType == TweenType.ShakeScale && tweenParameter.time == null) throw new Exception($"{nameof(TweenType.ShakeScale)} requires time.");
                    return true;
                }
                catch (Exception ex)
                {
                    DevLog.LogWarning($"[{nameof(TweenParameter)}] {ex.Message}");
                    tweenParameter = default;
                    return false;
                }
            }

            public float? time;
            public float? speed;
            public float delay;
            public float? x;
            public float? y;
            public float? z;
            public bool? islocal;

            public readonly bool IsSpeed => this.speed != null;
        }

        public static bool TweenTypeTryParse(string? s, out TweenType tween)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                tween = default;
                return false;
            }
            return Enum.TryParse(s, true, out tween);
        }

        /// <summary>Springに非対応</summary>
        public static bool EaseTryParse(string? s, out Ease ease)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                ease = default;
                return false;
            }
            if (s.StartsWith("ease", true, CultureInfo.InvariantCulture))
            {
                s = s[4..];
            }
            return Enum.TryParse(s, true, out ease);
        }

        public CommandCategory CommandCategory { get; } = CommandCategory.Effect;

        protected string TargetName { get; set; }
        protected TweenType Tween { get; set; }
        protected TweenParameter Parameter { get; set; }
        protected Ease? Ease { get; set; }

        /// <summary>背景を動かす場合は"BG"を指定</summary>
        public CmdTween(string targetName, TweenType tween, TweenParameter parameter, Ease? ease)
        {
            this.TargetName = targetName;
            this.Tween = tween;
            this.Parameter = parameter;
            this.Ease = ease;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            scenarioExecutor.IsWaiting = true;
            UniTask.Void(async () =>
            {
                try
                {
                    var rt =
                        this.TargetName == CmdBg.SPRITE_NAME_BG ? advRoot.Bg.GetBgOi().RootRectTransform :
                        advRoot.CharacterDictionary.ContainsKey(this.TargetName) ? advRoot.SpriteManager.GetCharacterView(advRoot.CharacterDictionary[this.TargetName])!.RootRectTransform :
                        advRoot.SpriteDictionary.ContainsKey(this.TargetName) ? advRoot.SpriteManager.GetSpriteOi(advRoot.SpriteDictionary[this.TargetName].Sprite)!.RootRectTransform :
                        null;
                    if (rt != null)
                    {
                        await TweenAsync(rt, this.Tween, this.Parameter, this.Ease);
                    }
                }
                finally
                {
                    scenarioExecutor.IsWaiting = false;
                }
            });
        }

        static async UniTask TweenAsync(RectTransform rt, TweenType tweenType, TweenParameter parameter, Ease? ease)
        {
            ease ??= DG.Tweening.Ease.OutQuad;
            switch (tweenType)
            {
                case TweenType.MoveTo:
                case TweenType.MoveFrom:
                    {
                        var tweener = rt
                            .DOAnchorPos(
                                new Vector2(parameter.x ?? rt.anchoredPosition.x, parameter.y ?? rt.anchoredPosition.y),
                                parameter.IsSpeed ? parameter.speed!.Value : parameter.time!.Value);
                        if (tweenType == TweenType.MoveFrom)
                        {
                            tweener = tweener.From();
                        }
                        await tweener
                            .SetSpeedBased(parameter.IsSpeed)
                            .SetDelay(parameter.delay)
                            .SetEase(ease!.Value);
                    }
                    break;
                case TweenType.MoveBy:
                    {
                        await rt
                            .DOAnchorPos(
                                new Vector2(parameter.x ?? 0, parameter.y ?? 0),
                                parameter.IsSpeed ? parameter.speed!.Value : parameter.time!.Value)
                            .SetSpeedBased(parameter.IsSpeed)
                            .SetDelay(parameter.delay)
                            .SetEase(ease!.Value)
                            .SetRelative();
                    }
                    break;
                case TweenType.PunchPosition:
                    {
                        await rt
                            .DOPunchAnchorPos(
                                new Vector2(parameter.x ?? 0, parameter.y ?? 0),
                                parameter.time!.Value)
                            .SetDelay(parameter.delay)
                            .SetEase(ease!.Value);
                    }
                    break;
                case TweenType.ShakePosition:
                    {
                        await rt
                            .DOShakeAnchorPos(
                                parameter.time!.Value,
                                new Vector3(parameter.x ?? 0, parameter.y ?? 0, parameter.z ?? 0),
                                20, 90, false, false)
                            .SetDelay(parameter.delay)
                            .SetEase(ease!.Value);
                    }
                    break;
                case TweenType.PunchScale:
                    await rt
                        .DOPunchScale(
                            new Vector3(parameter.x ?? 0, parameter.y ?? 0, 0),
                            parameter.time!.Value)
                        .SetDelay(parameter.delay)
                        .SetEase(ease!.Value);
                    break;
                case TweenType.ShakeScale:
                    await rt
                        .DOShakeScale(parameter.time!.Value,
                             new Vector3(parameter.x ?? 0, parameter.y ?? 0, 0),
                             20, 90, false)
                        .SetDelay(parameter.delay)
                        .SetEase(ease!.Value);
                    break;
                default:
                    break;
            }
        }
    }
}
