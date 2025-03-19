namespace COMMANDS
{
    /// <summary>
    /// 指令数据库扩展器
    /// </summary>
    public abstract class DatabaseExtention
    {
        public static void Extend(CommandDatabase database) { }
        public static CommandParameters ConvertDataToParameters(string[] data) => new(data);
    }
}