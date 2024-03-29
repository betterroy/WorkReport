﻿using Microsoft.AspNetCore.Mvc;
using System.Text;
using WorkReport.Commons.Api;
using WorkReport.Commons.EncryptHelper;
using WorkReport.Commons.FileHelper;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;

namespace WorkReport.Controllers
{
    public class SFilesController : Controller
    {

        private readonly Microsoft.Extensions.Configuration.IConfiguration _IConfiguration = null;
        private readonly ISFilesService _ISFilesService;

        public SFilesController(Microsoft.Extensions.Configuration.IConfiguration iConfiguration, ISFilesService ISFilesService)
        {
            this._IConfiguration = iConfiguration;
            this._ISFilesService = ISFilesService;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region 增删改查

        /// <summary>
        /// 获取全部的文件分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSFiles(BaseQuery baseQuery)
        {
            var rsult = _ISFilesService.GetSFiles(baseQuery);
            return new JsonResult(rsult.Data);
        }

        /// <summary>
        /// 获取全部的文件列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSFilesList()
        {
            var rsult = _ISFilesService.GetSFilesList();
            return new JsonResult(rsult);
        }


        /// <summary>
        /// 编辑信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSFilesByID(int ID)
        {
            SFiles sDepartment = _ISFilesService.Find<SFiles>(ID);
            return new JsonResult(sDepartment);
        }

        /// <summary>
        /// 添加修改方法
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveSFiles([FromBody] SFiles uReport)
        {
            HttpResponseCode doResult = HttpResponseCode.Failed;

            try
            {
                if (uReport != null && uReport.ID > 0)
                {
                    _ISFilesService.Update(uReport);
                }
                else
                {
                    _ISFilesService.Insert(uReport);
                }
                doResult = HttpResponseCode.Success;
            }
            catch (Exception ex)
            {
                doResult = HttpResponseCode.Failed;
            }

            return Json(new HttpResponseResult()
            {
                Msg = "保存成功",
                Code = doResult
            });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteSFiles(int ID)
        {
            _ISFilesService.Delete<SFiles>(ID);
            return Json(new HttpResponseResult()
            {
                Msg = "删除成功",
                Code = HttpResponseCode.Success
            });
        }

        #endregion

        #region 文件上传下载

        //上传excel，参见SUser中UploadExcel方法；使用miniexcel。

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UploadFile()
        {
            IFormFileCollection files = Request.Form.Files;
            if (files != null && files.Count != 0)
            {
                IFormFile file = files.FirstOrDefault();
                var fileStream = file.OpenReadStream();

                var _MD5Code = MD5Encrypt.AbstractFile(fileStream);
                var sFileWhere = _ISFilesService.Query<SFiles>(f => f.FileName == file.FileName && f.MD5Code == _MD5Code).FirstOrDefault(); //查询是否有相同文件，如有，则不上传
                if (sFileWhere != null)
                {
                    return new JsonResult(new HttpResponseResult()  //不给Code默认Success
                    {
                        Data = sFileWhere.FilePath
                    });
                }

                string suffix = string.Empty;  //后缀名
                string[] array = file.FileName.Split('.');
                if (array != null && array.Length > 0)
                {
                    suffix = array[array.Length - 1];
                }

                var uploadFileModel = GetFilePath();

                var fileName = $"{Guid.NewGuid()}.{suffix}";
                string savePath = Path.Combine($@"{uploadFileModel.catalog}\{uploadFileModel.url}", fileName);      //实际落盘位置
                string urlPath = Path.Combine(uploadFileModel.url, fileName);           //前台读取文件url
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                SFiles sFile = new SFiles();   //组装数据添加至数据库中。
                sFile.FileName = file.FileName;
                sFile.FilePath = urlPath;
                sFile.MD5Code = _MD5Code;  //存储文件的MD5 
                sFile.CreateTime = DateTime.Now;
                sFile.Sort = 1;
                sFile.IsDel = false;

                _ISFilesService.Insert(sFile);

                return new JsonResult(new HttpResponseResult()  //不给Code默认Success
                {
                    Data = savePath
                });
            }
            return new JsonResult(new HttpResponseResult()
            {
                Code = HttpResponseCode.BadRequest,
                Data = ""
            });
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <returns></returns>
        private UploadFileModel GetFilePath()
        {
            UploadFileModel uploadFileModel = new UploadFileModel();

            var url = _IConfiguration.GetSection("FileAddress").Value ?? "FileResource";    //url
            uploadFileModel.url = @$"{url}\{DateTime.Now.ToString("yyyyMMdd")}";

            var catalog = @$"{uploadFileModel.catalog}\{uploadFileModel.url}";
            if (!Directory.Exists(catalog))
            {
                Directory.CreateDirectory(catalog);
            }
            return uploadFileModel;
        }

        /// <summary>
        /// 下载本地文件
        /// </summary>
        /// <param name="fileName">文件名:用户信息模板.xlsx</param>
        /// <param name="fileName">文件名:FileResource\20220401\ddd7d156-6a80-42fc-b573-86f785211664.png</param>
        /// <returns></returns>
        public async Task<IActionResult> DownLoadFile(string fileName, string filePath)
        {
            UploadFileModel uploadFileModel = new UploadFileModel();

            var fileCatalog = $@"{uploadFileModel.catalog}{filePath}";

            if (string.IsNullOrEmpty(fileCatalog) || !System.IO.File.Exists(fileCatalog))
            {
                throw new Exception("文件不存在");
            }
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(fileCatalog, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            //文件名必须编码，否则会有特殊字符(如中文)无法在此下载。
            //string encodeFilename = Path.GetFileName(fileName);
            string encodeFilename = fileName;
            encodeFilename = System.Web.HttpUtility.UrlEncode(encodeFilename, System.Text.Encoding.GetEncoding("UTF-8"));
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + encodeFilename);
            return new FileStreamResult(memoryStream, MimeMappingHelper.GetMimeMapping(fileCatalog));//文件流方式，指定文件流对应的
        }

        #endregion

        #region word excel转为html返回前台


        public async Task<IActionResult> OpenOfficeToHtml(string fileName, string filePath)
        {
            UploadFileModel uploadFileModel = new UploadFileModel();

            var fileCatalog = $@"{uploadFileModel.catalog}{filePath}";  //文件存储路径

            if (string.IsNullOrEmpty(fileCatalog) || !System.IO.File.Exists(fileCatalog))
            {
                throw new Exception("文件不存在");
            }

            string suffix = string.Empty;  //后缀名
            string[] array = fileName.Split('.');
            if (array != null && array.Length > 0)
            {
                suffix = array[array.Length - 1].ToLower().Substring(0, 3);
            }
            string convertToHtml = string.Empty;

            if (string.Equals(suffix, "doc", StringComparison.CurrentCultureIgnoreCase))   //为word时
            {
                convertToHtml = ce.office.extension.WordHelper.ToHtml(fileCatalog);
            }
            else if (string.Equals(suffix, "xls", StringComparison.CurrentCultureIgnoreCase))  //为excel时
            {
                convertToHtml = ce.office.extension.ExcelHelper.ToHtml(fileCatalog);
            }

            return Content(convertToHtml);
        }

        public async Task<IActionResult> OpenTxtToHtml(string fileName, string filePath)
        {

            UploadFileModel uploadFileModel = new UploadFileModel();

            var fileCatalog = $@"{uploadFileModel.catalog}{filePath}";  //文件存储路径

            if (string.IsNullOrEmpty(fileCatalog) || !System.IO.File.Exists(fileCatalog))
            {
                throw new Exception("文件不存在");
            }

            StringBuilder convertToHtml = new StringBuilder();
            using (StreamReader sr = new StreamReader(fileCatalog, Encoding.GetEncoding("gb2312"))) //模板页路径
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    //为空就为换行
                    if (string.IsNullOrEmpty(line))
                    {
                        line = "<br/>";
                    }
                    convertToHtml.Append($"<div>{line}</div>");
                }
                sr.Close();
            }
            return Content(convertToHtml.ToString());
        }
        #endregion
    }
}
