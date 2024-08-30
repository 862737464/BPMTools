using BPM.Client;
using BPM;
using System;
using System.Linq;
using System.Text;
using BPMToolApp.Models;
using BPMToolApp.Common;
using System.Reflection;
using BPM.Client.ESB;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace BPMToolApp.Service
{
    /// <summary>
    /// 将一个流程中所有节点转换成html形式进行查看
    /// </summary>
    public class OutPutProcessNodeInfoService
    {
        /// <summary>
        /// 最终输出的HTML
        /// </summary>
        public string Html { get { return sb.ToString(); } }
        /// <summary>
        /// 组装HTML
        /// </summary>
        StringBuilder sb = new StringBuilder();
        BPMProcess bpmProcess;//流程信息
        FlowDataTableCollection global_fdts;//流程数据源表信息
        public OutPutProcessNodeInfoService(BPMProcess bpmProcess)
        {
            this.bpmProcess = bpmProcess;
            using (BPMConnection cn = BPMConnectionManager.Instance.CreateNewBPMConnection())
            {
                global_fdts = DataSourceManager.LoadDataSetSchema(cn, bpmProcess.GlobalTableIdentitys).Tables;
            }
        }
        public OutPutProcessNodeInfoService Load()
        {
            this.HtmlHead();
            this.HtmlBody();
            this.HtmlEnd();
            return this;
        }
        private void HtmlBody()
        {
            for (int i = 0; i < bpmProcess.Nodes.Count; i++)
            {
                var processNode = bpmProcess.Nodes[i];
                var methodName = processNode.GetType().Name;
                MethodInfo methodcall = this.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(ProcessNode) }, null);
                if (methodcall != null)
                {
                    sb.AppendLine("	<table>");
                    sb.AppendLine("		<tbody>");

                    sb.AppendLine("			<tr>");
                    sb.AppendLine("				<td colspan=\'6\'>");
                    sb.AppendLine("				节点名:");
                    sb.AppendLine(string.Format("				【{0}】", processNode.Name));
                    sb.AppendLine("				</td>");
                    sb.AppendLine("			</tr>");
                    sb.AppendLine("			<tr>");

                    if (processNode is HumanProcessNode)
                    {
                        var node = Flow.ConvertNode<HumanProcessNode>(processNode);
                        sb.AppendLine("			<tr>");
                        sb.AppendLine("				<td width=\'60\'>");
                        sb.AppendLine("				权限:");
                        sb.AppendLine("				</td>");
                        sb.AppendLine("				<td colspan=\'5\'>");
                        sb.AppendLine(string.Format("				{0}", ((MNodePermision)node.Permision)).ToString());
                        sb.AppendLine("				</td>");
                        sb.AppendLine("			</tr>");
                        sb.AppendLine("			<tr>");
                        sb.AppendLine("				<td>");
                        sb.AppendLine("				表单:");
                        sb.AppendLine("				</td>");
                        sb.AppendLine("				<td colspan=\'5\'>");
                        sb.AppendLine(string.Format("				{0}", node.Form));
                        sb.AppendLine("				</td>");
                        sb.AppendLine("			</tr>");
                        sb.AppendLine("			<tr>");
                        sb.AppendLine("				<td>");
                        sb.AppendLine("				移动表单:");
                        sb.AppendLine("				</td>");
                        sb.AppendLine("				<td colspan=\'5\'>");
                        sb.AppendLine(string.Format("				{0}", node.MobileForm));
                        sb.AppendLine("				</td>");
                        sb.AppendLine("			</tr>");
                    }

                    methodcall.Invoke(this, new object[] { processNode });

                    sb.AppendLine("		</tbody>");
                    sb.AppendLine("	</table>");

                    //审批类型节点，填充数据控制
                    if (processNode is HumanProcessNode)
                    {
                        this.NodeDataControl(Flow.ConvertNode<HumanProcessNode>(processNode));
                    }
                }
            }
        }

        #region Human节点
        /// <summary>
        /// 开始节点
        /// </summary>
        /// <param name="processNode"></param>
        private void StartNode(ProcessNode processNode)
        {
            StartNode node = Flow.ConvertNode<StartNode>(processNode);
            sb.AppendLine("			<tr>");
            sb.AppendLine("				<td>");
            sb.AppendLine("				处理人:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td colspan=\'5\'>");
            sb.AppendLine("				-");
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");
            sb.AppendLine("			<tr>");
            sb.AppendLine("				<td >");
            sb.AppendLine("				处理策略:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'300\'>");
            sb.AppendLine("				-");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'100\'>");
            sb.AppendLine("				无对应处理人时:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'100\'>");
            sb.AppendLine("				-");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'90\'>");
            sb.AppendLine("				自动同意规则:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'150\'>");
            sb.AppendLine("				-");
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");
        }
        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="processNode"></param>
        private void ActivityNode(ProcessNode processNode)
        {
            ActivityNode node = Flow.ConvertNode<ActivityNode>(processNode);
            sb.AppendLine("			<tr>");
            sb.AppendLine("				<td>");
            sb.AppendLine("				处理人:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td colspan=\'5\'>");
            foreach (Participant item in node.Participants)
            {
                sb.AppendLine(string.Format("				{0}", item.CodeBlock.CodeText));
            }
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");
            sb.AppendLine("			<tr>");
            sb.AppendLine("				<td >");
            sb.AppendLine("				处理策略:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'300\'>");
            sb.AppendLine(Flow.ConvertDisplayName_ParticipantPolicy(node.ParticipantPolicy));
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'100\'>");
            sb.AppendLine("				无对应处理人时:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'100\'>");
            sb.AppendLine(string.Format("				{0}", Flow.ConvertDisplayName_NoParticipantsAction(node.NoParticipantsAction)));
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'90\'>");
            sb.AppendLine("				自动同意规则:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'150\'>");
            sb.AppendLine(string.Format("				{0}", Flow.ConvertDisplayName_JumpIfRoute(node.JumpIfSameOwnerWithInitiator, node.JumpIfSameOwnerWithPrevStep, node.JumpIfProcessed)));
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");


        }
        /// <summary>
        /// 层级审批
        /// </summary>
        /// <param name="processNode"></param>
        private void DecisionNode(ProcessNode processNode)
        {
            DecisionNode node = Flow.ConvertNode<DecisionNode>(processNode);
            foreach (DecisionStep step in node.Steps)
            {
                sb.AppendLine("			</tr>");
                sb.AppendLine("				<td>");
                sb.AppendLine("				关卡名:");
                sb.AppendLine("				</td>");
                sb.AppendLine("				<td colspan=\'5\'>");
                sb.AppendLine(string.Format("				{0}", step.StepName));
                sb.AppendLine("				</td>");
                sb.AppendLine("			</tr>");

                sb.AppendLine("			<tr>");
                sb.AppendLine("				<td>");
                sb.AppendLine("				处理人:");
                sb.AppendLine("				</td>");
                sb.AppendLine("				<td colspan=\'5\'>");
                foreach (Participant item in step.Participants)
                {
                    sb.AppendLine(string.Format("				{0}", item.CodeBlock.CodeText));
                }
                sb.AppendLine("				</td>");
                sb.AppendLine("			</tr>");
                sb.AppendLine("			<tr>");
                sb.AppendLine("				<td >");
                sb.AppendLine("				处理策略:");
                sb.AppendLine("				</td>");
                sb.AppendLine("				<td width=\'300\'>");
                sb.AppendLine(Flow.ConvertDisplayName_ParticipantPolicy(step.ParticipantPolicy));
                sb.AppendLine("				</td>");
                sb.AppendLine("				<td width=\'100\'>");
                sb.AppendLine("				无对应处理人时:");
                sb.AppendLine("				</td>");
                sb.AppendLine("				<td width=\'100\'>");
                sb.AppendLine(string.Format("				{0}", Flow.ConvertDisplayName_NoParticipantsAction(step.NoParticipantsAction)));
                sb.AppendLine("				</td>");
                sb.AppendLine("				<td width=\'90\'>");
                sb.AppendLine("				自动同意规则:");
                sb.AppendLine("				</td>");
                sb.AppendLine("				<td width=\'150\'>");
                sb.AppendLine(string.Format("				{0}", Flow.ConvertDisplayName_JumpIfRoute(step.JumpIfSameOwnerWithInitiator, step.JumpIfSameOwnerWithPrevStep, step.JumpIfProcessed)));
                sb.AppendLine("				</td>");
                sb.AppendLine("			</tr>");
            }
        }
        /// <summary>
        /// 自由流
        /// </summary>
        /// <param name="processNode"></param>
        private void FreeRoutingNode(ProcessNode processNode)
        {
            FreeRoutingNode node = Flow.ConvertNode<FreeRoutingNode>(processNode);
            sb.AppendLine("			</tr>");
            sb.AppendLine("				<td colspan='2'>");
            sb.AppendLine("				处理人由以下步骤指定:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td colspan=\'4\'>");
            sb.AppendLine(string.Format("				{0}", Flow.ConvertDisplayName_BPMObjectNameCollection(node.ParticipantDeclare.RecpDeclareSteps)));
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");

            sb.AppendLine("			</tr>");
            sb.AppendLine("				<td>");
            sb.AppendLine("				人员选择:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td colspan=\'5\'>");
            sb.AppendLine(string.Format("				{0}", Flow.ConvertDisplayName_MultiRecipient(node.ParticipantDeclare.MultiRecipient)));
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");

            sb.AppendLine("			</tr>");
            sb.AppendLine("				<td>");
            sb.AppendLine("				处理路由:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td colspan=\'5\'>");
            sb.AppendLine(string.Format("				{0}", Flow.ConvertDisplayName_ConsignRoutingType(node.ParticipantDeclare.RoutingType)));
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");

            sb.AppendLine("			</tr>");
            sb.AppendLine("				<td colspan='2'>");
            sb.AppendLine("				不选择处理人:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td colspan=\'4\'>");
            sb.AppendLine(string.Format("				{0}", Flow.ConvertDisplayName_JumpIfNoParticipants(node.ParticipantDeclare.JumpIfNoParticipants)));
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");

        }
        #endregion

        /// <summary>
        /// 数据控制
        /// </summary>
        /// <param name="node"></param>
        private void NodeDataControl(HumanProcessNode node)
        {
            string nodeName = node.Name;
            string activeNode = node.FlowElementType.ToString();
            FlowDataTableCollection fdtc = node.ControlDataSet.Tables;
            for (int i = 0; i < global_fdts.Count; i++)
            {
                FlowDataTable cur_fdt;
                if ((fdtc.Where(p => p.TableName == global_fdts[i].TableName)).Count() > 0)
                    cur_fdt = fdtc.Find(p => p.TableName == global_fdts[i].TableName);
                else
                    cur_fdt = new FlowDataTable();
                if (global_fdts[i].Columns.Count > 0)//对字段做控制了
                {
                    NodeDataControl(node, cur_fdt, global_fdts[i]);
                }
            }
        }
        /// <summary>
        /// 数据控制表字段填充
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cur_fdt"></param>
        /// <param name="global_fdt"></param>
        private void NodeDataControl(HumanProcessNode node, FlowDataTable cur_fdt, FlowDataTable global_fdt)
        {
            sb.AppendLine("	<table>");
            sb.AppendLine("		<tbody>");
            sb.AppendLine("			<tr class=\'NodeInfoHead\'>");
            sb.AppendLine("				<td colspan=\'6\'>");
            sb.AppendLine(string.Format("					节点名:【{0}】表名:{1}[{2}]-{3}", TypeCodeConvert.ToString(node.Name), global_fdt.TableName, global_fdt.DataSourceName, Flow.GetRepeatleableTableDescription(cur_fdt)));
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");
            this.NodeInitCreateRecordSet(node, cur_fdt.TableName);
            this.NodeControlDataSetFilter(node, cur_fdt.TableName);
            sb.AppendLine("			<tr class=\'NodeInfoBody\'>");
            sb.AppendLine("				<td width=\'150\' align=\'center\'>");
            sb.AppendLine("					字段");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'250\' align=\'center\'>");
            sb.AppendLine("					初始值");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'250\' align=\'center\'>");
            sb.AppendLine("					提交时");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td width=\'50\' align=\'center\'>");
            sb.AppendLine("					读");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td align=\'center\'>");
            sb.AppendLine("					写");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td align=\'center\'>");
            sb.AppendLine("					痕迹");
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");
            //填充字段信息
            for (int i = 0; i < global_fdt.Columns.Count; i++)
            {
                if (Flow.IsColumnInAnotherFlowDataTable(global_fdt.Columns[i].ColumnName, cur_fdt))
                {
                    //不存在，全黑
                    sb.AppendLine("			<tr class=\'TRView\'>");
                    sb.AppendLine("				<td>");
                    sb.AppendLine(string.Format("					{0}", global_fdt.Columns[i].ColumnName));
                    sb.AppendLine("				</td>");
                    sb.AppendLine("				<td>");
                    sb.AppendLine(string.Format("					<span class=\'AllowWriteTrue\'>{0}</span>", ""));
                    sb.AppendLine("				</td>");
                    sb.AppendLine("				<td>");
                    sb.AppendLine(string.Format("					<span class=\'AllowWriteFalse\'>{0}</span>", ""));
                    sb.AppendLine("				</td>");
                    sb.AppendLine("				<td>");
                    sb.AppendLine("					<span class=\'AllowReadTrue\'>√</span>");
                    sb.AppendLine("				</td>");
                    sb.AppendLine("				<td>");
                    sb.AppendLine("					<span class=\'AllowWriteTrue\'>√</span>");
                    sb.AppendLine("				</td>");
                    sb.AppendLine("				<td>");
                    sb.AppendLine("					<span class=\'ShowSpoorFalse\'>X</span>");
                    sb.AppendLine("				</td>");
                    sb.AppendLine("			</tr>");
                }
                else
                {
                    FlowDataColumn fdc = cur_fdt.Columns[global_fdt.Columns[i].ColumnName];
                    sb.AppendLine("			<tr class=\'TRView\'>");
                    sb.AppendLine("				<td>");
                    sb.AppendLine(string.Format("					{0}", fdc.ColumnName));
                    sb.AppendLine("				</td>");
                    sb.AppendLine("				<td>");
                    object defaultValue = fdc.DefaultValue;//初始值
                    sb.AppendLine(string.Format("					<span class=\'AllowWriteFalse\'>{0}</span>", TypeCodeConvert.ToString(defaultValue)));
                    sb.AppendLine("				</td>");
                    sb.AppendLine("				<td>");
                    object saveValue = fdc.SaveValue;//提交时
                    sb.AppendLine(string.Format("					<span class=\'AllowWriteFalse\'>{0}</span>", TypeCodeConvert.ToString(saveValue)));
                    sb.AppendLine("				</td>");
                    sb.AppendLine("				<td>");
                    Boolean allowRead = fdc.AllowRead;//读
                    if (allowRead)
                        sb.AppendLine("					<span class=\'AllowReadTrue\'>√</span>");
                    else
                        sb.AppendLine("					<span class=\'AllowReadFalse\'>X</span>");
                    sb.AppendLine("				</td>");
                    sb.AppendLine("				<td>");
                    Boolean allowWrite = fdc.AllowWrite;//写
                    if (allowWrite)
                        sb.AppendLine("					<span class=\'AllowWriteTrue\'>√</span>");
                    else
                        sb.AppendLine("					<span class=\'AllowWriteFalse\'>X</span>");
                    sb.AppendLine("				</td>");
                    sb.AppendLine("				<td>");
                    Boolean showSpoor = fdc.ShowSpoor;//痕迹
                    if (showSpoor)
                        sb.AppendLine("					<span class=\'ShowSpoorTrue\'>√</span>");
                    else
                        sb.AppendLine("					<span class=\'ShowSpoorFalse\'>X</span>");
                    sb.AppendLine("				</td>");
                    sb.AppendLine("			</tr>");
                }
            }
            sb.AppendLine("	</table>");
        }
        /// <summary>
        /// 进入本步骤时自动创建以下数据记录
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cur_ftablename"></param>
        private void NodeInitCreateRecordSet(HumanProcessNode node, string cur_ftablename)
        {
            var init_fdts = node.InitCreateRecordSet.Tables.FindAll(p => p.TableName == cur_ftablename);
            if (init_fdts.Count == 0) return;
            sb.AppendLine("			</tr>");
            sb.AppendLine("				<td colspan='2'>");
            sb.AppendLine("				进入本步骤时自动创建以下数据记录:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td colspan=\'4\'>");
            foreach (FlowDataTable init_fdt in init_fdts)
            {
                sb.AppendLine($"				何时创建:{Flow.ConvertDisplayName_CreateRecordType(init_fdt.CreateRecordType)}</br>");
                for (int i = 0; i < init_fdt.Columns.Count; i++)
                {
                    sb.AppendLine(string.Format("				{0}{1}", init_fdt.Columns[i].ValueString, "</br>"));
                }
            }
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");
        }
        /// <summary>
        /// 数据过滤
        /// </summary>
        /// <param name="fdt"></param>
        /// <returns></returns>
        private void NodeControlDataSetFilter(HumanProcessNode node, string cur_ftablename)
        {
            var ctl_fdt = node.ControlDataSet.Tables.Find(p => p.TableName == cur_ftablename);
            if (ctl_fdt == null || ctl_fdt.Columns.Count == 0) return;
            sb.AppendLine("			</tr>");
            sb.AppendLine("				<td colspan='2'>");
            sb.AppendLine("				数据过滤:");
            sb.AppendLine("				</td>");
            sb.AppendLine("				<td colspan=\'4\'>");
            for (int i = 0; i < ctl_fdt.Columns.Count; i++)
            {
                if (ctl_fdt.Columns[i].FilterValue != null)
                    sb.AppendLine(string.Format("				{0}{1}{2}", i == 0 ? string.Empty : "</br>", ctl_fdt.Columns[i].ValueString, ctl_fdt.Columns[i].FilterValue.ToString()));
            }
            sb.AppendLine("				</td>");
            sb.AppendLine("			</tr>");
        }

        private void HtmlHead()
        {
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta http-equiv=\'X-UA-Compatible\' content=\'IE=8\' charset=\'UTF-8\' />");
            sb.AppendLine(string.Format("    <title>{0}</title> <style>", bpmProcess.Name));
            sb.AppendLine("    BODY {FONT-SIZE: 12px; FONT-FAMILY: verdana}");
            sb.AppendLine("    TABLE {");
            sb.AppendLine("		border:1px solid black;");
            sb.AppendLine("        border-collapse: collapse;");
            sb.AppendLine("        FONT-SIZE: 12px;");
            sb.AppendLine("        FONT-FAMILY: verdana;");
            sb.AppendLine("        width: 800px;");
            sb.AppendLine("    }");
            sb.AppendLine("	TD{");
            sb.AppendLine("		border:1px solid black;");
            sb.AppendLine("	}");
            sb.AppendLine("    .AllowWriteTrue {");
            sb.AppendLine("        color:black;");
            sb.AppendLine("    }");
            sb.AppendLine("    .AllowWriteFalse {");
            sb.AppendLine("        color:red;");
            sb.AppendLine("    }");
            sb.AppendLine("    .AllowReadTrue {");
            sb.AppendLine("        color:black;");
            sb.AppendLine("    }");
            sb.AppendLine("    .AllowReadFalse {");
            sb.AppendLine("        color:red;");
            sb.AppendLine("    }");
            sb.AppendLine("    .ShowSpoorTrue {");
            sb.AppendLine("        color:red;");
            sb.AppendLine("    }");
            sb.AppendLine("    .ShowSpoorFalse {");
            sb.AppendLine("        color:black;");
            sb.AppendLine("    }");
            sb.AppendLine("    .TRView TD{");
            sb.AppendLine("        text-align:center;");
            sb.AppendLine("    }");
            sb.AppendLine("    .NodeInfoHead {");
            sb.AppendLine("        background-color:#bbe4fe;");
            sb.AppendLine("    }");
            sb.AppendLine("    .NodeInfoHead TD{");
            sb.AppendLine("        text-align:left;");
            sb.AppendLine("        height:18px;");
            sb.AppendLine("    }");
            sb.AppendLine("    .NodeInfoBody{");
            sb.AppendLine("        background-color:#f1f0f0;");
            sb.AppendLine("    }");
            sb.AppendLine("    .NodeInfoBody TD{");
            sb.AppendLine("        text-align:center;");
            sb.AppendLine("        height:18px;");
            sb.AppendLine("    }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
        }
        private void HtmlEnd()
        {
            sb.AppendLine("		</tbody>");
            sb.AppendLine("	</table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            sb.AppendLine("");
        }
    }
}
