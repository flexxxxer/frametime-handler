using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia.Media;
using FrameTimeHandler.FTAnlzerInterop;
using Microsoft.Extensions.Configuration.Ini;

namespace FrameTimeHandler.GraphsExport
{
    public static class GraphExporter
    {
        public class ExportData
        {
            public string OutputFile { get; set; }
            public string TestName { get; set; }
            public bool Append { get; set; }
            public (string graphName, string graphData, Color color) Graph { get; set; }
        }

        private static Dictionary<ProgramsThatReadOutput, Action<ExportData>> Savers =
            new Dictionary<ProgramsThatReadOutput, Action<ExportData>>()
            {
                {
                    ProgramsThatReadOutput.Graph, data =>
                    {
                        IDictionary<string, string> iniFile = new Dictionary<string, string>();

                        if (data.Append)
                        {
                            using FileStream fileStream = File.Open(data.OutputFile, FileMode.OpenOrCreate);
                            iniFile = IniStreamConfigurationProvider.Read(fileStream);
                        }

                        if (iniFile.Count == 0)
                        {
                            iniFile.Add("Graph:Version", "4.4.2.543");
                            iniFile.Add("Graph:MinVersion", "2.5");
                            iniFile.Add("Graph:OS", "Windows NT 6.2");

                            iniFile.Add("Axes:xMin", "-10");
                            iniFile.Add("Axes:xMax", "10");
                            iniFile.Add("Axes:xTickUnit", "1");
                            iniFile.Add("Axes:xGridUnit", "1");
                            iniFile.Add("Axes:yMin", "-10");
                            iniFile.Add("Axes:yMax", "10");
                            iniFile.Add("Axes:yTickUnit", "2");
                            iniFile.Add("Axes:yGridUnit", "2");
                            iniFile.Add("Axes:AxesColor", "clBlue");
                            iniFile.Add("Axes:GridColor", "0x00FF9999");
                            iniFile.Add("Axes:ShowLegend", "1");
                            iniFile.Add("Axes:Radian", "1");

                            iniFile.Add("Data:TextLabelCount", "0");
                            iniFile.Add("Data:FuncCount", "0");
                            iniFile.Add("Data:PointSeriesCount", "0");
                            iniFile.Add("Data:ShadeCount", "0");
                            iniFile.Add("Data:RelationCount", "0");
                            iniFile.Add("Data:OleObjectCount", "0");
                        }

                        var (graphName, graphData, color) = data.Graph;

                        string nextPointSeries = iniFile.Keys
                            .Select(x => x.Split(':')[0])
                            .Where(x => x.StartsWith("PointSeries"))
                            .Select(x => x.Replace("PointSeries", ""))
                            .Select(s => int.Parse(s))
                            .OrderByDescending(v => v)
                            .FirstOrDefault()
                            .ToString();

                        if (nextPointSeries == "0")
                        {
                            nextPointSeries = "PointSeries1";
                        }
                        else
                        {
                            nextPointSeries = "PointSeries" + (int.Parse(nextPointSeries) + 1);
                        }

                        Color withoutAlpha = Color.FromArgb(Byte.MinValue, color.R, color.G, color.B);

                        iniFile[$"{nextPointSeries}:FillColor"] = withoutAlpha.ToString().ToUpper().Replace("#", "0x");
                        iniFile[$"{nextPointSeries}:LineColor"] = withoutAlpha.ToString().ToUpper().Replace("#", "0x");
                        iniFile[$"{nextPointSeries}:Size"] = "0";
                        iniFile[$"{nextPointSeries}:Style"] = "0";
                        iniFile[$"{nextPointSeries}:LineStyle"] = "0";
                        iniFile[$"{nextPointSeries}:LabelPosition"] = "0";
                        iniFile[$"{nextPointSeries}:Visible"] = "0";

                        iniFile.Add($"{nextPointSeries}:LegendText", $"{data.TestName} {graphName}");

                        (double, double)[] points = FTAnlzer.PythonTupleListParse(graphData).ToArray();

                        iniFile.Add($"{nextPointSeries}:PointCount", $"{points.Length}");
                        iniFile.Add($"{nextPointSeries}:Points", string.Join(';', points.Select(p => $"{p.Item1},{p.Item2}")));

                        using FileStream file = File.Open(data.OutputFile, FileMode.OpenOrCreate);
                        using StreamWriter fileWriter = new StreamWriter(file);

                        foreach (var keyGroup in iniFile.Keys.GroupBy(lel => lel.Split(':')[0]))
                        {
                            fileWriter.WriteLine($"[{keyGroup.Key}]");

                            foreach (var key in keyGroup)
                            {
                                string valueKey = key.Split(':')[1];
                                fileWriter.WriteLine($"{valueKey} = {iniFile[key]}");
                            }

                            fileWriter.WriteLine();
                        }
                    }
                }
            };

        public static void Export(string outputFile, string testName, ProgramsThatReadOutput program, bool append,
            (string graphName, string graphData, Color color) graph)
        {
            Savers[program](new ExportData()
            {
                Append = append,
                Graph = graph,
                OutputFile = outputFile,
                TestName = testName
            });
        }
    }
}
