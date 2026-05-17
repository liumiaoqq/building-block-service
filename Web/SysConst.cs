namespace Web
{
    public static class SysConst
    {
        public static List<string> NotCodeFileType = new List<string>()  {
                     ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff",
                    ".mp4", ".avi", ".mov", ".wmv", ".mkv",
                    ".mp3", ".wav", ".ogg",
                    ".zip", ".rar", ".7z",
                    ".pdf",
                    ".docx", ".xlsx"
                };



        public static List<string> FilterFileType = new List<string>() { ".dll", "obj", "debug", ".idea", ".git", "target", "node_modules" };

        public static List<string> FilterFolderType = new List<string>() { ".dll", "obj", "debug", ".idea", ".git", "target", "node_modules" };
        public static string TempleteFileRootDir = "Templetes";

        public static string FolderFormat = "folder";

        public static string NoSuffix = "nosuffix";

        public static string GengerationCodeRootDir = "GengerationCode";


        /// <summary>
        /// 临时文件目录
        /// </summary>
        public static string TemporaryTempletesDir = "TemporaryTempletes";


        /// <summary>
        /// 特殊字段
        /// </summary>
        public static List<string> SpeColumnCodes = new List<string>() { "No", "Title", "Name", "OrderNo" };


        /// <summary>
        /// 文件名称不能包含的特殊字符
        /// </summary>
        public static List<string> FilterChar = new List<string>() { "+", "@", "|", "/", "\\", ".", "#", ":", ">", "<", "?", "*", "%", "&", "select", "delete", "where", "drop", "=", "$", "!" };



        /// <summary>
        /// 验证前端的header标识(请求来源)
        /// </summary>
        public const string XRequestSource = "X-Rs";
        /// <summary>
        /// 验证前端的header标识（时间戳）
        /// </summary>
        public const string XTicks = "X-Ticks";

        /// <summary>
        /// 验证前端的
        /// </summary>
        public const string TimestampWhilteUrls = "TimestampWhilteUrls";

        /// <summary>
        /// 验证前端的
        /// </summary>
        public const string RequestSourceWhilteUrls = "RequestSourceWhilteUrls";


        /// <summary>
        /// 基础的用户表体系 
        /// </summary>
        public static List<string> BaseTableNames = new List<string>() { "AppUser","User", "SysUser" };
    }
}
