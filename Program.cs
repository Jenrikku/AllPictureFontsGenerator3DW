using System;
using System.IO;
using System.Text;
using Hack.io.YAZ0;
using SZS;

namespace AllPictureFontsGenerator3DW {
    class Program {
        static string BFFNTPath;
        static string LocalizedDataPath;
        static string OutputPath;

        static void Main(string[] args) {
            // Enables Shift-JIS encoding.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("Shift-JIS");

            // Input.
            Console.WriteLine("Please input the bffnt path (PictureFont80.bffnt):");
            BFFNTPath = Console.ReadLine().Replace("\"", "");

            Console.WriteLine("Now, input a vanilla LocalizedData folder from 3DW or 3DW+BF:");
            LocalizedDataPath = Console.ReadLine().Replace("\"", "");

            Console.WriteLine("Finally, choose an output path:");
            OutputPath = Console.ReadLine().Replace("\"", "");

            Console.WriteLine();

            // Cheks for each folder in LocalizedData.
            foreach(DirectoryInfo dir in new DirectoryInfo(LocalizedDataPath).GetDirectories()) {
                if(dir.Name != "Common") {
                    Console.WriteLine(dir.Name + "...");

                    // Decompresses and loads the file to the RAM.
                    SarcData currentData =
                        SARC.UnpackRamN(
                            YAZ0.Decompress(
                                Path.Join(dir.FullName, "LayoutData\\", "FontData.szs")));

                    // Replaces the font on the RAM.
                    currentData.Files["PictureFont80.bffnt"] = File.ReadAllBytes(BFFNTPath);

                    // Stores the szs
                    MemoryStream uncompressed = new MemoryStream(SARC.PackN(currentData).Item2);

                    Directory.CreateDirectory(Path.Join(OutputPath, "LocalizedData\\", dir.Name, "LayoutData\\"));

                    File.WriteAllBytes(
                        Path.Join(OutputPath, "LocalizedData\\", dir.Name, "LayoutData\\", "FontData.szs"), YAZ0.Compress(uncompressed));

                    uncompressed.Close();
                }
            }
        }
    }
}
