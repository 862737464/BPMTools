using BPM.Client;
using BPMToolApp.Common;
using BPMToolApp.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMToolApp
{
    internal class Program
    {
        static readonly string APPROOTPATH = AppDomain.CurrentDomain.BaseDirectory;
        static string ProcessRootPath;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("请输入BPM根目录：");
                ProcessRootPath = Console.ReadLine();
                string processPath = Path.Combine(ProcessRootPath, "Process");
                if (!Directory.Exists(processPath))
                {
                    Console.WriteLine($"指定目录不存在：{processPath}");
                }
                AnalysisProcess(processPath);
                break;
            }
            Console.ReadLine();
        }
        static void AnalysisProcess(string processPath)
        {
            var files = Directory.GetFiles(processPath, "*.flo");
            foreach (var fileFullPath in files)
            {
                BPMProcess bpmProcess = new BPMProcess();
                bpmProcess.Load(fileFullPath);

                var service = new OutPutProcessNodeInfoService(bpmProcess).Load();

                string fileName = bpmProcess.Name + ".html";
                string appDirProcessPath = processPath.Replace(ProcessRootPath, APPROOTPATH);
                if (!Directory.Exists(appDirProcessPath)) Directory.CreateDirectory(appDirProcessPath);
                new OutPutFile().Write(service.Html, Path.Combine(appDirProcessPath, fileName));

                Console.WriteLine("完成");

                //将流程文件输出成JSON文件
                //MLog.Log.Info(Path.GetFileName(fileFullPath)).Info(Newtonsoft.Json.JsonConvert.SerializeObject(bpmProcess, Newtonsoft.Json.Formatting.Indented));
            }
            var dirs = Directory.GetDirectories(processPath);
            foreach (var dir in dirs)
            {
                AnalysisProcess(dir);
            }
        }

    }
}
