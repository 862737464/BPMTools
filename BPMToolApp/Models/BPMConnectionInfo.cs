using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMToolApp.Models
{
    /// <summary>
    /// 读取配置文件，获得BPM的连接信息
    /// </summary>
    public class BPMConnectionInfo
    {
        /// <summary>
        /// BPM的服务IP
        /// </summary>
        public static string Server { get { return ConfigurationManager.AppSettings["server"]; } }
        /// <summary>
        /// BPM管理员账号
        /// </summary>
        public static string UID { get { return ConfigurationManager.AppSettings["uid"]; } }
        /// <summary>
        /// BPM管理员账号密码
        /// </summary>
        public static string PWD { get { return ConfigurationManager.AppSettings["pwd"]; } }
        /// <summary>
        /// BPM的端口号
        /// </summary>
        public static int Port { get { return Convert.ToInt32(ConfigurationManager.AppSettings["port"]); } }
    }
}
