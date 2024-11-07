using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Steam_to_GOG_Mod_Converter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> pathlist = File.ReadAllLines(@"Paths.txt").ToList();
            string STEAMPATH = pathlist[0].Replace("STEAMPATH=","");
            string GOGPATH = pathlist[1].Replace("GOGPATH=","");
            DirectoryInfo SteamDir = new DirectoryInfo(STEAMPATH);
            DirectoryInfo GOGDir = new DirectoryInfo(GOGPATH);
            Console.WriteLine(GOGDir.FullName);
            Console.WriteLine(SteamDir.FullName);
            foreach (DirectoryInfo file in SteamDir.GetDirectories())
            {
                string TrueName = File.ReadAllLines(file.FullName + @"\descriptor.mod").ToList().Where(x => x.StartsWith("name")).First().Replace("name=", "").Replace("\"", "");
                Console.WriteLine($"Found {file.Name} {TrueName}");
            }
            foreach (DirectoryInfo Dir in SteamDir.GetDirectories())
            {
                string TrueName = File.ReadAllLines(Dir.FullName + @"\descriptor.mod").ToList().Where(x => x.StartsWith("name")).First().Replace("name=", "").Replace("\"", "");
                string Target = Path.Combine(GOGDir.FullName, TrueName);
                Console.WriteLine($"{Dir.FullName} -> {Target}");
                if (!Directory.Exists(Target))
                {
                    Console.WriteLine($"Creating directory for {TrueName}.");
                    Directory.CreateDirectory(Target);
                }
                foreach (FileInfo file in Dir.GetFiles())
                {
                    File.Create(Path.Combine(Target, file.Name)).Close();
                    File.Copy(file.FullName, Path.Combine(Target, file.Name), true);
                    Console.WriteLine($"{file.FullName} -> {Path.Combine(Target, file.Name)}");
                }
            }
            foreach (DirectoryInfo Dir in SteamDir.GetDirectories())
            {
                string TrueName = File.ReadAllLines(Dir.FullName + @"\descriptor.mod").ToList().Where(x => x.StartsWith("name")).First().Replace("name=", "").Replace("\"", "");
                string Target = Path.Combine(GOGDir.FullName, TrueName);
                copySubDir(Dir, Target);
            }

            foreach(DirectoryInfo Dir in GOGDir.GetDirectories())
            {
                string TrueName = File.ReadAllLines(Dir.FullName + @"\descriptor.mod").ToList().Where(x => x.StartsWith("name")).First().Replace("name=", "").Replace("\"", "");
                string Target = Path.Combine (Dir.FullName, "descriptor.mod");
                string TargetEnd = Path.Combine(GOGDir.FullName, TrueName + ".mod");
                if (File.Exists(TargetEnd)) File.Delete(TargetEnd);
                File.Move(Target, TargetEnd);

                using (StreamWriter sr = new StreamWriter(TargetEnd, true))
                {
                    sr.WriteLine();
                    sr.WriteLine($"path = \"{Path.Combine(@"Mod/" , Dir.Name)}\"");
                }
            }
            Console.WriteLine("Finished");
            Console.WriteLine("Press any button to exit");

            Console.ReadKey();
        }
        static void copySubDir(DirectoryInfo Dir, string Target)
        {
            Console.WriteLine($"Found {Dir.Name}");
            foreach (DirectoryInfo subDir in Dir.GetDirectories())
            {
                Console.WriteLine($"Found {subDir.Name} (sub)");
                string subTarget = Path.Combine(Target, subDir.Name);
                if (!Directory.Exists(subTarget))
                {
                    Directory.CreateDirectory(subTarget);
                    Console.WriteLine($"Created {subTarget}");
                    
                }
                foreach (FileInfo file in subDir.GetFiles())
                {
                    File.Create(Path.Combine(subTarget, file.Name)).Close();
                    File.Copy(file.FullName, Path.Combine(subTarget, file.Name), true);
                    Console.WriteLine($"{file.FullName} -> {Path.Combine(subTarget, file.Name)}");
                }
                foreach (DirectoryInfo subsubDir in subDir.GetDirectories())
                {
                    copySubDir(subDir,subTarget);
                }
            }

        }
    }
}
