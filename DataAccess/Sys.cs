using AgileFramework.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using En = WebProject.GetCode.Entity;

namespace WebProject.GetCode.DataAccess
{
    public class Sys
    {
        public static List<string> GetAllDatabases(string connectionString)
        {
            var sql = @"SELECT DISTINCT name FROM sys.databases";

            var dataTable = new DataTable();

            AgileDatabase.Fill(connectionString, sql, dataTable);

            return dataTable.AsEnumerable().Select(one => one.Field<string>("name")).ToList();
        }

        public static List<En.SysTables> GetAllTables(string connectionString, string dbName)
        {
            var sql = $@"USE {dbName};
                        SELECT * FROM sys.tables";

            var dataTable = new DataTable();

            AgileDatabase.Fill(connectionString, sql, dataTable);

            return dataTable.AsEnumerable().Select(one => new En.SysTables()
            {
                Name = one.Field<string>("name"),
                ObjectID = one.Field<int>("object_id").ToString()
            }).ToList();
        }

        public static List<En.SysColumns> GetColumns(string connectionString, dynamic wherePart)
        {
            var sql = $@"USE {wherePart.DBName};
                        SELECT 
	                        A.[name] AS [Name],
	                        B.[value] AS [Property],
	                        C.[name] AS [Type]
                        FROM sys.columns AS A
                        INNER JOIN sys.extended_properties AS B ON A.object_id = B.major_id AND A.column_id = B.minor_id
                        INNER JOIN sys.types AS C ON A.system_type_id = C.system_type_id AND A.user_type_id = C.user_type_id
                        WHERE object_id = '{wherePart.ID}'";
            var dataTable = new DataTable();

            AgileDatabase.Fill(connectionString, sql, dataTable);

            return dataTable.AsEnumerable().Select(one => new En.SysColumns()
            {
                Name = one.Field<string>("Name"),
                Property = one.Field<string>("Property"),
                Type = one.Field<string>("Type").ToLower()
            }).ToList();
        }
    }
}
