#nullable enable

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
                (csvHeaders) => MapperFactory(csvHeaders),
                64);
        }

        public static IReadOnlyList<ParamCsvDto> Load(string csvText)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return csvText.ReadCsv(
                config,
                (csvHeaders) => MapperFactory(csvHeaders),
                64);
        }

        public static CsvRowMapper<ParamCsvDto> MapperFactory(string[] csvHeaders)
        {
            var iLabel = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Label");
            var iType = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Type");
            var iValue = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Value");
            var iFileType = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "FileType");
            ParamCsvDto mapper(CsvRow csvRow)
            {
                return new ParamCsvDto()
                {
                    Label = csvRow.GetString(iLabel),
                    Type = csvRow.GetString(iType),
                    Value = csvRow.GetString(iValue),
                    FileType = csvRow.GetString(iFileType)
                };
            }
            return mapper;
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
