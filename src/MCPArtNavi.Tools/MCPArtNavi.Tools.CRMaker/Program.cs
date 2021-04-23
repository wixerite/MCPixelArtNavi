using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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


            // Checks file exists.

            var crMdFileName = "03_01_ColorReference.md";
            var crMdFilePath = Path.Combine(docsDir, crMdFileName);
            if (File.Exists(crMdFilePath))
            {
                Console.WriteLine("[WARN] '{0}' is exists.", crMdFileName);
                Console.Write("Please press enter key to continue.");
                Console.ReadLine();
            }


            // Creates color sample files.

            var cc = new ColorConverter();
            var enabledItems = MCItemUtils.EnabledItems;
            var enabledItemSamples = enabledItems.ToDictionary(e => e, e => (Color)cc.ConvertFromString(e.ItemColor));

            var colorsDir = Path.Combine(docsDir, "docs_images", "colors");
            if (Directory.Exists(colorsDir) == false)
                Directory.CreateDirectory(colorsDir);

            string _mcItemToSampleImagePath(IMCItem item) => Path.Combine(colorsDir, $"{item.ItemId}.png");

            var imageWidth = 5;
            var imageHeight = imageWidth;
            foreach (var kvp in enabledItemSamples)
            {
                var sampleImagePath = _mcItemToSampleImagePath(kvp.Key);
                if (File.Exists(sampleImagePath))
                {
                    Console.WriteLine("[INFO] Skipped '{0}' because already file exists.", kvp.Key.ItemId);
                    continue;
                }

                using (var bmp = new Bitmap(imageWidth, imageHeight))
                {
                    for (var i = 0; i < imageHeight; i++)
                    {
                        for (var j = 0; j < imageWidth; j++)
                            bmp.SetPixel(j, i, kvp.Value);
                    }

                    bmp.Save(sampleImagePath, ImageFormat.Png);
                }

                Console.WriteLine("[INFO] Saved color sample for '{0}'.", kvp.Key.ItemId);
            }


            // Loads template.

            var templateDir = Path.Combine(asmDir, "Assets", "Tools", "CRMaker");
            var templateFileBaseName = "03_01_ColorReference";
            var templateFileLang = "ja-JP";
            var templateFileExt = "md";
            var templateFileName = String.Empty;

            if (String.IsNullOrEmpty(templateFileLang))
                templateFileName = String.Concat(templateFileBaseName + "." + templateFileExt);
            else
                templateFileName = String.Concat(templateFileBaseName + "." + templateFileLang + "." + templateFileExt);

            var templateData = File.ReadAllText(Path.Combine(templateDir, templateFileName));


            // Creates table source.

            var tableSourceSb = new StringBuilder();
            tableSourceSb.Append("| No | Id | Name | Color | Color sample |\r\n");
            tableSourceSb.Append("|----|----|------|-------|--------------|\r\n");

            var scount = 0;
            foreach (var kvp in enabledItemSamples)
            {
                scount++;
                
                tableSourceSb.Append("|");
                tableSourceSb.Append(scount);

                tableSourceSb.Append("|");
                tableSourceSb.Append(kvp.Key.ItemId);
                
                tableSourceSb.Append("|");
                tableSourceSb.Append(kvp.Key.ItemName);

                tableSourceSb.Append("|");
                tableSourceSb.Append(kvp.Key.ItemColor);

                tableSourceSb.Append("|");
                tableSourceSb.Append($"![{kvp.Key.ItemName}](docs_images/colors/{kvp.Key.ItemId}.png \"{kvp.Key.ItemName}\")");

                tableSourceSb.Append("|");
                tableSourceSb.Append("\r\n");
            }

            var tableSource = tableSourceSb.ToString();


            // Saves document.
            File.WriteAllText(crMdFilePath, templateData.Replace("%TABLE_POSITION%", tableSource));
        }
    }
}
