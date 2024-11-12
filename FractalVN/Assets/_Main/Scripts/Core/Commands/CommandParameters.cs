using System.Collections.Generic;
using System.Threading;

namespace COMMANDS
{
    public class CommandParameters
    {

        #region ����/Property
        private Dictionary<string, string> Parameters { get; } = new();
        private static char ID_Parameter { get; } = '/';
        #endregion
        #region ����/Method
        public CommandParameters(string[] parameterArray)
        {
            for (int i = 0; i < parameterArray.Length; Interlocked.Increment(ref i))
            {
                if (parameterArray[i].StartsWith(ID_Parameter))
                {
                    string pName = parameterArray[i];
                    string pValue = "";
                    //ȷ��������ֵ��ȷ���ǿ��ҷǱ�ʶ����
                    if (i + 1 < parameterArray.Length && !parameterArray[i + 1].StartsWith(ID_Parameter))
                    {
                        pValue = parameterArray[i + 1];
                        Interlocked.Increment(ref i);
                    }
                    Parameters.Add(pName, pValue);
                }
            }
        }
        public bool TryGetValue<T>(string parameterName, out T value, T defaultValue = default(T)) => TryGetValue(new string[] { parameterName }, out value, defaultValue);
        /// <summary>
        /// ��ȡ�ı�ָ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterNames"></param>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool TryGetValue<T>(string[] parameterNames, out T value, T defaultValue = default(T))
        {
            foreach(string parameterName in parameterNames)
            {
                if(Parameters.TryGetValue(parameterName,out string parameterValue))//һ������Ĵ��η����������ز���ֵ��ʹ��out�ؼ��ַ���ָ������
                {
                    if(TryCastParameter(parameterValue,out value))
                    {
                        return true;
                    }
                }
            }
            value = defaultValue;
            return false;
        }
        private bool TryCastParameter<T>(string parameterValue,out T value)
        {
            if (typeof(T) == typeof(bool))
            {
                if (bool.TryParse(parameterValue, out bool boolValue))
                {
                    //ǿ��ת������
                    value = (T)(object)boolValue;
                    return true;
                }
            }
            else if (typeof(T) == typeof(int))
            {
                if (int.TryParse(parameterValue, out int intValue))
                {
                    //ǿ��ת������
                    value = (T)(object)intValue;
                    return true;
                }
            }
            else if (typeof(T) == typeof(float))
            {
                if (float.TryParse(parameterValue, out float floatValue))
                {
                    //ǿ��ת������
                    value = (T)(object)floatValue;
                    return true;
                }
            }
            else if (typeof(T) == typeof(string))
            {
                value = (T)(object)parameterValue;
                return true;
            }
            value = default;
            return false;
        }
        #endregion
    }
}