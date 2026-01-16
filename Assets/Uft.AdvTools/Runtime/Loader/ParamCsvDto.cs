#nullable enable

using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Uft.UnityUtils.Csv;

namespace Uft.AdvTools.Loader
{
    public class ParamCsvDto
    {
        static readonly PropertyInfo[] stringPropertyInfos =
        typeof(ParamCsvDto)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(string))
            .ToArray();

        public static IReadOnlyList<ParamCsvDto> Load(FileInfo fileInfo)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return fileInfo.ReadCsv(
                config,
                (reader) => Map(reader));
        }

        public static IReadOnlyList<ParamCsvDto> Load(string csvText)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return csvText.ReadCsv(
                config,
                (reader) => Map(reader));
        }

        public static ParamCsvDto Map(CsvReader reader)
        {
            return new ParamCsvDto
            {
                Label = reader.GetField<string>("Label"),
                Type = reader.GetField<string>("Type"),
                Value = reader.GetField<string>("Value"),
                FileType = reader.GetField<string>("FileType"),
            };
        }

        public string? Label { get; set; }
        public string? Type { get; set; }
        public string? Value { get; set; }
        public string? FileType { get; set; }

        public virtual bool IsAllNullOrWhiteSpace()
        {
            return stringPropertyInfos
                .All(p => string.IsNullOrWhiteSpace((string?)p.GetValue(this)!));
        }

        public override string ToString() =>
            $"{this.Label},{this.Type},{this.Value},{this.FileType}";
    }
}
