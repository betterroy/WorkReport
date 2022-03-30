using System;
using System.Collections.Generic;
using System.Text;

namespace WorkReport.Repositories.Extend
{
    /// <summary>
    /// 枚举：确定某一次操作是使用主库还是从库
    /// </summary>
    public enum WriteAndReadEnum
    {
        Write,  //主库操作
        Read  //从库操作
    }
}
