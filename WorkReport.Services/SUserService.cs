﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Api;
using WorkReport.Commons.MvcResult;
using WorkReport.Models.ViewModel;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories.Extend;
using WorkReport.Repositories.Models;
using WorkReport.Commons.Extensions;
using WorkReport.Interface.AopExtension;
using Autofac.Extras.DynamicProxy;

namespace WorkReport.Services
{
    public class SUserService : BaseService, ISUserService
    {
        private readonly IMapper _iMapper;
        //private readonly ISMenuService _iSMenuService;
        private readonly ISRoleService _iSRoleService;
        public SUserService(ICustomDbContextFactory dbContextFactory, IMapper mapper
            //, ISMenuService iSMenuService
            , ISRoleService iSRoleService) : base(dbContextFactory)
        {
            _iMapper = mapper;
            //_iSMenuService = iSMenuService;
            _iSRoleService = iSRoleService;
        }

        /// <summary>
        /// 获取当前用户分页数据
        /// </summary>
        /// <param name="baseQuery"></param>
        /// <returns></returns>
        public HttpResponseResult GetSUser(BaseQuery baseQuery)
        {

            Expression<Func<SUser, bool>> expressionWhere = null;
            Expression<Func<SUser, int?>> expressionOrder = c => c.ID;

            PageResult<SUser> pageResult = QueryPage<SUser, int?>(expressionWhere, baseQuery.limit, baseQuery.page, expressionOrder, true);
            PageResult<SUserViewModel> result = new PageResult<SUserViewModel>()
            {
                page = pageResult.page,
                limit = pageResult.limit,
                count = pageResult.count,
                data = _iMapper.Map<List<SUser>, List<SUserViewModel>>(pageResult.data)
            };

            return new HttpResponseResult() { Data = pageResult };
        }

        /// <summary>
        /// 获取当前用户分页数据带部门
        /// </summary>
        /// <param name="baseQuery"></param>
        /// <returns></returns>
        public HttpResponseResult GetSUserAndDepartment(BaseQuery baseQuery)
        {
            //var query = baseQuery.getQuery();
            string Name = "";
            //if (query!=null)
            //{
            //    Name = query["Name"].ToString();
            //}
            if (baseQuery["Name"] != null)
            {
                Name = baseQuery["Name"].ToString();
            }
            var list = (from a in Set<SUser>()
                        join b in Set<SDepartment>() on a.DeptId equals b.DeptId
                        where a.Name == Name || Name==""
                        orderby a.ID
                        select new
                        {
                            a.ID,
                            a.UserCode,
                            a.Name,
                            a.UserType,
                            a.DeptId,
                            a.Status,
                            a.Mobile,
                            a.QQ,
                            a.WeChat,
                            a.Email,
                            a.Sex,
                            b.DeptName
                        });

            List<SUserViewModel> sUserViewModels = new List<SUserViewModel>();
            var users = list.Skip((baseQuery.page - 1) * baseQuery.limit).Take(baseQuery.limit).ToList();
            foreach (var item in users)
            {
                SUserViewModel sUserViewModel = new SUserViewModel()
                {
                    ID = item.ID,
                    UserCode = item.UserCode,
                    Name = item.Name,
                    UserType = item.UserType,
                    DeptId = item.DeptId,
                    Status = item.Status,
                    Mobile = item.Mobile,
                    QQ = item.QQ,
                    WeChat = item.WeChat,
                    Email = item.Email,
                    Sex = item.Sex,
                    DeptName = item.DeptName
                };
                var sRoleUserList = Query<SRoleUser>(s => s.UserID == item.ID);
                if (sRoleUserList.Count() > 0) {
                    var sRoleList = Query<SRole>(r => sRoleUserList.Any(list => list.RoleID == r.ID));                    
                    sUserViewModel.RoleId = string.Join(",", sRoleList.Select(s => s.RoleName));
                }
                sUserViewModels.Add(sUserViewModel);
            }

            PageResult<SUserViewModel> result = new PageResult<SUserViewModel>()
            {
                data = sUserViewModels,
                page = baseQuery.page,
                limit = baseQuery.limit,
                count = list.Count()
            };
            return new HttpResponseResult() { Data = result };
        }

        /// <summary>
        /// 获取全部的用户列表
        /// </summary>
        /// <returns></returns>
        public List<SUserViewModel> GetSUserList()
        {
            var sUsers = Set<SUser>().ToList();
            var res = _iMapper.Map<List<SUser>, List<SUserViewModel>>(sUsers);
            return res;
        }

        #region 登录

        /// <summary>
        /// 验证用户是否登录成功；
        /// 登录成功，就获取改用户的角色 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="sysUser"></param>
        /// <param name="menulist"></param>
        /// <returns></returns> 
        public virtual bool SUserLogin(string username, string password, out SUser sysUser, out List<SRoleUser> sRoleUser
            //,out List<SMenuViewModel> menueViewList
            )
        {
            IQueryable<SUser> sUser = Query<SUser>(u => u.UserCode.Equals(username)
                                                        && u.Password.Equals(password)
                                                        && (u.Status == true || u.Status == null));
            if (sUser != null && sUser.Count() > 0)
            {
                sysUser = sUser.FirstOrDefault();

                sRoleUser = _iSRoleService.GetSRoleUserByUserID(sysUser.ID.ToInt());

                //menueViewList = _iSMenuService.GetSMenuList(sysUser.ID.ToInt());   //根据当前用户获取所有菜单

                return true;
            }
            else
            {
                sysUser = null;
                sRoleUser = null;
                //menueViewList = null;
                return false;
            }
        }

        /// <summary>
        /// 获取部门下的所有用户
        /// </summary>
        /// <returns></returns>
        public List<DepartmentAndUsersViewModel> GetUserOfDepartment()
        {
            var sDepartments = Set<SDepartment>().ToList();
            var sUsers = Set<SUser>().ToList();

            List<DepartmentAndUsersViewModel> departmentAndUsersViewModels = new List<DepartmentAndUsersViewModel>();

            for (int j = 0; j < sDepartments.Count(); j++)
            {
                var val = new DepartmentAndUsersViewModel();

                val.SDepartment = _iMapper.Map<SDepartment, SDepartmentViewModel>(sDepartments[j]);

                var sUserOfsDepartment = sUsers.Where(s => s.DeptId == val.SDepartment.DeptId).ToList();
                val.Users = _iMapper.Map<List<SUser>, List<SUserViewModel>>(sUserOfsDepartment);

                departmentAndUsersViewModels.Add(val);
            }
            return departmentAndUsersViewModels;
        }

        #endregion
    }
}
