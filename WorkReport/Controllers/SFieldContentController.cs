using Microsoft.AspNetCore.Mvc;
using WorkReport.Commons.Api;
using WorkReport.Commons.MvcResult;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;

namespace WorkReport.Controllers
{
    public class SFieldContentController : Controller
    {

        private readonly ISFieldContentService _ISFieldContentService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iCommentService"></param>
        public SFieldContentController(ISFieldContentService ISUserService)
        {
            _ISFieldContentService = ISUserService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取全部目录下拉树
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSFieldCatalog(BaseQuery baseQuery)
        {
            var rsult = _ISFieldContentService.GetSFieldCatalog(baseQuery);
            return new JsonResult(rsult.Data);
        }

        /// <summary>
        /// 获取全部的分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSFieldContent(BaseQuery baseQuery)
        {
            var rsult = _ISFieldContentService.GetSFieldContent(baseQuery);
            return new JsonResult(rsult.Data);
        }

        /// <summary>
        /// 编辑字典目录信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSFieldCatalogByID(int ID)
        {
            SFieldCatalog s = _ISFieldContentService.Find<SFieldCatalog>(ID);
            return new JsonResult(s);
        }
        /// <summary>
        /// 编辑字典信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSFieldContentByID(int ID)
        {
            SFieldContent s = _ISFieldContentService.Find<SFieldContent>(ID);
            return new JsonResult(s);
        }

        /// <summary>
        /// 添加修改目录方法
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveSFieldCatalog([FromBody] SFieldCatalog sFieldCatalog)
        {
            HttpResponseCode doResult = HttpResponseCode.Failed;

            try
            {
                if (sFieldCatalog != null && sFieldCatalog.ID > 0)
                {
                    _ISFieldContentService.Update(sFieldCatalog);
                }
                else
                {
                    _ISFieldContentService.Insert(sFieldCatalog);
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
        /// 添加修改字典方法
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveSFieldContent([FromBody] SFieldContent sFieldContent)
        {
            HttpResponseCode doResult = HttpResponseCode.Failed;

            try
            {
                if (sFieldContent != null && sFieldContent.ID > 0)
                {
                    _ISFieldContentService.Update(sFieldContent);
                }
                else
                {
                    _ISFieldContentService.Insert(sFieldContent);
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
        public IActionResult DeleteSFieldCatalog(int ID)
        {
            _ISFieldContentService.Delete<SFieldCatalog>(ID);
            return Json(new HttpResponseResult()
            {
                Msg = "删除成功",
                Code = HttpResponseCode.Success
            });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteSFieldContent(int ID)
        {
            _ISFieldContentService.Delete<SFieldContent>(ID);
            return Json(new HttpResponseResult()
            {
                Msg = "删除成功",
                Code = HttpResponseCode.Success
            });
        }

    }
}
