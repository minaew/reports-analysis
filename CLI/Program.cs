using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using PdfExtractor;
using PdfExtractor.Parsers;

namespace CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            ICategories categories;
            string path;
            switch (args.Length)
            {
                case 0:
                    Console.WriteLine(
                        $"{Assembly.GetExecutingAssembly().GetName().Name} path [categories-file] [cases-file]");
                    return -1;

                case 1:
                    path = args[0];
                    categories = new EmptyCategories();
                    break;

                case 2:
                    path = args[0];
                    categories = new Categories(args[1]);
                    break;

                default:
                    path = args[0];
                    categories = new Categories(args[1], args[2]);
                    break;
            }

            var operations = new MetaParser(categories).Parse(path);

            var options = new JsonSerializerOptions {WriteIndented = true};
            var content = JsonSerializer.Serialize(operations.OrderBy(o => o.DateTime), options);
            Console.WriteLine(content);

            return 0;
        }
    }
}
