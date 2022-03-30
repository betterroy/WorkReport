using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Api;
using WorkReport.Commons.Extensions;
using WorkReport.Commons.MvcResult;
using WorkReport.Models.ViewModel;
using WorkReport.Interface.IService;
using WorkReport.Models.Query;
using WorkReport.Repositories;
using WorkReport.Repositories.Extend;
using WorkReport.Repositories.Models;

namespace WorkReport.Services
{
    public class UReportService : BaseService, IUReportService
    {
        private readonly IMapper _iMapper;
        private readonly ISUserService _iSUserService;
        public UReportService(ICustomDbContextFactory dbContextFactory, ISUserService iSUserService, IMapper mapper) : base(dbContextFactory)
        {
            _iMapper = mapper;
            _iSUserService = iSUserService;
        }

        public HttpResponseResult GetUReport(UReportPageQuery queryWhere)
        {
            var res = Query<UReport>(c => c.CreateTime >= queryWhere.stime && c.CreateTime <= queryWhere.etime && (c.UserId == queryWhere.userID || queryWhere.userID == null))
                .OrderByDescending(u => u.ReportTime).ThenByDescending(u => u.CreateTime)
                .Select(a => new
                {
                    a.UserId,
                    a.CreateTime,
                    a.Content,
                    a.ID,
                    a.ReportTime,
                    a.User.Name
                });
            return new HttpResponseResult() { Data = res };
        }

        public HttpResponseResult GetUReportLinq(UReportPageQuery queryWhere)
        {
            #region demo

            //var db1 = base.GetReadContext();
            ////db.Set<SUser>()可改成Set<SUser>
            //var res = (from a in Set<SUser>()
            //           join u in db1.Set<UReport>() on a.ID equals u.UserId
            //           where u.ReportTime >= DateTime.Now.AddDays(-3)
            //           select new
            //           {
            //               a.ID
            //           }).ToList();
            //return null;

            #endregion

            var db = (WorkReportContext)DbContextFactory.ConnWriteOrRead(WriteAndReadEnum.Read);
            var res = (from a in db.UReports
                       join u in db.SUsers on a.UserId equals u.ID
                       where a.UserId == queryWhere.userID && a.CreateTime >= queryWhere.stime && a.CreateTime <= queryWhere.etime.AddDays(1)
                       select new
                       {
                           a.UserId,
                           a.CreateTime,
                           a.Content,
                           a.ID,
                           u.Name
                       }).ToList();
            return new HttpResponseResult() { Data = res };
        }

        public HttpResponseResult GetUReportPage(UReportPageQuery queryWhere)
        {

            Expression<Func<UReport, bool>> expressionWhere = c => c.CreateTime >= queryWhere.stime && c.CreateTime <= queryWhere.etime.AddDays(1) && c.UserId == queryWhere.userID;
            Expression<Func<UReport, DateTime>> expressionOrder = c => c.CreateTime;

            PageResult<UReport> pageResult = QueryPage<UReport, DateTime>(expressionWhere, queryWhere.limit, queryWhere.page, expressionOrder, false);
            PageResult<UReportViewModel> result = new PageResult<UReportViewModel>()
            {
                page = pageResult.page,
                limit = pageResult.limit,
                count = pageResult.count,
                data = _iMapper.Map<List<UReport>, List<UReportViewModel>>(pageResult.data)
            };
            return new HttpResponseResult() { Data = result };
        }

        /// <summary>
        /// 获取部门下的所有用户。
        /// </summary>
        /// <returns></returns>
        public List<UReportUserViewModel> GetUReportWriteStatus()
        {
            var stime = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd").ToDate();

            List<DepartmentAndUsersViewModel> departmentAndUsersViewModels = _iSUserService.GetUserOfDepartment();  //获取部门及归属用户

            var uReports = Query<UReport>(u => u.ReportTime >= stime).Select(s => new { s.UserId, s.ReportTime }).ToList(); //获取设定日期内填的日志人。

            var disTimes = uReports.Select(u => u.ReportTime).Distinct().ToList();    //填报日志的不重复时间。

            List<UReportUserViewModel> uReportUserViewModels = new List<UReportUserViewModel>();    //返回的数据


            for (int j = 0; j < departmentAndUsersViewModels.Count(); j++)
            {
                DepartmentAndUsersViewModel d = departmentAndUsersViewModels[j];     //部门及归属用户

                for (int i = 0; i < disTimes.Count(); i++)
                {
                    var uReportFor = uReports.Where(u => u.ReportTime == disTimes[i]).ToList();  //获取当天所填写的用户

                    UReportUserViewModel uReportUserViewModel = new UReportUserViewModel();
                    uReportUserViewModel.ReportTime = disTimes[i];
                    uReportUserViewModel.Department = d.SDepartment.DeptName;

                    var listUsers = d.Users.Where(u => uReportFor.Where(f => f.UserId == u.ID).Any()).ToList();  //获取写日志的用户

                    uReportUserViewModel.UserHadWrite = d.Users.Intersect(listUsers).Select(u => u.Name).ToList();    //求交集
                    uReportUserViewModel.UserUnWrite = d.Users.Except(listUsers).Select(u => u.Name).ToList();      //求差集

                    uReportUserViewModels.Add(uReportUserViewModel);
                }
            }
            uReportUserViewModels.Sort((a, b) => { return b.ReportTime.CompareTo(a.ReportTime); });
            return uReportUserViewModels.Where(u=>u.UserHadWrite.Count()>0).ToList();

        }
    }
}
