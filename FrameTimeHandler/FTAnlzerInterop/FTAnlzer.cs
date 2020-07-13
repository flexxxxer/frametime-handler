using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        
    }
}