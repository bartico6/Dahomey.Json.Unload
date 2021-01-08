using Dahomey.Json;

using System;
using System.Text.Json;

namespace LibraryLoaded
{
    class Program
    {

        static void Test()
        {
            JsonSerializerOptions opts = new JsonSerializerOptions();

            opts.SetupExtensions();

            string json = JsonSerializer.Serialize(new TestConfig(), opts);

            Console.WriteLine($"\t > JSON: {json}");
        }
    }

    public class TestConfig
    {
        public string Something { get; set; }
    }
}
