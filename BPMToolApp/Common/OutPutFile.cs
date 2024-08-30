using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMToolApp.Common
{
    /// <summary>
    /// 输出文件
    /// </summary>
    public class OutPutFile
    {
        object obj = new object();
        public void Write(string Mess, string filePath)
        {
            try
            {
                lock (obj)
                {
                    //String filePath = System.Threading.Thread.GetDomain().BaseDirectory + FileName;
                    if (!File.Exists(filePath) == true)
                    {
                        FileStream fs1 = new FileStream(filePath, FileMode.Create, FileAccess.Write);//创建写入文件 
                        StreamWriter sw = new StreamWriter(fs1);
                        sw.WriteLine(Mess);//开始写入值
                        sw.Close();
                        fs1.Close();
                    }
                    else
                    {
                        FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write);
                        StreamWriter sr = new StreamWriter(fs);
                        sr.WriteLine(Mess);//开始写入值
                        sr.Close();
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
