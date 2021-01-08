using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace DahomeyUnload
{
    internal class TestLoadContext : AssemblyLoadContext
    {
        public TestLoadContext(string name) : base(name: name, true)
        {

        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name == "Dahomey.Json")
            {
                TestLoadContext tlc = new TestLoadContext(assemblyName.ToString());

                Program.AddContext(tlc);

                return tlc.LoadFromAssemblyPath(Path.GetFullPath("Dahomey.Json.dll"));
            }

            return base.Load(assemblyName);
        }
    }
}
