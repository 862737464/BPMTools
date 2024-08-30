using BPM.Client;
using BPMToolApp.Common;
using BPMToolApp.Models;

namespace BPMToolApp
{
    public class BPMConnectionManager : Singleton<BPMConnectionManager>
    {
        /// <summary>
        /// 获取一个BPM的连接
        /// 请注意用完了及时关闭
        /// </summary>
        /// <returns></returns>
        public BPMConnection CreateNewBPMConnection()
        {
            BPMConnection cn = new BPMConnection();
            cn.Open(BPMConnectionInfo.Server, BPMConnectionInfo.UID, BPMConnectionInfo.PWD, BPMConnectionInfo.Port);
            return cn;
        }
    }
}
