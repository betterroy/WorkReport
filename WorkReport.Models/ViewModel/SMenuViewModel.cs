using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Extensions;

namespace WorkReport.Models.ViewModel
{
    public class SMenuViewModel
    {
        public SMenuViewModel()
        {
            this.SubSMenu = new List<SMenuViewModel>();
        }
        public int? ID { get; set; }
        public string Name { get; set; }
        public int? PID { get; set; }
        public string Url { get; set; }
        public int? Sort { get; set; }
        public List<SMenuViewModel> SubSMenu { get; set; }

        public void AddChilrden(SMenuViewModel node)
        {
            if (SubSMenu == null)
                SubSMenu = new List<SMenuViewModel>();
            this.SubSMenu.Add(node);
        }

    }

    /// <summary>
    /// 临时搭建
    /// </summary>
    public static class SMenuViewModelToTree
    {
        /// <summary>
        /// 临时搭建
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public static List<SMenuViewModel> ToDo(List<SMenuViewModel> models)
        {
            var dtoMap = new Dictionary<int, SMenuViewModel>();
            foreach (var item in models)
            {
                dtoMap.Add(item.ID.ToInt(), item);
            }
            List<SMenuViewModel> result = new List<SMenuViewModel>();
            foreach (var item in dtoMap.Values)
            {
                if (item.PID == 0)
                {
                    result.Add(item);
                }
                else
                {
                    if (dtoMap.ContainsKey(item.PID.ToInt()))
                    {
                        dtoMap[item.PID.ToInt()].AddChilrden(item);
                    }
                }
            }
            return result;
        }
    }
}
