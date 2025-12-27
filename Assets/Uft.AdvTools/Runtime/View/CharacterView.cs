#nullable enable

using System;
using Uft.AdvTools.Entities;
using UnityEngine;

namespace Uft.AdvTools.View
{
    public class CharacterView : OffsettableImage
    {
        // Status

        public Character? CastedKeyObject => this.KeyObject as Character;

        // Methods

        public override void Set<T>(bool isOn, T? keyObject, Sprite? sprite,
            Vector2 pivot, float scale, Vector2 position, Vector2 offset, float fromAlpha, float toAlpha, float fadeTime_sec)
            where T : class
        {
            if (keyObject is not Character castedKeyObject) throw new ArgumentException($"{nameof(keyObject)} must be a non-null {nameof(Character)}.", nameof(keyObject));
            base.Set(isOn, keyObject, sprite, pivot, scale, position, offset, fromAlpha, toAlpha, fadeTime_sec);
        }

        public override void Set<T>(bool isOn, T? keyObject, GameObject instantiated,
            Vector2 pivot, float scale, Vector2 position, Vector2 offset, float fromAlpha, float toAlpha, float fadeTime_sec)
            where T : class
        {
            if (keyObject is not Character castedKeyObject) throw new ArgumentException($"{nameof(keyObject)} must be a non-null {nameof(Character)}.", nameof(keyObject));
            base.Set(isOn, keyObject, instantiated, pivot, scale, position, offset, fromAlpha, toAlpha, fadeTime_sec);
        }
    }
}
