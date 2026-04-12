#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Uft.UnityUtils.Csv;

namespace Uft.AdvTools.Loader
{
    public class ScenarioCsvDto
    {
        static readonly PropertyInfo[] stringPropertyInfos =
        typeof(ScenarioCsvDto)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(string))
            .ToArray();

        public static IReadOnlyList<ScenarioCsvDto> Load(FileInfo fileInfo)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return fileInfo.ReadCsv(
                config,
                (csvHeaders) => MapperFactory(csvHeaders),
                256);
        }

        public static IReadOnlyList<ScenarioCsvDto> Load(string csvText)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return csvText.ReadCsv(
                config,
                (csvHeaders) => MapperFactory(csvHeaders),
                256);
        }

        public static CsvRowMapper<ScenarioCsvDto> MapperFactory(string[] csvHeaders)
        {
            var iCommand = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Command");
            var iArg1 = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Arg1");
            var iArg2 = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Arg2");
            var iArg3 = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Arg3");
            var iArg4 = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Arg4");
            var iArg5 = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Arg5");
            var iArg6 = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Arg6");
            var iWaitType = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "WaitType");
            var iText = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Text");
            var iPageCtrl = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "PageCtrl");
            var iVoice = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "Voice");
            var iWindowType = CsvUtil.FindColumnIndexOrMinus1(csvHeaders, "WindowType");
            ScenarioCsvDto mapper(CsvRow csvRow)
            {
                return new ScenarioCsvDto
                {
                    Command = csvRow.GetString(iCommand),
                    Arg1 = csvRow.GetString(iArg1),
                    Arg2 = csvRow.GetString(iArg2),
                    Arg3 = csvRow.GetString(iArg3),
                    Arg4 = csvRow.GetString(iArg4),
                    Arg5 = csvRow.GetString(iArg5),
                    Arg6 = csvRow.GetString(iArg6),
                    WaitType = csvRow.GetString(iWaitType),
                    Text = csvRow.GetString(iText),
                    PageCtrl = csvRow.GetString(iPageCtrl),
                    Voice = csvRow.GetString(iVoice),
                    WindowType = csvRow.GetString(iWindowType),
                };
            }
            return mapper;
        }

        public string? Command { get; set; }
        public string? Arg1 { get; set; }
        public string? Arg2 { get; set; }
        public string? Arg3 { get; set; }
        public string? Arg4 { get; set; }
        public string? Arg5 { get; set; }
        public string? Arg6 { get; set; }
        public string? WaitType { get; set; }
        public string? Text { get; set; }
        public string? PageCtrl { get; set; }
        public string? Voice { get; set; }
        public string? WindowType { get; set; }

        public virtual bool IsAllNullOrWhiteSpace()
        {
            return stringPropertyInfos
                .All(p => string.IsNullOrWhiteSpace((string?)p.GetValue(this)!));
        }

        public override string ToString() =>
            $"{this.Command},{this.Arg1},{this.Arg2},{this.Arg3},{this.Arg4},{this.Arg5},{this.Arg6},{this.WaitType},{this.Text},{this.PageCtrl},{this.Voice},{this.WindowType}";
    }
}
