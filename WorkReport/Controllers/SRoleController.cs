using Microsoft.AspNetCore.Mvc;
using WorkReport.Commons.Api;
using WorkReport.Commons.Enums;
using WorkReport.Commons.Extensions;
using WorkReport.Commons.MvcResult;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;

namespace WorkReport.Controllers
{
    public class SRoleController : Controller
    {

        private readonly ISRoleService _ISRoleService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iCommentService"></param>
        public SRoleController(ISRoleService ISUserService)
        {
            _ISRoleService = ISUserService;
        }

        #region 基础增删改查

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取全部的权限分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSRole(BaseQuery baseQuery)
        {
            var rsult = _ISRoleService.GetSRole(baseQuery);
            return new JsonResult(rsult.Data);
        }

        /// <summary>
        /// 获取全部的权限列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSRoleList()
        {
            var rsult = _ISRoleService.GetSRoleList();
            return new JsonResult(rsult);
        }


        /// <summary>
        /// 编辑信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSRoleByID(int ID)
        {
            SRole sRole = _ISRoleService.Find<SRole>(ID);
            return new JsonResult(sRole);
        }

        /// <summary>
        /// 添加修改方法
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveSRole([FromForm] SRole sRole, [FromForm] string[] menuIDs)
        {
            HttpResponseCode doResult = HttpResponseCode.Failed;

            try
            {
                if (sRole != null && sRole.ID > 0)
                {
                    _ISRoleService.Update(sRole);
                }
                else
                {
                    _ISRoleService.Insert(sRole);
                }

                SaveRolePermissions(sRole.ID, menuIDs);    //添加修改权限

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
        public IActionResult DeleteSRole(int ID)
        {
            _ISRoleService.Delete<SRole>(ID);

            //查询数据库当前角色的所有权限。
            var SRolePermissionsListFromDB = _ISRoleService.Query<SRolePermissions>(r => r.RoleID == ID).ToList();

            _ISRoleService.Delete<SRolePermissions>(SRolePermissionsListFromDB);

            return Json(new HttpResponseResult()
            {
                Msg = "删除成功",
                Code = HttpResponseCode.Success
            });
        }

        #endregion

        /// <summary>
        /// 获取全部的菜单
        /// </summary>
        /// <param name="query">分页需要的参数和关键字</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSRoleMenu(BaseQuery baseQuery, int? RoleID)
        {
            var rsult = _ISRoleService.GetSRoleMenu(baseQuery, RoleID);     //未传ID加载全部菜单，传递的话，获取勾选的菜单项。
            return new JsonResult(rsult.Data);
        }

        /// <summary>
        /// 添加角色对应的菜单
        /// </summary>
        /// <returns></returns>
        public bool SaveRolePermissions(int? RoleID, string[] menuIDs)
        {
            bool result = false;

            List<SRolePermissions> roles = new List<SRolePermissions>(menuIDs.Length);  //待添加的所有权限
            foreach (var menuID in menuIDs)
            {
                roles.Add(new SRolePermissions() { MenuID = menuID.ToInt(), RoleID = RoleID });
            }

            //查询数据库当前角色的所有权限。
            var SRolePermissionsListFromDB = _ISRoleService.Query<SRolePermissions>(r => r.RoleID == RoleID).ToList();

            var insertList = roles.Where(r => !SRolePermissionsListFromDB.Any(s => s.MenuID == r.MenuID)).ToList();  //待添加与现存差集，进行添加操作
            _ISRoleService.Insert<SRolePermissions>(insertList);

            //var delList = SRolePermissionsListFromDB.Except(roles).ToList();  //现存与待添加差集，进行删除操作
            var delList = SRolePermissionsListFromDB.Where(r => !roles.Any(s => s.MenuID == r.MenuID)).ToList();  //现存与待添加差集，进行删除操作
            _ISRoleService.Delete<SRolePermissions>(delList);

            return result;
        }
    }
}
