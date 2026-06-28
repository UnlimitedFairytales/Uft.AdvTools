#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using Uft.AdvTools.Entities;
using Uft.UnityUtils;
using Uft.UnityUtils.Asset;
using Uft.UnityUtils.Common;
using Uft.UnityUtils.UI;

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

        readonly AssetLoadProxy _assetLoadProxy;

        public TextureCsvLoader(AssetLoadProxy assetLoadProxy)
        {
            this._assetLoadProxy = assetLoadProxy;
        }

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

        public TextureDictionaries Load(TextureTableSO so, string resourcesFolderPathPart)
        {
            var textureBgRoot = resourcesFolderPathPart + "Texture/BG/";
            var textureSpriteRoot = resourcesFolderPathPart + "Texture/Sprite/";
            var bgDict = new Dictionary<string, TextureRow>(so.entries.Count);
            var spriteDict = new Dictionary<string, TextureRow>(so.entries.Count);
            foreach (var data in so.entries)
            {
                if (string.IsNullOrWhiteSpace(data.label) || string.IsNullOrWhiteSpace(data.fileName)) continue;
                if (!FileType.TryParse(data.fileType, out FileType fileType)) continue;
                var root = data.type.ToLower() == Bg ? textureBgRoot : textureSpriteRoot;
                var row = new TextureRow(data.label, data.type, data.x, data.y, data.pivot, data.scale, this._assetLoadProxy, root + data.fileName, fileType);
                switch (data.type.ToLower())
                {
                    case Bg: bgDict[data.label] = row; break;
                    case Sprite: spriteDict[data.label] = row; break;
                }
            }
            DevLog.Log($"[{nameof(TextureCsvLoader)}] Load(SO) done. bg={bgDict.Count}, sprite={spriteDict.Count}");
            return new TextureDictionaries { _bgDict = bgDict, _spriteDict = spriteDict };
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
                    if (!FileType.TryParse(dto.FileType, out FileType fileType)) throw new Exception($"{nameof(TextureRow)} : FileType is unsupported value. : {dto.FileType}");

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
                                    this._assetLoadProxy,
                                    textureBgRoot + Path.ChangeExtension(dto.FileName, null),
                                    fileType);
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
                                    this._assetLoadProxy,
                                    textureSpriteRoot + Path.ChangeExtension(dto.FileName, null),
                                    fileType);
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
