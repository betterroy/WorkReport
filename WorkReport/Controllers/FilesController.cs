using Microsoft.AspNetCore.Mvc;
using WorkReport.Commons.Api;
using WorkReport.Commons.FileHlper;

namespace WorkReport.Controllers
{
    public class FilesController : Controller
    {

        private readonly Microsoft.Extensions.Configuration.IConfiguration _IConfiguration = null;

        public FilesController(Microsoft.Extensions.Configuration.IConfiguration iConfiguration)
        {
            this._IConfiguration = iConfiguration;
        }

        public IActionResult Index()
        {
            return View();
        }

        //上传excel，参见SUser中UploadExcel方法；使用miniexcel。

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UploadFile()
        {
            IFormFileCollection files = Request.Form.Files;
            string savePath = string.Empty;
            if (files != null && files.Count != 0)
            {
                IFormFile file = files.FirstOrDefault();

                //获取配置文件的文件地址
                var fileAddressSection = _IConfiguration.GetSection("FileAddress");
                if (fileAddressSection == null)
                {
                    return new JsonResult(new HttpResponseResult()
                    {
                        Msg = "未配置文件保存地址"
                    });
                }

                string filePath = $"{fileAddressSection.Value}/{DateTime.Now.ToString("yyyyMMdd")}";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string suffix = string.Empty;  //后缀名
                string[] array = file.FileName.Split('.');
                if (array != null && array.Length > 0)
                {
                    suffix = array[array.Length - 1];
                }
                savePath = Path.Combine(filePath, $"{Guid.NewGuid()}.{suffix}");
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return new JsonResult(new HttpResponseResult()
                {
                    Code = HttpResponseCode.Success,
                    Data = savePath
                });
            }
            return new JsonResult(new HttpResponseResult()
            {
                Code = HttpResponseCode.BadRequest,
                Data = savePath
            });
        }

        /// <summary>
        /// 下载本地文件
        /// </summary>
        /// <param name="fileName">文件名:(eg:Files/用户信息模板.xlsx)</param>
        /// <returns></returns>
        public async Task<IActionResult> DownLoadFile(string fileName)
        {
            var directory = Environment.CurrentDirectory;

            var filePath = $@"{directory}\wwwroot\{fileName}";

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                throw new Exception("文件不存在");
            }
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            //文件名必须编码，否则会有特殊字符(如中文)无法在此下载。
            string encodeFilename = Path.GetFileName(fileName);
            encodeFilename = System.Web.HttpUtility.UrlEncode(encodeFilename, System.Text.Encoding.GetEncoding("UTF-8"));
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + encodeFilename);
            return new FileStreamResult(memoryStream, MimeMappingHelper.GetMimeMapping(filePath));//文件流方式，指定文件流对应的
        }

    }
}
