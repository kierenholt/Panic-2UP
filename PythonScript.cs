using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Panic
{
    public static class PythonScript
    {
        //private Func<string, string> funcC;

        //public PythonScript(Func<string, string> paramfuncC)
        //{
        //    funcC = paramfuncC;
        //}

        //public string Execute(string input, Player player)
        //{
        //    string output = funcC(input);
        //    return output;
        //}

        private static string argumentList(int numArguments)
        {
            return string.Join(",", Enumerable.Range(0, numArguments).Select(n => n.toLowerAlpha()));
        }

        public static Func<string, string> Create(string pythonCode, string functionName, int numArguments)
        {
            /* bring up an IronPython runtime */
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();

            /* create a source tree from code */

            if (string.IsNullOrEmpty(pythonCode))
                return null;

            ScriptSource source =
                engine.CreateScriptSourceFromString(pythonCode + String.Format(@"

def alwaysReturnString({1}):
  return str({0}({1}))
", functionName, argumentList(numArguments)));

            IronRubyErrors errors = new IronRubyErrors();
            source.Compile(errors);

            if (errors.ErrorCode != 0)
            {
                MessageBox.Show(string.Format("Syntax error /r/n {0} at line {1} column {2}",
                    errors.Message,
                    errors.Span.Start.Line,
                    errors.Span.Start.Column), "Syntax error");
                return null;
            }

            try
            {
                source.Execute(scope);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }

            PythonFunction returnPythonFunction;
            PythonFunction movePythonFunction;
            if (scope.TryGetVariable(functionName, out movePythonFunction) && scope.TryGetVariable("alwaysReturnString", out returnPythonFunction))
            {
                if (movePythonFunction.__code__.co_argcount != numArguments)
                {
                    MessageBox.Show(string.Format("the function named '{0}' must have only one argument", functionName));
                }

                Func<string, string> funcC;
                if (scope.TryGetVariable<Func<string, string>>(returnPythonFunction.__name__, out funcC))
                {
                    return funcC;
                }
            }
            else
            {
                MessageBox.Show(string.Format(@"the function name '{0}({1})' is not found in your code. Try something like:

def {0}({1}):
  return ""F""", functionName, numArguments));
            }
            return null;
        }
    }

    public class IronRubyErrors : ErrorListener
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public Severity sev { get; set; }
        public SourceSpan Span { get; set; }

        public override void ErrorReported(ScriptSource source, string message, Microsoft.Scripting.SourceSpan span, int errorCode, Microsoft.Scripting.Severity severity)
        {
            Message = message;
            ErrorCode = errorCode;
            sev = severity;
            Span = span;
        }
    }
}
