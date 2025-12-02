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

                    var pattern = new CharacterDetail(
                        dto.Pattern!,
                        InvariantCultureUtil.FloatTryParse(dto.X, out var x) ? x : null,
                        InvariantCultureUtil.FloatTryParse(dto.Y, out var y) ? y : null,
                        AnchorPresetUtil.TryParseLooseAnchorPreset(dto.Pivot, out var pivot) ? pivot : null,
                        InvariantCultureUtil.FloatTryParse(dto.Scale, out var scale) ? scale : null,
                        Resources.Load<Sprite>(textureCharacterRoot + Path.ChangeExtension(dto.FileName, null)));

                    // 行のキャラクター切り替わり
                    if (lastCharacter == null || (!string.IsNullOrWhiteSpace(name) && lastCharacter.CharacterName != name))
                    {
                        if (characterDict.ContainsKey(name))
                        {
                            lastCharacter = characterDict[name];
                        }
                        else
                        {
                            lastCharacter = new Character(dto.CharacterName!, dto.NameText!, pattern);
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
