using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Models.ViewModel;
using WorkReport.Repositories.Models;

namespace WorkReport.Interface.Automapping
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<UReport, UReportViewModel>().ReverseMap();
            CreateMap<SUser, SUserViewModel>().ReverseMap();
            CreateMap<SDepartment, SDepartmentViewModel>().ReverseMap();
        }
    }
}
