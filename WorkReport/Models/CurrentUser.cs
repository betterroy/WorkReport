using System.ComponentModel.DataAnnotations;
using WorkReport.Repositories.Models;

namespace WorkReport.Models
{
    public static class CurrentUser
    {
        public static SUser Value { get; set; }

    }
}
