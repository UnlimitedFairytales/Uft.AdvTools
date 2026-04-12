#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Uft.UnityUtils.Csv;

namespace Uft.AdvTools.Loader
{
    public class CharacterCsvDto
    {
        static readonly PropertyInfo[] stringPropertyInfos =
        typeof(CharacterCsvDto)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(string))
            .ToArray();

        public static IReadOnlyList<CharacterCsvDto> Load(FileInfo fileInfo)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return fileInfo.ReadCsv(
                config,
                (csvHeaders) => MapperFactory(csvHeaders),
                64);
        }

        public static IReadOnlyList<CharacterCsvDto> Load(string csvText)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return csvText.ReadCsv(
                config,
                (csvHeaders) => MapperFactory(csvHeaders),
                64);
        }

        public static CsvRowMapper<CharacterCsvDto> MapperFactory(string[] csvHeaders)
        {
            var iCharacterName = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "CharacterName");
            var iNameText = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "NameText");
            var iPattern = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Pattern");
            var iX = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "X");
            var iY = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Y");
            var iZ = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Z");

            var iPivot = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Pivot");
            var iScale = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Scale");
            var iConditional = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Conditional");
            var iFileName = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "FileName");
            var iSubFileName = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "SubFileName");

            var iFileType = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "FileType");
            var iAnimationState = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "AnimationState");
            var iAnimation = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Animation");
            var iRenderTexture = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "RenderTexture");
            var iRenderRect = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "RenderRect");
            var iRenderTextureScale = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "RenderTextureScale");

            var iLoop = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Loop");
            CharacterCsvDto mapper(CsvRow csvRow)
            {
                return new CharacterCsvDto
                {
                    CharacterName = csvRow.GetString(iCharacterName),
                    NameText = csvRow.GetString(iNameText),
                    Pattern = csvRow.GetString(iPattern),
                    X = csvRow.GetString(iX),
                    Y = csvRow.GetString(iY),
                    Z = csvRow.GetString(iZ),

                    Pivot = csvRow.GetString(iPivot),
                    Scale = csvRow.GetString(iScale),
                    Conditional = csvRow.GetString(iConditional),
                    FileName = csvRow.GetString(iFileName),
                    SubFileName = csvRow.GetString(iSubFileName),

                    FileType = csvRow.GetString(iFileType),
                    AnimationState = csvRow.GetString(iAnimationState),
                    Animation = csvRow.GetString(iAnimation),
                    RenderTexture = csvRow.GetString(iRenderTexture),
                    RenderRect = csvRow.GetString(iRenderRect),
                    RenderTextureScale = csvRow.GetString(iRenderTextureScale),

                    Loop = csvRow.GetString(iLoop),
                };
            }
            return mapper;
        }

        public string? CharacterName { get; set; }
        public string? NameText { get; set; }
        public string? Pattern { get; set; }
        public string? X { get; set; }
        public string? Y { get; set; }
        public string? Z { get; set; }

        // Pivot0
        public string? Pivot { get; set; }
        public string? Scale { get; set; }
        public string? Conditional { get; set; }
        public string? FileName { get; set; }
        public string? SubFileName { get; set; }

        public string? FileType { get; set; }
        public string? AnimationState { get; set; }
        public string? Animation { get; set; }
        public string? RenderTexture { get; set; }
        public string? RenderRect { get; set; }
        public string? RenderTextureScale { get; set; }

        // EyeBlink
        // LipSynch
        // Icon
        // IconSubFileName
        // IconRect
        // IconAutoFlip
        public string? Loop { get; set; }

        public virtual bool IsAllNullOrWhiteSpace()
        {
            return stringPropertyInfos
                .All(p => string.IsNullOrWhiteSpace((string?)p.GetValue(this)!));
        }

        public override string ToString() =>
            $"{this.CharacterName},{this.NameText},{this.Pattern},{this.X},{this.Y},{this.Z},{this.Pivot},{this.Scale},{this.Conditional},{this.FileName},{this.SubFileName},{this.FileType},{this.AnimationState}...";
    }
}
