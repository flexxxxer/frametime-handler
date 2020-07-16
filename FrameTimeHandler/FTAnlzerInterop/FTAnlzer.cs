using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia.Media;
using Microsoft.Extensions.Configuration.Ini;
using PythonInterop;

namespace FrameTimeHandler.FTAnlzerInterop
{
    public static class FTAnlzer
    {
        public static Lazy<PythonInterpreter> Interpreter { get; } = new Lazy<PythonInterpreter>(() => PythonEngine.PythonInterpreters.First(i => i.Version[0] == '3'));

        public static string[] SupportedPrograms
        {
            get
            {
                PythonEngine engine = new PythonEngine(Interpreter.Value);

                ExecutionResult result = engine.Execute(Directory.GetCurrentDirectory() + "/ftanlzer/ftanlzer.py", "--spn");

                return result.StdOut
                             .Split(';')
                             .Select(x => x.Replace("\r", "").Replace("\n", ""))
                             .ToArray();
            }
        }

        public static (IEnumerable<(string graphName, string graphData, Color color)> info, string error) GetGraphsData(string filePath, string program, IEnumerable<(GraphTypes graphType, Color color)> graphs)
        {
            var graphsCopy = graphs as (GraphTypes graphType, Color color)[] ?? graphs.ToArray(); // multiple enumeration maybe
            string args = $"-f {filePath} -p {program} {string.Join(' ', graphsCopy.Select(x => GraphRepresentations[x.graphType]))}";

            PythonEngine engine = new PythonEngine(Interpreter.Value);
            ExecutionResult result = engine.Execute(Directory.GetCurrentDirectory() + "/ftanlzer/ftanlzer.py", args);

            if (result.StdError.Replace(@"\r\n", "").Length != 0)
            {
                return (Enumerable.Empty<(string graphName, string graphData, Color color)>(), result.StdError);
            }

            var parser = new IniStreamConfigurationProvider(new IniStreamConfigurationSource());

            parser.Load(new MemoryStream(Encoding.ASCII.GetBytes(result.StdOut.Replace(@"\r\n", ""))));

            return (graphsCopy.Select(gr =>
            {
                parser.TryGet($"{GraphHumanRepresentations[gr.graphType]}:data", out string data);
                return (GraphHumanRepresentations[gr.graphType], data, gr.color);
            }), "");
        }

        public static IEnumerable<(double f, double s)> PythonTupleListParse(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                List<char> tupleChars = new List<char>(50);

                if ('(' == s[i])
                {
                    while (true)
                    {
                        i++;

                        if (s[i] == ')')
                            break;

                        tupleChars.Add(s[i]);
                    }

                    string tupleString = new string(tupleChars.ToArray());

                    string[] pair = tupleString.Split(',');

                    yield return (float.Parse(pair[0]), float.Parse(pair[1]));
                }
            }
        }

        public static Dictionary<GraphTypes, string> GraphRepresentations = new Dictionary<GraphTypes, string>()
        {
            {GraphTypes.FrameTiming, "--ftg"},
            {GraphTypes.ProbabilityDensity, "--pdensg"},
            {GraphTypes.ProbabilityDistribution, "--pdistg"},
        };

        public static Dictionary<GraphTypes, string> GraphHumanRepresentations = new Dictionary<GraphTypes, string>()
        {
            {GraphTypes.FrameTiming, "frame timing graph"},
            {GraphTypes.ProbabilityDensity, "probability density graph"},
            {GraphTypes.ProbabilityDistribution, "probability distribution graph"},
        };
    }
}