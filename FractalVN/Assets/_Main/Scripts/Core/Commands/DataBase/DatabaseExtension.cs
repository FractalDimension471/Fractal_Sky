namespace COMMANDS
{
    /// <summary>
    /// ָ�����ݿ���չ��
    /// </summary>
    public abstract class DatabaseExtention
    {
        public static void Extend(CommandDatabase database) { }
        public static CommandParameters ConvertDataToParameters(string[] data) => new(data);
    }
}