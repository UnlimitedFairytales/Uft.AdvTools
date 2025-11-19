#nullable enable

using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.IO;
using Uft.UnityUtils.Csv;

namespace Uft.AdvTools.Loader
{
    public class ScenarioCsvDto
    {
        public static IReadOnlyList<ScenarioCsvDto> Load(FileInfo fileInfo)
        {
            return fileInfo.ReadCsv(
                CsvUtil.GetCsvConfiguration(CsvUtil.UTF8),
                (reader) => Map(reader));
        }

        public static IReadOnlyList<ScenarioCsvDto> Load(string csvText)
        {
            return csvText.ReadCsv(
                CsvUtil.GetCsvConfiguration(CsvUtil.UTF8),
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

        [Name("Command")] public string? Command { get; set; }
        [Name("Arg1")] public string? Arg1 { get; set; }
        [Name("Arg2")] public string? Arg2 { get; set; }
        [Name("Arg3")] public string? Arg3 { get; set; }
        [Name("Arg4")] public string? Arg4 { get; set; }
        [Name("Arg5")] public string? Arg5 { get; set; }
        [Name("Arg6")] public string? Arg6 { get; set; }
        [Name("WaitType")] public string? WaitType { get; set; }
        [Name("Text")] public string? Text { get; set; }
        [Name("PageCtrl")] public string? PageCtrl { get; set; }
        [Name("Voice")] public string? Voice { get; set; }
        [Name("WindowType")] public string? WindowType { get; set; }

        public override string ToString() =>
            $"{this.Command},{this.Arg1},{this.Arg2},{this.Arg3},{this.Arg4},{this.Arg5},{this.Arg6},{this.WaitType},{this.Text},{this.PageCtrl},{this.Voice},{this.WindowType}";
    }
}
