#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Uft.UnityUtils.Csv;

namespace Uft.AdvTools.Loader
{
    public class TextureCsvDto
    {
        static readonly PropertyInfo[] stringPropertyInfos =
        typeof(TextureCsvDto)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(string))
            .ToArray();

        public static IReadOnlyList<TextureCsvDto> Load(FileInfo fileInfo)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return fileInfo.ReadCsv(
                config,
                (csvHeaders) => MapperFactory(csvHeaders),
                64);
        }

        public static IReadOnlyList<TextureCsvDto> Load(string csvText)
        {
            var config = CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return csvText.ReadCsv(
                config,
                (csvHeaders) => MapperFactory(csvHeaders),
                64);
        }

        public static CsvRowMapper<TextureCsvDto> MapperFactory(string[] csvHeaders)
        {
            var iLabel = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Label");
            var iType = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Type");
            var iX = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "X");
            var iY = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Y");
            var iZ = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Z");

            var iPivot = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Pivot");
            var iScale = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Scale");
            var iConditional = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Conditional");
            var iFileName = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "FileName");
            var iSubFileName = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "SubFileName");

            var iFileType = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "FileType");
            var iAnimation = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Animation");
            var iRenderTexture = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "RenderTexture");
            var iRenderRect = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "RenderRect");
            var iRenderTextureScale = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "RenderTextureScale");

            var iThumbnail = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Thumbnail");
            var iCgCategolly = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "CgCategolly");
            var iLoop = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Loop");
            TextureCsvDto mapper(CsvRow csvRow)
            {
                return new TextureCsvDto
                {
                    Label = csvRow.GetString(iLabel),
                    Type = csvRow.GetString(iType),
                    X = csvRow.GetString(iX),
                    Y = csvRow.GetString(iY),
                    Z = csvRow.GetString(iZ),

                    Pivot = csvRow.GetString(iPivot),
                    Scale = csvRow.GetString(iScale),
                    Conditional = csvRow.GetString(iConditional),
                    FileName = csvRow.GetString(iFileName),
                    SubFileName = csvRow.GetString(iSubFileName),

                    FileType = csvRow.GetString(iFileType),
                    Animation = csvRow.GetString(iAnimation),
                    RenderTexture = csvRow.GetString(iRenderTexture),
                    RenderRect = csvRow.GetString(iRenderRect),
                    RenderTextureScale = csvRow.GetString(iRenderTextureScale),

                    Thumbnail = csvRow.GetString(iThumbnail),
                    CgCategolly = csvRow.GetString(iCgCategolly),
                    Loop = csvRow.GetString(iLoop),
                };
            }
            return mapper;
        }

        public string? Label { get; set; }
        public string? Type { get; set; }
        public string? X { get; set; }
        public string? Y { get; set; }
        public string? Z { get; set; }

        public string? Pivot { get; set; }
        public string? Scale { get; set; }
        public string? Conditional { get; set; }
        public string? FileName { get; set; }
        public string? SubFileName { get; set; }

        public string? FileType { get; set; }
        public string? Animation { get; set; }
        public string? RenderTexture { get; set; }
        public string? RenderRect { get; set; }
        public string? RenderTextureScale { get; set; }

        public string? Thumbnail { get; set; }
        public string? CgCategolly { get; set; }
        public string? Loop { get; set; }

        public virtual bool IsAllNullOrWhiteSpace()
        {
            return stringPropertyInfos
                .All(p => string.IsNullOrWhiteSpace((string?)p.GetValue(this)!));
        }

        public override string ToString() =>
            $"{this.Label},{this.Type},{this.X},{this.Y},{this.Z},{this.Pivot},{this.Scale},{this.Conditional},{this.FileName},{this.SubFileName},{this.FileType},...";
    }
}
