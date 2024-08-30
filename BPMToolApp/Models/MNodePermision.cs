using System;

namespace BPMToolApp.Models
{
    /// <summary>
    /// 任务操作权限枚举，与NodePermision是一样的
    /// </summary>
    [Flags]
    public enum MNodePermision : long
    {
        None = 0,
        AgentPost = 1,//代填
        AgentWrite = 2,
        PickBackRestart = 4,//取回
        Abort = 8,//撤销
        Delete = 16,//删除
        Continue = 32,
        Transfer = 64,//委托
        Jump = 128,//调度
        Public = 256,//公开
        BatchApprove = 512,//批量同意
        RecedeRestart = 1024,//退回重填
        RecedeBack = 2048,//退回某步
        Reject = 4096,//拒绝
        AssignOwner = 8192,
        Consign = 16384,//阅批（加签）
        InviteIndicate = 32768,//阅示
        PickBack = 65536,//取回
        Inform = 131072,//知会
        MobileApprove = 262144,//手机上审批
        ReActive = 524288,
        PickBackExt = 1048576,
        Repair = 2097152,
        StepKM = 4194304,
        ProcessKM = 8388608,
        Reminder = 16777216
    }
}
