using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkReport.Commons.Extensions
{
    /// <summary>
    /// 转换帮助类
    /// </summary>
    public class ConvertUtils
    {
        /// <summary>
        /// 把DataRow对象转换成指定类型对象
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="dr">dataRow对象</param>
        /// <returns></returns>
        public static T DataRowToEntity<T>(DataRow dr) where T : new()
        {
            if (dr == null) return default;

            var obj = new T();
            var columns = dr.Table.Columns;

            foreach (var property in obj.GetType().GetProperties())
            {
                var columnName = property.Name;
                if (!property.CanWrite) continue;
                if (!columns.Contains(columnName)) continue;

                var valueObj = dr[columnName];
                if (valueObj is DBNull) continue;
                var value = valueObj.ToString();
                try
                {
                    if (property.PropertyType.IsGenericType == false)
                    {
                        //非泛型
                        property.SetValue(obj,
                            string.IsNullOrEmpty(Convert.ToString(value))
                                ? null
                                : Convert.ChangeType(value, property.PropertyType), null);
                    }
                    else
                    {
                        //泛型Nullable<>
                        var genericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                        if (genericTypeDefinition == typeof(Nullable<>))
                        {
                            property.SetValue(obj, string.IsNullOrEmpty(Convert.ToString(value))
                                ? null
                                : Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType)), null);
                        }
                    }

                }
                catch
                {
                    // ignored
                }
            }
            return obj;
        }

        /// <summary>
        /// 把DataTable对象转换成指定类型对象列表
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="dt">DataTable对象</param>
        /// <returns></returns>
        public static List<T> DataTableToEntities<T>(DataTable dt) where T : new()
        {
            var list = new List<T>();

            if (dt == null || dt.Rows.Count <= 0) return list;

            foreach (DataRow dr in dt.Rows)
            {
                var curEntity = DataRowToEntity<T>(dr);
                list.Add(curEntity);
            }

            return list;
        }


        /// <summary>
        /// 把IDataReader对象转成实体类。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr">需要参数的方法dr.Read()返回值为true。</param>
        /// <returns></returns>
        private static T DataReaderToEntity<T>(IDataReader dr) where T : new()
        {
            if (dr == null) throw new ArgumentNullException(nameof(dr));

            var t = new T();
            var propertyList = t.GetType().GetProperties();
            var fieldNameList = new List<string>();

            for (var i = 0; i < dr.FieldCount; i++)
            {
                fieldNameList.Add(dr.GetName(i));
            }

            foreach (var property in propertyList)
            {
                if (!property.CanWrite) continue;

                var fieldName = property.Name;
                if (!fieldNameList.Contains(fieldName)) continue;

                var value = dr[fieldName];

                if (value is DBNull) continue;

                property.SetValue(t, value, null);
            }
            return t;
        }

        /// <summary>
        /// 把IDataReader对象转成实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToEntity<T>(IDataReader dr) where T : new()
        {
            if (dr != null && dr.Read())
            {
                return DataReaderToEntity<T>(dr);
            }
            return default(T);
        }

        /// <summary>
        /// 把IDataReader对象转成实体类的列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static List<T> ToEntities<T>(IDataReader dr) where T : new()
        {
            var list = new List<T>();

            if (dr == null) return list;

            while (dr.Read())
            {
                list.Add(DataReaderToEntity<T>(dr));
            }
            return list;
        }


        /// <summary>
        /// 把简单Model类转换为键值对集合
        /// </summary>
        /// <param name="obj">数据实体</param>
        /// <returns></returns>
        public static Dictionary<string, object> ObjectToDictionary(object obj)
        {
            if (obj == null) return null;

            var dic = obj.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(obj, null));

            var baseType = obj.GetType().BaseType;

            if (baseType == null) return dic;

            foreach (var p in baseType.GetProperties())
            {
                if (dic.ContainsKey(p.Name)) continue;

                dic.Add(p.Name, p.GetValue(obj, null));
            }

            return dic;
        }


        /// <summary>
        /// 把枚举类型转换为键值对集合
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns></returns>
        public static Dictionary<string, int> EnumToDictionary<T>() where T : struct
        {
            var tType = typeof(T);
            if (tType.IsEnum == false) throw new Exception($"传入的类型: `{tType.FullName}` 非枚举类型.");

            var enumToDictionary = new Dictionary<string, int>();

            foreach (int v in System.Enum.GetValues(tType))
            {
                var strName = System.Enum.GetName(tType, v);
                enumToDictionary.Add(strName ?? throw new InvalidOperationException(), v);
            }

            return enumToDictionary;
        }

        /// <summary>    
        /// 将实体类转换成DataTable    
        /// </summary>    
        /// <param name="list">集合</param>    
        /// <returns></returns>    
        public static DataTable ToDataTableTow(IList list)
        {
            if (list == null) return null;

            var result = new DataTable();
            if (list.Count <= 0) return result;
            var listFirst = list[0];
            if (listFirst == null)
            {
                throw new Exception("第一个元素为null,无法采集DataTable列名");
            }
            var propertyArray = listFirst.GetType().GetProperties();

            foreach (var property in propertyArray)
            {
                result.Columns.Add(property.Name, property.PropertyType);
            }

            foreach (var t in list)
            {
                var tempList = new ArrayList();
                foreach (var property in propertyArray)
                {
                    var obj = property.GetValue(t, null);
                    tempList.Add(obj);
                }
                var array = tempList.ToArray();
                result.LoadDataRow(array, true);
            }
            return result;
        }

        /// <summary>
        /// 将简单集合类转换成DataTable
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public static DataTable ModelListToDataTable<T>(List<T> modelList)
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }

            var dt = CreateData<T>(); //创建表结构

            foreach (var model in modelList)
            {
                var dataRow = dt.NewRow();
                foreach (var property in typeof(T).GetProperties())
                {
                    var value = property.GetValue(model);
                    if (value == null)
                    {
                        dataRow[property.Name] = DBNull.Value;
                    }
                    else
                    {
                        dataRow[property.Name] = value;
                    }

                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }
        /// <summary>
        /// 根据实体类得到DataTable结构
        /// </summary>
        /// <returns></returns>
        private static DataTable CreateData<T>()
        {
            var dataTable = new DataTable(typeof(T).Name);
            foreach (var property in typeof(T).GetProperties())
            {
                var currentType = property.PropertyType;

                // 当字段类型是Nullable<>时
                if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    currentType = currentType.GetGenericArguments()[0];
                }

                dataTable.Columns.Add(new DataColumn(property.Name, currentType));

            }
            return dataTable;

        }


        /// <summary>
        /// 把键值对数据转换成Model实体
        /// </summary>
        /// <typeparam name="T">model类型</typeparam>
        /// <param name="dictionary">键值对数据</param>
        /// <returns></returns>
        public static T DictionaryToModel<T>(Dictionary<string, string> dictionary)
        {
            //T curObj = default(T);
            var curObj = Activator.CreateInstance<T>();

            //根据Key值设定 Columns
            foreach (var (key, value) in dictionary)
            {
                var prop = curObj.GetType().GetProperty(key);
                if (string.IsNullOrEmpty(value)) continue;

                var curValue = value;

                //Nullable 获取Model类字段的真实类型
                var itemType = Nullable.GetUnderlyingType(prop.PropertyType) == null
                    ? prop.PropertyType
                    : Nullable.GetUnderlyingType(prop.PropertyType);
                //根据Model类字段的真实类型进行转换

                prop.SetValue(curObj, Convert.ChangeType(curValue, itemType), null);
            }

            return curObj;
        }
    }
}
