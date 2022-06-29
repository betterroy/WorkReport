using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkReport.AuthorizationCenter.Model;
using WorkReport.Models.ViewModel;
using WorkReport.Repositories.Models;

namespace WorkReport.AuthorizationCenter.Utility
{
    public interface ICustomJWTService
    {
        TokenOption GetToken(SUser sUser);

        TokenOption GetTwoToken(SUserViewModel sUser);
    }
}
