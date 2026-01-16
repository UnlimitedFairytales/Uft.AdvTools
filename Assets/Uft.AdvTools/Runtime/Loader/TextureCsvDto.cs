#nullable enable

using CsvHelper;
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
                (reader) => Map(reader));
        }

        public static IReadOnlyList<TextureCsvDto> Load(string csvText)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return csvText.ReadCsv(
                config,
                (reader) => Map(reader));
        }

        public static TextureCsvDto Map(CsvReader reader)
        {
            return new TextureCsvDto
            {
                Label = reader.GetField<string>("Label"),
                Type = reader.GetField<string>("Type"),
                X = reader.GetField<string>("X"),
                Y = reader.GetField<string>("Y"),
                Z = reader.GetField<string>("Z"),

                Pivot = reader.GetField<string>("Pivot"),
                Scale = reader.GetField<string>("Scale"),
                Conditional = reader.GetField<string>("Conditional"),
                FileName = reader.GetField<string>("FileName"),
                SubFileName = reader.GetField<string>("SubFileName"),

                FileType = reader.GetField<string>("FileType"),
                Animation = reader.GetField<string>("Animation"),
                RenderTexture = reader.GetField<string>("RenderTexture"),
                RenderRect = reader.GetField<string>("RenderRect"),
                RenderTextureScale = reader.GetField<string>("RenderTextureScale"),

                Thumbnail = reader.GetField<string>("Thumbnail"),
                CgCategolly = reader.GetField<string>("CgCategolly"),
                Loop = reader.GetField<string>("Loop"),
            };
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
