using System;

namespace BPMToolApp.Common
{
    /// <summary>
    /// 类型转换
    /// </summary>
    public class TypeCodeConvert
    {
        public static string ToString(object value) 
        {
            if (value == null)
                return "";
            TypeCode typeCode = Type.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return Convert.ToBoolean(value).ToString();
                case TypeCode.DateTime:
                    return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    return Convert.ToDecimal(value).ToString();
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.Char:
                case TypeCode.String:
                    return Convert.ToString(value);
                default:
                    return ((BPM.CodeBlock)value).CodeText;
            }
        }
    }
}
