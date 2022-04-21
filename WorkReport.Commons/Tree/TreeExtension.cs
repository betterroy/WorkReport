using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkReport.Commons.Extensions;

namespace WorkReport.Commons.Tree
{
    public class TreeExtension<T> where T : ITreeModel
    {
        public static List<T> ToDo(List<T> models)
        {
            var dtoMap = new Dictionary<int, T>();
            foreach (var item in models)
            {
                dtoMap.Add(item.id.ToInt(), item);
            }
            List<T> result = new List<T>();
            foreach (var item in dtoMap.Values)
            {
                if (item.parentid == 0)
                {
                    result.Add(item);
                }
                else
                {
                    if (dtoMap.ContainsKey(item.parentid.ToInt()))
                    {
                        dtoMap[item.parentid.ToInt()].AddChilrden(item);
                    }
                }
            }
            return result;
        }
    }
}
