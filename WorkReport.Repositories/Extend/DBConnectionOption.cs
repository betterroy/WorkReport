using System;
using System.Collections.Generic;
using System.Text;

namespace WorkReport.Repositories.Extend
{
    /// <summary>
    /// 读取链接
    /// </summary>
    public class DBConnectionOption
    {
        public string WriteConnection { get; set; }
        public List<string> ReadConnectionList { get; set; }
    }
}
