#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using Uft.AdvTools.Entities;
using Uft.UnityUtils;
using Uft.UnityUtils.Common;
using Uft.UnityUtils.UI;
using UnityEngine;

namespace Uft.AdvTools.Loader
{
    public class TextureCsvLoader
    {
        public struct TextureDictionaries
        {
            public Dictionary<string, TextureRow> _bgDict;
            public Dictionary<string, TextureRow> _spriteDict;
        }

        public const string Bg = "bg";
        public const string Sprite = "sprite";

        public TextureDictionaries Load(FileInfo fileInfo, string resourcesFolderPathPart)
        {
            var csvDtoList = TextureCsvDto.Load(fileInfo);
            return this.LoadInner(csvDtoList, resourcesFolderPathPart);
        }

        public TextureDictionaries Load(string csvText, string resourcesFolderPathPart)
        {
            var csvDtoList = TextureCsvDto.Load(csvText);
            return this.LoadInner(csvDtoList, resourcesFolderPathPart);
        }

        TextureDictionaries LoadInner(IReadOnlyList<TextureCsvDto> csvDtoList, string resourcesFolderPathPart)
        {
            var textureBgRoot = resourcesFolderPathPart + "Texture/BG/";
            var textureSpriteRoot = resourcesFolderPathPart + "Texture/Sprite/";
            var bgDict = new Dictionary<string, TextureRow>();
            var spriteDict = new Dictionary<string, TextureRow>();
            int i = 0;
            TextureCsvDto? dto = null;
            try
            {
                for (i = 0; i < csvDtoList.Count; i++)
                {
                    dto = csvDtoList[i];
                    if (dto.IsAllNullOrWhiteSpace()) continue;
                    if (string.IsNullOrWhiteSpace(dto.Label)) continue;

                    var type = dto.Type!.ToLower();
                    switch (type)
                    {
                        case Bg:
                            {
                                bgDict[dto.Label] = new TextureRow(
                                    dto.Label,
                                    dto.Type,
                                    InvariantCultureUtil.FloatTryParse(dto.X, out var x) ? x : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Y, out var y) ? y : null,
                                    AnchorPresetUtil.TryParseLooseAnchorPreset(dto.Pivot, out var pivot) ? pivot : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Scale, out var scale) ? scale : null,
                                    Resources.Load<Sprite>(textureBgRoot + Path.ChangeExtension(dto.FileName, null)));
                            }
                            break;
                        case Sprite:
                            {
                                spriteDict[dto.Label] = new TextureRow(
                                    dto.Label,
                                    dto.Type,
                                    InvariantCultureUtil.FloatTryParse(dto.X, out var x) ? x : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Y, out var y) ? y : null,
                                    AnchorPresetUtil.TryParseLooseAnchorPreset(dto.Pivot, out var pivot) ? pivot : null,
                                    InvariantCultureUtil.FloatTryParse(dto.Scale, out var scale) ? scale : null,
                                    Resources.Load<Sprite>(textureSpriteRoot + Path.ChangeExtension(dto.FileName, null)));
                            }
                            break;
                        default:
                            break;
                    }
                }
                DevLog.Log($"[{nameof(TextureCsvLoader)}] {nameof(Load)} done. bgDict.Count={bgDict.Count}, spriteDict.Count={spriteDict.Count}");
                return new TextureDictionaries()
                {
                    _bgDict = bgDict,
                    _spriteDict = spriteDict,
                };
            }
            catch (Exception ex)
            {
                DevLog.LogError($"[{nameof(TextureCsvLoader)}] Invalid format : i={i}, dto=({dto})\n{ex.Message}");
                throw;
            }
        }
    }
}
