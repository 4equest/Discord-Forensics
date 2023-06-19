using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CacheViewer
{
    internal class ChromeCacheView
    {
        public static void ExportCache(string dirName)
        {
            string tmpPath = "tmp.tsv";
            string tmpDir = "tmp";

            ExportCacheList(dirName, tmpPath);

            
            var cacheInfos = File.ReadAllLines(tmpPath)
                .Skip(1) // skip header row
                .Select(line => line.Split('\t'))
                .Select(columns => new CacheInfo {
                    Url = columns[0],
                    ContentType = columns[1],
                    FileSize = columns[2],
                })
                .Where(info => info.Url.Contains("/messages") && info.ContentType == "application/json" && info.FileSize != "0")
                .ToList();

            Console.WriteLine(cacheInfos.Count);

            if(!Directory.Exists(tmpDir))
            {
                Directory.CreateDirectory(tmpDir);
            }
            
            foreach(var cacheInfo in cacheInfos)
            {
                CopyCache(dirName, tmpDir, cacheInfo);
            }
            
        }

        static void CopyCache(string dirName,string tmpDir, CacheInfo cacheInfo)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "ChromeCacheView.exe";
            startInfo.Arguments = "/copycache \"" + cacheInfo.Url + "\" \"application/json\" /CopyFilesFolder \"" + tmpDir + "\" -folder \"" + dirName + "\"";
            Console.WriteLine(startInfo.FileName + " " + startInfo.Arguments);
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        
        static void ExportCacheList(string dirName, string tmpPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "ChromeCacheView.exe";
            startInfo.Arguments = "/stab \"" + tmpPath + "\" /Columns URL,ContentType,FileSize -folder \"" + dirName + "\"";
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
