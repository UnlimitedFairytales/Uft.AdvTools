#nullable enable

using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Uft.UnityUtils.Csv;

namespace Uft.AdvTools.Loader
{
    public class SoundCsvDto
    {
        static readonly PropertyInfo[] stringPropertyInfos =
        typeof(SoundCsvDto)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(string))
            .ToArray();

        public static IReadOnlyList<SoundCsvDto> Load(FileInfo fileInfo)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return fileInfo.ReadCsv(
                config,
                (reader) => Map(reader));
        }

        public static IReadOnlyList<SoundCsvDto> Load(string csvText)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return csvText.ReadCsv(
                config,
                (reader) => Map(reader));
        }

        public static SoundCsvDto Map(CsvReader reader)
        {
            return new SoundCsvDto
            {
                Label = reader.GetField<string>("Label"),
                Title = reader.GetField<string>("Title"),
                Type = reader.GetField<string>("Type"),
                FileName = reader.GetField<string>("FileName"),
                IntroTime = reader.GetField<string>("IntroTime"),

                Volume = reader.GetField<string>("Volume"),
            };
        }

        public string? Label { get; set; }
        public string? Title { get; set; }
        public string? Type { get; set; }
        public string? FileName { get; set; }
        public string? IntroTime { get; set; }

        public string? Volume { get; set; }

        public virtual bool IsAllNullOrWhiteSpace()
        {
            return stringPropertyInfos
                .All(p => string.IsNullOrWhiteSpace((string?)p.GetValue(this)!));
        }

        public override string ToString() =>
            $"{this.Label},{this.Title},{this.Type},{this.FileName},{this.IntroTime},{this.Volume}";
    }
}
