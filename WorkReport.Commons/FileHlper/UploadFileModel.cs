using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.FileHlper
{
    /// <summary>
    /// 上传文件 返回数据模型
    /// </summary>
    public class UploadFileModel
    {
        /// <summary>
        /// 目录名称
        /// </summary>
        public string catalog { set; get; }
        /// <summary>
        /// 文件名称，包括扩展名
        /// </summary>
        public string fileName { set; get; }
        /// <summary>
        /// 原始文件名,包括扩展名
        /// </summary>
        public string originalName { get; set; }
        /// <summary>
        /// 文件大小（KB）
        /// </summary>
        public long fileSize { get; set; }
        /// <summary>
        /// 浏览路径
        /// </summary>
        public string url { set; get; }
    }
}
