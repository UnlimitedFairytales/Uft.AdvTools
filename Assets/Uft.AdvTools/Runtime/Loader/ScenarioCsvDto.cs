#nullable enable

using CsvHelper;
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
                (reader) => Map(reader));
        }

        public static IReadOnlyList<ScenarioCsvDto> Load(string csvText)
        {
            var config =  CsvUtil.GetCsvConfiguration(CsvUtil.UTF8);
            config.Comment = '/';
            config.MissingFieldFound = null;
            return csvText.ReadCsv(
                config,
                (reader) => Map(reader));
        }

        public static ScenarioCsvDto Map(CsvReader reader)
        {
            return new ScenarioCsvDto
            {
                Command = reader.GetField<string>("Command"),
                Arg1 = reader.GetField<string>("Arg1"),
                Arg2 = reader.GetField<string>("Arg2"),
                Arg3 = reader.GetField<string>("Arg3"),
                Arg4 = reader.GetField<string>("Arg4"),
                Arg5 = reader.GetField<string>("Arg5"),
                Arg6 = reader.GetField<string>("Arg6"),
                WaitType = reader.GetField<string>("WaitType"),
                Text = reader.GetField<string>("Text"),
                PageCtrl = reader.GetField<string>("PageCtrl"),
                Voice = reader.GetField<string>("Voice"),
                WindowType = reader.GetField<string>("WindowType"),
            };
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
