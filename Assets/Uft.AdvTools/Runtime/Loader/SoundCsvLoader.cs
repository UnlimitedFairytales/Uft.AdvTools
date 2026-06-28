#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using Uft.UnityUtils;

namespace Uft.AdvTools.Loader
{
    public class SoundCsvLoader
    {
        public struct SoundDictionaries
        {
            public Dictionary<string, string> _bgmPathDict;
            public Dictionary<string, string> _sePathDict;
            public Dictionary<string, string> _voicePathDict;
        }

        public const string Bgm = "bgm";
        public const string Se = "se";
        public const string Voice = "voice";

        public SoundDictionaries Load(FileInfo fileInfo, string resourcesFolderPathPart)
        {
            var csvDtoList = SoundCsvDto.Load(fileInfo);
            return this.LoadInner(csvDtoList, resourcesFolderPathPart);
        }

        public SoundDictionaries Load(string csvText, string resourcesFolderPathPart)
        {
            var csvDtoList = SoundCsvDto.Load(csvText);
            return this.LoadInner(csvDtoList, resourcesFolderPathPart);
        }

        public SoundDictionaries Load(SoundTableSO so, string resourcesFolderPathPart)
        {
            var soundBgmRoot = resourcesFolderPathPart + "Sound/BGM/";
            var soundSeRoot = resourcesFolderPathPart + "Sound/SE/";
            var soundVoiceRoot = resourcesFolderPathPart + "Sound/Voice/";
            var bgmPathDict = new Dictionary<string, string>(so.entries.Count);
            var sePathDict = new Dictionary<string, string>(so.entries.Count);
            var voicePathDict = new Dictionary<string, string>(so.entries.Count);
            foreach (var data in so.entries)
            {
                if (string.IsNullOrWhiteSpace(data.label) || string.IsNullOrWhiteSpace(data.fileName)) continue;
                switch (data.type?.ToLower())
                {
                    case Bgm: bgmPathDict[data.label] = soundBgmRoot + data.fileName; break;
                    case Se: sePathDict[data.label] = soundSeRoot + data.fileName; break;
                    case Voice: voicePathDict[data.label] = soundVoiceRoot + data.fileName; break;
                }
            }
            DevLog.Log($"[{nameof(SoundCsvLoader)}] Load(SO) done. bgm={bgmPathDict.Count}, se={sePathDict.Count}, voice={voicePathDict.Count}");
            return new SoundDictionaries
            {
                _bgmPathDict = bgmPathDict,
                _sePathDict = sePathDict,
                _voicePathDict = voicePathDict,
            };
        }

        SoundDictionaries LoadInner(IReadOnlyList<SoundCsvDto> csvDtoList, string resourcesFolderPathPart)
        {
            var soundBgmRoot = resourcesFolderPathPart + "Sound/BGM/";
            var soundSeRoot = resourcesFolderPathPart + "Sound/SE/";
            var soundVoiceRoot = resourcesFolderPathPart + "Sound/Voice/";
            var bgmPathDict = new Dictionary<string, string>();
            var sePathDict = new Dictionary<string, string>();
            var voicePathDict = new Dictionary<string, string>();
            int i = 0;
            SoundCsvDto? dto = null;
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
                        case Bgm:
                            bgmPathDict[dto.Label] = soundBgmRoot + Path.ChangeExtension(dto.FileName, null);
                            break;
                        case Se:
                            sePathDict[dto.Label] = soundSeRoot + Path.ChangeExtension(dto.FileName, null);
                            break;
                        case Voice:
                            voicePathDict[dto.Label] = soundVoiceRoot + Path.ChangeExtension(dto.FileName, null);
                            break;
                        default:
                            break;
                    }
                }
                DevLog.Log($"[{nameof(SoundCsvLoader)}] {nameof(Load)} done. bgmPathDict.Count={bgmPathDict.Count}, sePathDict.Count={sePathDict.Count}, voicePathDict.Count={voicePathDict.Count}");
                return new SoundDictionaries()
                {
                    _bgmPathDict = bgmPathDict,
                    _sePathDict = sePathDict,
                    _voicePathDict = voicePathDict,
                };
            }
            catch (Exception ex)
            {
                DevLog.LogError($"[{nameof(SoundCsvLoader)}] Invalid format : i={i}, dto=({dto})\n{ex.Message}");
                throw;
            }
        }
    }
}
