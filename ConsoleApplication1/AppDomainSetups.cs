using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleApplication1
{
    class AppDomainSetups
    {
        static byte[] loadFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            fs = null;
            return buffer;
        }
        public static void testc()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "ClassLibrary1";
            setup.ApplicationBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "ApplicationBase");
            setup.ShadowCopyDirectories = AppDomain.CurrentDomain.BaseDirectory + "ShadowCopyDirectories";
            setup.CachePath = AppDomain.CurrentDomain.BaseDirectory + "cache";
            setup.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "priviate");
            setup.CachePath = setup.ApplicationBase;
            setup.ShadowCopyFiles = "true"; //启用影像复制程序集

            AppDomain.CurrentDomain.SetShadowCopyFiles();

            //创建应用域appdomian
            System.AppDomain appDomain = System.AppDomain.CreateDomain("Domain" +
"someName",
new System.Security.Policy.Evidence(AppDomain.CurrentDomain.Evidence),
setup);
            AppDomain newDomain = AppDomain.CreateDomain("newDomain");

            byte[] rawAssembly = loadFile(setup.PrivateBinPath + "\\ClassLibrary1.dll");//不存在就会报错
            Assembly assembly = appDomain.Load(rawAssembly, null);
            ClassLibrary2.IClass1 obj = (ClassLibrary2.IClass1)assembly.CreateInstance("ClassLibrary1.Class1");
            //appDomain.CreateInstanceFromAndUnwrap(setup.PrivateBinPath + "\\ClassLibrary1.dll", "ClassLibrary1.Class1");

            Console.WriteLine(obj.test());
            AppDomain.Unload(appDomain);
            appDomain = null;
        }
    }
}
