using BPM;
using BPM.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMToolApp.Common
{
    /// <summary>
    /// BPM中Flow开头类型的方法
    /// </summary>
    public class Flow
    {
        /// <summary>
        /// 判断字段是否在另一个FlowDataTable中存在
        /// </summary>
        /// <param name="sourceColumnName"></param>
        /// <param name="tag_fdt"></param>
        /// <returns></returns>
        public static Boolean IsColumnInAnotherFlowDataTable(string sourceColumnName, FlowDataTable tag_fdt)
        {
            if (tag_fdt.Columns[sourceColumnName] == null)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 获取表可重复类型
        /// </summary>
        /// <param name="fdt"></param>
        /// <returns></returns>
        public static string GetRepeatleableTableDescription(FlowDataTable fdt)
        {
            if (fdt.IsRepeatableTable == false)
                return "非可重复表";
            if (fdt.IsRepeatableTable == true && fdt.AllowAddRecord == false)
                return "可重复表-不允许添加行";
            if (fdt.IsRepeatableTable == true && fdt.AllowAddRecord == true)
                return "可重复表-允许添加行";
            return "不知道此表是否为grid表";
        }
        /// <summary>
        /// 节点转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static T ConvertNode<T>(ProcessNode node) where T : ProcessNode
        {
            if (node is T convertedNode)
            {
                return convertedNode;
            }
            throw new InvalidCastException($"转换节点失败： {node.GetType().Name} -> {typeof(T).Name}");
        }
        /// <summary>
        /// 处理路由显示名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ConvertDisplayName_ConsignRoutingType(ConsignRoutingType type)
        {
            switch (type)
            {
                case ConsignRoutingType.Serial:
                    return "顺序签核";
                case ConsignRoutingType.Parallel:
                    return "平行签核";
                case ConsignRoutingType.None:
                    return "处理人决定";
                default:
                    return "未知";
            }
        }
        /// <summary>
        /// 人员选择显示名称
        /// </summary>
        /// <param name="JumpIfNoParticipants"></param>
        /// <returns></returns>
        public static string ConvertDisplayName_MultiRecipient(bool JumpIfNoParticipants)
        {
            if (JumpIfNoParticipants)
                return "跳过本步骤";
            else
                return "不能提交";
        }
        /// <summary>
        /// 不选择处理人显示名称
        /// </summary>
        /// <param name="JumpIfNoParticipants"></param>
        /// <returns></returns>
        public static string ConvertDisplayName_JumpIfNoParticipants(bool MultiRecipient)
        {
            if (MultiRecipient)
                return "一人或多人";
            else
                return "仅一人";
        }
        /// <summary>
        /// 名称对象转换字符串
        /// 默认以逗号分割
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static string ConvertDisplayName_BPMObjectNameCollection(BPMObjectNameCollection names, string split = ",")
        {
            if (names == null || names.Count == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (var name in names)
            {
                sb.Append($"{name}{split}");
            }
            return sb.ToString(0, sb.Length - 1);
        }
        /// <summary>
        /// 自动同意规则
        /// </summary>
        /// <param name="JumpIfSameOwnerWithInitiator"></param>
        /// <returns></returns>
        public static string ConvertDisplayName_JumpIfRoute(bool JumpIfSameOwnerWithInitiator, bool JumpIfSameOwnerWithPrevStep, bool JumpIfProcessed, string split = "</br>")
        {
            StringBuilder sb = new StringBuilder();
            if (JumpIfSameOwnerWithInitiator)
                sb.AppendLine("处理人就是提交人");
            if (JumpIfSameOwnerWithPrevStep)
                sb.AppendLine("处理人和上一步相同");
            if (JumpIfProcessed)
                sb.AppendLine("处理人已经审批过");
            return sb.ToString().Replace("\r\n", split);
        }
        /// <summary>
        /// 无对应处理人
        /// </summary>
        /// <returns></returns>
        public static string ConvertDisplayName_NoParticipantsAction(NoParticipantsAction action)
        {
            switch (action)
            {
                case NoParticipantsAction.PreventSubmit: return "不能提交";
                case NoParticipantsAction.Jump: return "跳过本步骤";
                case NoParticipantsAction.SendToExceptionList: return "发送给管理员（异常列表）";
                default: return "未知";
            }
        }
        /// <summary>
        /// 处理策略
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static string ConvertDisplayName_ParticipantPolicy(ParticipantPolicy policy, string split = "</br>")
        {
            ParticipantPolicyType pType = policy.PolicyType;
            bool boo = policy.BParam1;//处理策略后的勾选
            switch (pType)
            {
                case ParticipantPolicyType.FirstUser:
                    return string.Format("				列表中的第一处理人{0}", policy.BParam1 ? $"{split}外出时自动转发给代理人" : "");
                case ParticipantPolicyType.Share:
                    return string.Format("				列表中人员共享处理", policy.BParam1 ? $"{split}直接将任务送入第一个共享人员的待处理任务列表" : "");
                case ParticipantPolicyType.All:
                    return string.Format("				发送给列表中的所有人", policy.BParam1 ? $"{split}外出时自动转发给代理人" : "");
                default:
                    return "未知";
            }
        }
        /// <summary>
        /// 创建条件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ConvertDisplayName_CreateRecordType(CreateRecordType type)
        {
            switch (type)
            {
                case CreateRecordType.FirstTimeEnterStep:
                    return "第一次进入本步骤";
                case CreateRecordType.EveryEnterStep:
                    return "每次进入本步骤";
                case CreateRecordType.RecordNotExist:
                    return "相同记录不存在";
                default:
                    return "未知";
            }
        }
    }
}
