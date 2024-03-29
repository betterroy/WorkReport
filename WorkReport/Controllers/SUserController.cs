﻿using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using WorkReport.Commons.Api;
using WorkReport.Commons.EncryptHelper;
using WorkReport.Commons.Extensions;
using WorkReport.Commons.MvcResult;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;
using WorkReport.Commons.Extensions;

namespace WorkReport.Controllers
{
    public class SUserController : Controller
    {

        private readonly ISUserService _ISUserService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iCommentService"></param>
        public SUserController(ISUserService ISUserService)
        {
            _ISUserService = ISUserService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取全部的用户
        /// </summary>
        /// <param name="query">分页需要的参数和关键字</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSUser(BaseQuery baseQuery)
        {
            //var rsult = _ISUserService.GetSUser(baseQuery);
            var rsult = _ISUserService.GetSUserAndDepartment(baseQuery);
            return new JsonResult(rsult.Data);
        }

        /// <summary>
        /// 获取全部的列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSUserList()
        {
            var rsult = _ISUserService.GetSUserList();
            return new JsonResult(rsult);
        }

        /// <summary>
        /// 编辑信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSUserByID(int ID)
        {
            SUser sUser = _ISUserService.Find<SUser>(ID);

            #region 往SRoleUser表中添加角色用户

            sUser.Password = "";
            List<SRoleUser> roleList = _ISUserService.Query<SRoleUser>(r => r.UserID == ID).ToList();
            var roleIds = roleList.Select(r => r.RoleID);
            sUser.RoleId = String.Join(",", roleIds);   //拼接起角色ID

            #endregion

            return new JsonResult(sUser);
        }

        /// <summary>
        /// 添加修改方法
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveSUser([FromBody] SUser sUser)
        {
            HttpResponseCode doResult = HttpResponseCode.Failed;

            try
            {
                if (sUser != null && sUser.ID > 0)
                {
                    _ISUserService.Update(sUser, it => new { it.Password });    //更新用户，密码不进行更新。
                                                                                //_ISUserService.Update(sUser, "Password");

                    #region 往SRoleUser表中添加角色用户

                    var roleIDs = sUser.RoleId.Split(",");
                    List<SRoleUser> roleList = new List<SRoleUser>(roleIDs.Length);  //待添加的所有权限
                    foreach (var roleID in roleIDs)
                    {
                        roleList.Add(new SRoleUser() { RoleID = roleID.ToInt(), UserID = sUser.ID });
                    }

                    //查询数据库当前角色的所有权限。
                    var SRoleUsersListFromDB = _ISUserService.Query<SRoleUser>(r => r.UserID == sUser.ID).ToList();

                    var insertList = roleList.Where(r => !SRoleUsersListFromDB.Any(s => s.RoleID == r.RoleID)).ToList();  //待添加与现存差集，进行添加操作
                    _ISUserService.Insert<SRoleUser>(insertList);

                    var delList = SRoleUsersListFromDB.Where(r => !roleList.Any(s => s.RoleID == r.RoleID)).ToList();  //现存与待添加差集，进行删除操作
                    _ISUserService.Delete<SRoleUser>(delList);

                    #endregion

                }
                else
                {
                    sUser.Password = sUser.UserCode + "123a.";
                    sUser.Password = MD5Encrypt.Encrypt(sUser.Password);
                    _ISUserService.Insert(sUser);
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
        public IActionResult DeleteSUser(int ID)
        {
            _ISUserService.Delete<SUser>(ID);
            return Json(new HttpResponseResult()
            {
                Msg = "删除成功",
                Code = HttpResponseCode.Success
            });
        }

        /// <summary>
        /// 上传excel
        /// </summary>
        /// <param name="excel"></param>
        /// <returns></returns>
        public IActionResult UploadExcel()
        {

            IFormFileCollection files = Request.Form.Files;
            if (files != null && files.Count != 0)
            {
                IFormFile excel = files.FirstOrDefault();
                var stream = new MemoryStream();
                excel.CopyTo(stream);

                //foreach (var item in stream.Query(true))
                //{
                //    SUser sUser = new SUser();
                //    sUser.Name = item.姓名;
                //}
                List<SUser> sUsers = new List<SUser>();
                var items = stream.Query<user>();
                foreach (var item in items)
                {
                    SUser sUser = new SUser();
                    sUser.Name = item.姓名;
                    sUser.UserCode = item.登录账号;
                    //sUser.Age = item.年龄;
                    sUser.Sex = item.性别.Substring(0,1);
                    //sUser.QQ = item.身高;
                    //sUser.QQ = item.学历;
                    sUser.Mobile = item.手机号;
                    sUser.Email = item.邮箱;
                    sUser.QQ = item.QQ.ToLong();
                    sUser.WeChat = item.微信;
                    sUser.DeptId = item.部门;
                    sUser.Password = sUser.UserCode + "123a.";
                    sUser.Password = MD5Encrypt.Encrypt(sUser.Password);

                    sUsers.Add(sUser);
                }
                _ISUserService.Insert<SUser>(sUsers);
            }

            return Json(new HttpResponseResult()
            {
                Msg = "上传用户成功",
                Code = HttpResponseCode.Success
            });
        }

        /// <summary>
        /// 下载全部用户
        /// </summary>
        /// <returns></returns>
        public IActionResult ExportUserExcel()
        {
            var values = _ISUserService.GetSUserList();
            var memoryStream = new MemoryStream();
            memoryStream.SaveAs(values);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "用户列表.xlsx"
            };
        }

        public class user
        {
            public string 姓名 { get; set; }
            public string 登录账号 { get; set; }
            public string 年龄 { get; set; }
            public string 性别 { get; set; }
            public string 学历 { get; set; }
            public string 身高 { get; set; }
            public string 邮箱 { get; set; }
            public string 手机号 { get; set; }
            public string QQ { get; set; }
            public string 微信 { get; set; }
            public string 部门 { get; set; }
        }

    }
}
