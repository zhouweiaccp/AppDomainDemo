using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CSharp;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
/******* // Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            //string path = AppDomain.CurrentDomain.BaseDirectory + "1.txt";
            //string cont = System.IO.File.ReadAllText(path);
            //Evaluator evl = new Evaluator(new EvaluatorItem(typeof(string), cont, "test"));

            //string i = evl.EvaluateString("test");********/
namespace EDoc2.Common.Validate
{
    public class Evaluator
    {
        #region Construction
        public Evaluator(EvaluatorItem[] items)
        {
            ConstructEvaluator(items);
        }

        public Evaluator(Type returnType, string expression, string name)
        {
            EvaluatorItem[] items = { new EvaluatorItem(returnType, expression, name) };
            ConstructEvaluator(items);
        }

        public Evaluator(EvaluatorItem item)
        {
            EvaluatorItem[] items = { item };
            ConstructEvaluator(items);
        }

        private void ConstructEvaluator(EvaluatorItem[] items)
        {


//            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
//CompilerParameters cp = new CompilerParameters();
//cp.ReferencedAssemblies.Add("System.Windows.Forms. dll")
//cp.GenerateInMemory = false;
//cp.OutputAssembly = "myasm.dll";

//CompilerResults result =
//provider.CompileAssemblyFromSource(cp, sourceCode);
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
         //   ICodeCompiler comp = new CSharpCodeProvider().CreateCompiler();
            CompilerParameters cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.data.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;

            StringBuilder code = new StringBuilder();
            code.Append("using System; \n ");
            code.Append("using System.Data; \n ");
            code.Append("using System.Data.SqlClient; \n ");
            code.Append("using System.Data.OleDb; \n ");
            code.Append("using System.Xml; \n ");
            code.Append("namespace EDoc2.Common.Validate { \n ");
            code.Append("  public class _Evaluator { \n ");
            foreach(EvaluatorItem item in items)
            {
                code.AppendFormat("    public {0} {1}() ", 
                    item.ReturnType.Name, 
                    item.Name);
                code.Append("{ ");
                code.AppendFormat("   {0}; ", item.Expression);
                code.Append("}\n");
            }
            code.Append("} }");

            CompilerResults cr = provider.CompileAssemblyFromSource(cp, code.ToString());// comp.CompileAssemblyFromSource(cp, code.ToString());
            if (cr.Errors.HasErrors)
            {
                StringBuilder error = new StringBuilder();
                error.Append("Error Compiling Expression: ");
                foreach (CompilerError err in cr.Errors)
                {
                    error.AppendFormat("{0}\n", err.ErrorText);
                }
                throw new Exception("Error Compiling Expression: " + error.ToString());
            }
            Assembly a = cr.CompiledAssembly;
            _Compiled = a.CreateInstance("EDoc2.Common.Validate._Evaluator");
        }
        #endregion

        #region Public Members
        public int EvaluateInt(string name)
        {
            return (int) Evaluate(name);
        }

        public string EvaluateString(string name)
        {
            return (string) Evaluate(name);
        }

        public bool EvaluateBool(string name)
        {
            return (bool) Evaluate(name);
        }

        public object Evaluate(string name)
        {
            MethodInfo mi = _Compiled.GetType().GetMethod(name);
            return mi.Invoke(_Compiled, null);
        }
        #endregion

        #region Static Members
        static public int EvaluateToInteger(string code)
        {
            Evaluator eval = new Evaluator(typeof(int), code, staticMethodName);
            return (int) eval.Evaluate(staticMethodName);
        }

        static public string EvaluateToString(string code)
        {
            Evaluator eval = new Evaluator(typeof(string), code, staticMethodName);
            return (string) eval.Evaluate(staticMethodName);
        }
    
        static public bool EvaluateToBool(string code)
        {
            Evaluator eval = new Evaluator(typeof(bool), code, staticMethodName);
            return (bool) eval.Evaluate(staticMethodName);
        }

        static public object EvaluateToObject(string code)
        {
            Evaluator eval = new Evaluator(typeof(object), code, staticMethodName);
            return eval.Evaluate(staticMethodName);
        }
        #endregion

        #region Private
        const string staticMethodName = "__foo";
        Type _CompiledType = null;
        object _Compiled = null;
        #endregion
    }

    public class EvaluatorItem
    {
        public EvaluatorItem(Type returnType, string expression, string name)
        {
            ReturnType = returnType;
            Expression = expression;
            Name = name;
        }

        public Type ReturnType;
        public string Name;
        public string Expression;
    }
}
