#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using Uft.AdvTools.Entities;
using Uft.UnityUtils;

namespace Uft.AdvTools.Loader
{
    public class ParamCsvLoader
    {
        public Dictionary<string, Param> Load(FileInfo fileInfo)
        {
            var csvDtoList = ParamCsvDto.Load(fileInfo);
            return this.LoadInner(csvDtoList);
        }

        public Dictionary<string, Param> Load(string csvText)
        {
            var csvDtoList = ParamCsvDto.Load(csvText);
            return this.LoadInner(csvDtoList);
        }

        public Dictionary<string, Param> Load(ParamTableSO so)
        {
            var paramDict = new Dictionary<string, Param>(so.entries.Count);
            foreach (var data in so.entries)
            {
                if (string.IsNullOrWhiteSpace(data.label)) continue;
                paramDict[data.label] = new Param(data.label, data.value);
            }
            DevLog.Log($"[{nameof(ParamCsvLoader)}] Load(SO) done. count={paramDict.Count}");
            return paramDict;
        }

        Dictionary<string, Param> LoadInner(IReadOnlyList<ParamCsvDto> csvDtoList)
        {
            var paramDict = new Dictionary<string, Param>();
            int i = 0;
            ParamCsvDto? dto = null;
            try
            {
                for (i = 0; i < csvDtoList.Count; i++)
                {
                    dto = csvDtoList[i];
                    if (dto.IsAllNullOrWhiteSpace()) continue;
                    if (string.IsNullOrWhiteSpace(dto.Label)) continue;

                    paramDict[dto.Label] = new Param(
                        dto.Label,
                        int.TryParse(dto.Value, out var value) ? value : null);
                }
                DevLog.Log($"[{nameof(ParamCsvLoader)}] {nameof(Load)} done. paramDict.Count={paramDict.Count}");
                return paramDict;
            }
            catch (Exception ex)
            {
                DevLog.LogError($"[{nameof(ParamCsvLoader)}] Invalid format : i={i}, dto=({dto})\n{ex.Message}");
                throw;
            }
        }
    }
}
