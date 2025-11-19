#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using Uft.UnityUtils;
using UnityEngine;

namespace Uft.AdvTools.Loader
{
    public class SoundCsvLoader
    {
        public struct SoundDictionaries
        {
            public Dictionary<string, AudioClip> _bgmDict;
            public Dictionary<string, AudioClip> _seDict;
            public Dictionary<string, AudioClip> _voiceDict;
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

        SoundDictionaries LoadInner(IReadOnlyList<SoundCsvDto> csvDtoList, string resourcesFolderPathPart)
        {
            var soundBgmRoot = resourcesFolderPathPart + "Sound/BGM/";
            var soundSeRoot = resourcesFolderPathPart + "Sound/SE/";
            var soundVoiceRoot = resourcesFolderPathPart + "Sound/Voice/";
            var bgmDict = new Dictionary<string, AudioClip>();
            var seDict = new Dictionary<string, AudioClip>();
            var voiceDict = new Dictionary<string, AudioClip>();
            int i = 0;
            SoundCsvDto? dto = null;
            try
            {
                for (i = 0; i < csvDtoList.Count; i++)
                {
                    dto = csvDtoList[i];
                    if (string.IsNullOrWhiteSpace(dto.Label)) continue;

                    var type = dto.Type!.ToLower();
                    switch (type)
                    {
                        case Bgm:
                            bgmDict[dto.Label] = Resources.Load<AudioClip>(soundBgmRoot + Path.ChangeExtension(dto.FileName, null));
                            var bgmClip = bgmDict[dto.Label];
                            if (bgmClip != null && bgmClip.length >= 10 && bgmClip.loadType != AudioClipLoadType.Streaming)
                            {
                                DevLog.LogWarning($"[{nameof(SoundCsvLoader)}] Bgm is not streaming (>=10sec) : length={bgmClip.length:0}, loadType={bgmClip.loadType}, dto=({dto})");
                            }
                            break;
                        case Se:
                            seDict[dto.Label] = Resources.Load<AudioClip>(soundSeRoot + Path.ChangeExtension(dto.FileName, null));
                            break;
                        case Voice:
                            voiceDict[dto.Label] = Resources.Load<AudioClip>(soundVoiceRoot + Path.ChangeExtension(dto.FileName, null));
                            break;
                        default:
                            break;
                    }
                }
                DevLog.Log($"[{nameof(SoundCsvLoader)}] {nameof(Load)} done. bgmDict.Count={bgmDict.Count}, seDict.Count={seDict.Count}, voiceDict.Count={voiceDict.Count}");
                return new SoundDictionaries()
                {
                    _bgmDict = bgmDict,
                    _seDict = seDict,
                    _voiceDict = voiceDict,
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
