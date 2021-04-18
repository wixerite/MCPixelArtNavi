using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MCPArtNavi.Common;
using MCPArtNavi.Common.Items;

namespace MCPArtNavi.Tools.CRMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("/*************************************");
            Console.WriteLine(" * MC Pixel Art Navi Tools");
            Console.WriteLine(" *             Color Reference Maker");
            Console.WriteLine(" *************************************/");
            Console.WriteLine();

            var asmPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            var asmDir = Path.GetDirectoryName(asmPath);

            start:

            Console.WriteLine("Enter path for 'docs' directory:");
            Console.WriteLine("ex) C:\\dev\\MCPixelArtNavi\\docs");
            Console.Write(">");

            var docsDir = Console.ReadLine();

            if (String.IsNullOrEmpty(docsDir))
            {
                var splittedPath = asmDir.Split('\\');

                var devIndex = splittedPath.ToList().IndexOf("src");
                if (devIndex < 0)
                {
                    Console.WriteLine("[ERROR] Path is not entered.");
                    goto start;
                }

                var devDir = String.Join("\\", splittedPath.Take(devIndex));

                docsDir = Path.Combine(devDir, "docs");

                Console.WriteLine("[INFO] The following path will be used instead of the path you have entered.");
                Console.WriteLine("[INFO] {0}", docsDir);
                Console.Write("Please press enter key to continue.");
                Console.ReadLine();
            }

            if (Directory.Exists(docsDir) == false)
            {
                Console.WriteLine("[ERROR] Directory not found.");
                goto start;
            }

            var crMdFileName = "03_01_ColorReference.md";
            var crMdFilePath = Path.Combine(docsDir, crMdFileName);
            if (File.Exists(crMdFilePath))
            {
                Console.WriteLine("[WARN] '{0}' is exists.", crMdFileName);
                Console.Write("Please press enter key to continue.");
                Console.ReadLine();
            }

            var templateDir = Path.Combine(asmDir, "Assets", "Tootls", "CRMaker");
            var templateFileBaseName = "03_01_ColorReference";
            var templateFileLang = "ja-JP";
            var templateFileExt = "md";
            var templateFileName = String.Empty;

            if (String.IsNullOrEmpty(templateFileLang))
                templateFileLang = String.Concat(templateFileBaseName + "." + templateFileExt);
            else
                templateFileLang = String.Concat(templateFileBaseName + "." + templateFileLang + "." + templateFileExt);

            var templateData = File.ReadAllText(Path.Combine(templateDir, templateFileName));



            var enabledItems = MCItemUtils.EnabledItems;

        }
    }
}
