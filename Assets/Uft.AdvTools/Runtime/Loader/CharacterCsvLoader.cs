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
    public class CharacterCsvLoader
    {
        readonly AssetLoadProxy _assetLoadProxy;

        public CharacterCsvLoader(AssetLoadProxy assetLoadProxy)
        {
            this._assetLoadProxy = assetLoadProxy;
        }

        public Dictionary<string, Character> Load(FileInfo fileInfo, string resourcesFolderPathPart)
        {
            var csvDtoList = CharacterCsvDto.Load(fileInfo);
            return this.LoadInner(csvDtoList, resourcesFolderPathPart);
        }

        public Dictionary<string, Character> Load(string csvText, string resourcesFolderPathPart)
        {
            var csvDtoList = CharacterCsvDto.Load(csvText);
            return this.LoadInner(csvDtoList, resourcesFolderPathPart);
        }

        public Dictionary<string, Character> Load(CharacterTableSO so)
        {
            var characterDict = new Dictionary<string, Character>(so.characters.Count);
            foreach (var charData in so.characters)
            {
                if (string.IsNullOrWhiteSpace(charData.characterName)) continue;
                if (charData.details == null || charData.details.Count == 0) continue;
                if (!FileType.TryParse(charData.fileTypeValue, out var fileType)) continue;

                var defaultDetail = ToDetail(charData.details[0]);
                var character = new Character(charData.characterName, charData.nameText, fileType, charData.prefab, defaultDetail);
                for (int i = 1; i < charData.details.Count; i++)
                {
                    var detail = ToDetail(charData.details[i]);
                    character.CharacterDetailDictionary.Add(detail.Pattern, detail);
                }
                characterDict.Add(charData.characterName, character);
            }
            DevLog.Log($"[{nameof(CharacterCsvLoader)}] Load(SO) done. count={characterDict.Count}");
            return characterDict;
        }

        static CharacterDetail ToDetail(CharacterTableSO.DetailData d) =>
            new(d.pattern, d.x, d.y, d.z, d.pivot, d.scale, d.sprite,
                string.IsNullOrEmpty(d.animationState) ? null : d.animationState);

        Dictionary<string, Character> LoadInner(IReadOnlyList<CharacterCsvDto> csvDtoList, string resourcesFolderPathPart)
        {
            var textureCharacterRoot = resourcesFolderPathPart + "Texture/Character/";
            var characterDict = new Dictionary<string, Character>();
            Character? lastCharacter = null;
            int i = 0;
            CharacterCsvDto? dto = null;
            try
            {
                for (i = 0; i < csvDtoList.Count; i++)
                {
                    dto = csvDtoList[i];
                    if (dto.IsAllNullOrWhiteSpace()) continue;

                    var name = dto.CharacterName!;
                    if (!FileType.TryParse(dto.FileType, out FileType fileType)) throw new Exception($"{nameof(Character)} : FileType is unsupported value. : {dto.FileType}");

                    Sprite? sprite = null;
                    GameObject? prefab = null;
                    if (fileType == FileType.None)
                    {
                        sprite = this._assetLoadProxy.Load<Sprite>(textureCharacterRoot + Path.ChangeExtension(dto.FileName, null));
                    }
                    else if (fileType == FileType.Prefab2D || fileType == FileType.Prefab3D)
                    {
                        prefab = this._assetLoadProxy.Load<GameObject>(textureCharacterRoot + Path.ChangeExtension(dto.FileName, null));
                    }
                    var pattern = new CharacterDetail(
                        dto.Pattern!,
                        InvariantCultureUtil.FloatTryParse(dto.X, out var x) ? x : null,
                        InvariantCultureUtil.FloatTryParse(dto.Y, out var y) ? y : null,
                        InvariantCultureUtil.FloatTryParse(dto.Z, out var z) ? z : null,
                        AnchorPresetUtil.TryParseLooseAnchorPreset(dto.Pivot, out var pivot) ? pivot : null,
                        InvariantCultureUtil.FloatTryParse(dto.Scale, out var scale) ? scale : null,
                        sprite,
                        dto.AnimationState);

                    // 行のキャラクター切り替わり
                    if (lastCharacter == null || (!string.IsNullOrWhiteSpace(name) && lastCharacter.CharacterName != name))
                    {
                        if (characterDict.ContainsKey(name))
                        {
                            lastCharacter = characterDict[name];
                        }
                        else
                        {
                            lastCharacter = new Character(dto.CharacterName!, dto.NameText!, fileType, prefab, pattern);
                            characterDict.Add(name, lastCharacter);
                            continue;
                        }
                    }

                    // Detail追加
                    lastCharacter.CharacterDetailDictionary.Add(pattern.Pattern, pattern);
                }
                DevLog.Log($"[{nameof(CharacterCsvLoader)}] {nameof(Load)} done. characterDict.Count={characterDict.Count}");
                return characterDict;
            }
            catch (Exception ex)
            {
                DevLog.LogError($"[{nameof(CharacterCsvLoader)}] Invalid format : i={i}, dto=({dto})\n{ex.Message}");
                throw;
            }
        }
    }
}
