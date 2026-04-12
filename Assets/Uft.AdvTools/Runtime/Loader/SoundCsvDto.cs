#nullable enable

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
                (csvHeaders) => MapperFactory(csvHeaders),
                64);
        }

        public static IReadOnlyList<SoundCsvDto> Load(string csvText)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return csvText.ReadCsv(
                config,
                (csvHeaders) => MapperFactory(csvHeaders),
                64);
        }

        public static CsvRowMapper<SoundCsvDto> MapperFactory(string[] csvHeaders)
        {
            var iLabel = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Label");
            var iTitle = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Title");
            var iType = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Type");
            var iFileName = CsvUtil.FindColumnIndexOrMinus1(csvHeaders,"FileName");
            var iIntroTime = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "IntroTime");

            var iVolume = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Volume");

            SoundCsvDto mapper(CsvRow csvRow)
            {
                return new SoundCsvDto
                {
                    Label = csvRow.GetString(iLabel),
                    Title = csvRow.GetString(iTitle),
                    Type = csvRow.GetString(iType),
                    FileName = csvRow.GetString(iFileName),
                    IntroTime = csvRow.GetString(iIntroTime),

                    Volume = csvRow.GetString(iVolume),
                };
            }
            return mapper;
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
