using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DahomeyUnload
{
    class Program
    {
        static List<WeakReference> refs = new List<WeakReference>();

        static void Main(string[] args)
        {
            Console.WriteLine($"Beginning test...");

            CallJson();

            RequestUnload();

            for (int i = 0; i < 25; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            for(int i = 0; i < refs.Count; i++)
            {
                Console.WriteLine($"Reference #{i} alive: {refs[i].IsAlive}");

                AnalyseReference(i);
            }

            Console.ReadLine();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void AnalyseReference(int i)
        {
            if (refs[i].IsAlive)
            {
                var tlc = refs[i].Target as TestLoadContext;
                Console.WriteLine($"Reference #{i} -> context {tlc.Name} with following assemblies loaded: {string.Join(", ", tlc.Assemblies.Select(x => x.FullName))}");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void RequestUnload()
        {
            foreach(var x in refs)
            {
                var tlc = x.Target as TestLoadContext;
                Console.WriteLine($"Unloading context {tlc.Name} with following assemblies loaded: {string.Join(", ", tlc.Assemblies.Select(x => x.GetName().Name))}.");
                tlc.Unload();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void CallJson()
        {
            TestLoadContext tlc = new TestLoadContext("LibraryLoaded");

            var asm = tlc.LoadFromAssemblyPath(Path.GetFullPath("LibraryLoaded.dll"));

            AddContext(tlc);

            var prg = asm.DefinedTypes.Where(x => x.Name == "Program").First();

            var test = prg.DeclaredMethods.Where(x => x.Name == "Test").First();

            test.Invoke(null, null);
        }

        public static void AddContext(TestLoadContext tlc)
        {
            Console.WriteLine($"[!] Now tracking context {tlc.Name} with following assemblies loaded: {string.Join(", ", tlc.Assemblies.Select(x => x.FullName))}");

            refs.Add(new WeakReference(tlc, true));
        }
    }
}
