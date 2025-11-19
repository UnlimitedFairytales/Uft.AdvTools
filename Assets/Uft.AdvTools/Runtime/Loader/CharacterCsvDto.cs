#nullable enable

using CsvHelper;
using System.Collections.Generic;
using System.IO;
using Uft.UnityUtils.Csv;

namespace Uft.AdvTools.Loader
{
    public class CharacterCsvDto
    {
        public static IReadOnlyList<CharacterCsvDto> Load(FileInfo fileInfo)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.MissingFieldFound = null;
            return fileInfo.ReadCsv(
                config,
                (reader) => Map(reader));
        }

        public static IReadOnlyList<CharacterCsvDto> Load(string csvText)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.MissingFieldFound = null;
            return csvText.ReadCsv(
                config,
                (reader) => Map(reader));
        }

        public static CharacterCsvDto Map(CsvReader reader)
        {
            return new CharacterCsvDto
            {
                CharacterName = reader.GetField<string>("CharacterName"),
                NameText = reader.GetField<string>("NameText"),
                Pattern = reader.GetField<string>("Pattern"),
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

                Loop = reader.GetField<string>("Loop"),
            };
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
        // AnimationState
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

        public override string ToString() =>
            $"{this.CharacterName},{this.NameText},{this.Pattern},{this.X},{this.Y},{this.Z},{this.Pivot},{this.Scale},{this.Conditional},{this.FileName},{this.SubFileName},{this.FileType},...";
    }
}
