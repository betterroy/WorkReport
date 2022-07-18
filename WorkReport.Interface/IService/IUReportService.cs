using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Api;
using WorkReport.Models.ViewModel;
using WorkReport.Interface.IRepositories;
using WorkReport.Models.Query;
using WorkReport.Repositories.Models;
using Autofac.Extras.DynamicProxy;
using WorkReport.Interface.AopExtension;

namespace WorkReport.Interface.IService
{
    [Intercept(typeof(CustomAutofacUReportAop))]
    public interface IUReportService : IBaseService
    {

        public HttpResponseResult GetUReport(UReportPageQuery queryWhere);
        public HttpResponseResult GetUReportLinq(UReportPageQuery queryWhere);
        public HttpResponseResult GetUReportPage(UReportPageQuery queryWhere);
        public List<UReportUserViewModel> GetUReportWriteStatus();
    }
}
