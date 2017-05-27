using System.Collections.Generic;
using En = WebProject.GetCode.Entity;

namespace WebProject.GetCode.Site.Common
{
    public class GenerateCode
    {
        public static string GetEntity(List<En.SysColumns> columns, string tableName)
        {
            var list = new List<string>();

            var result = @"";

            columns.ForEach(one =>
            {
                var type = "string";

                switch (one.Type)
                {
                    case "nvarchar":
                        type = "string";
                        break;
                    case "varchar":
                        type = "string";
                        break;
                    case "char":
                        type = "string";
                        break;
                    case "nchar":
                        type = "string";
                        break;
                    case "decimal":
                        type = "decimal";
                        break;
                    case "datetime":
                        type = "DateTime";
                        break;
                    case "int":
                        type = "int";
                        break;
                    case "bigint":
                        type = "long";
                        break;
                    case "bit":
                        type = "bool";
                        break;
                    case "tinyint":
                        type = "byte";
                        break;
                    case "smallint":
                        type = "short";
                        break;
                }

                var propertyCode =
                @"        
        /// <summary>
        /// @Property
        /// </summary>
        public @Type @Name { get; set; }
".Replace("@Property", one.Property).Replace("@Type", type).Replace("@Name", one.Name);

                list.Add(propertyCode);
            });
            if (list.Count > 0)
            {
                var propertyCode = string.Join("\n\n", list);

                result =
                    @"
    public class @TableName
    {
        @EntityTemplate
    }
".Replace("@EntityTemplate", propertyCode).Replace("@TableName", tableName);
            }
            return result;
        }

        public static string GetDataAccess(List<En.SysColumns> columns, string tableName)
        {
            var result =
                @"
    public class @TableName
    {@EntityTemplate
    }
".Replace("@TableName", tableName);

            var classCode = @"";

            #region 增加实体类
            {
                var functionString =
                    @"
        #region 增加
        /// <summary>
        /// 增加实体
        /// </summary>
        /// <param name=""databaseConnectionString"">数据库链接字符串</param>
        /// <param name=""entity"">实体</param>
        public static void Add(string databaseConnectionString, En.@TableName entity)
        {
@Content
        }
        #endregion
".Replace("@TableName", tableName);

                var functionCode =
                    @"
            var sql = @""
                INSERT INTO [@TableName]
                (@Column
                )
                VALUES
                (@Value
                )
            "";

            var parameters = new List<SqlParameter>();
@Parameters

            AgileDatabase.ExecuteNonQuery(databaseConnectionString, sql, parameters.ToArray());
".Replace("@TableName", tableName);

                var columnString = new List<string>();
                var valueString = new List<string>();
                var parameterString = new List<string>();

                columns.ForEach(one =>
                {
                    var column = $@"
                    [{one.Name}]";
                    columnString.Add(column);

                    var value = $@"
                    @{one.Name}";
                    valueString.Add(value);

                    var parameter =
                    @"
            parameters.Add(new SqlParameter() { ParameterName = ""@@ColumnName"", Value = entity.@ColumnName });".Replace("@ColumnName", one.Name);
                    parameterString.Add(parameter);
                });

                functionCode = functionCode.Replace("@Column", string.Join(",", columnString)).Replace("@Value", string.Join(",", valueString)).Replace("@Parameters", string.Join("", parameterString));

                classCode += functionString.Replace("@Content", functionCode);
            }
            #endregion

            #region 更新实体类
            {
                var functionString =
                    @"
        #region 更新
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name=""databaseConnectionString"">数据库链接字符串</param>
        /// <param name=""entity"">实体</param>
        public static void UpdateByID(string databaseConnectionString, En.@TableName entity)
        {
@Content
        }
        #endregion
".Replace("@TableName", tableName);

                var functionCode =
                    @"
            var sql = @""
                UPDATE [@TableName]
                SET@Value
                WHERE 
                    [ID] = @ID
            "";

            var parameters = new List<SqlParameter>();
@Parameters

            AgileDatabase.ExecuteNonQuery(databaseConnectionString, sql, parameters.ToArray());
".Replace("@TableName", tableName);

                var valueString = new List<string>();
                var parameterString = new List<string>();

                columns.ForEach(one =>
                {
                    var value = $@"
                    {one.Name} = @{one.Name}";
                    valueString.Add(value);

                    var parameter =
                    @"
            parameters.Add(new SqlParameter() { ParameterName = ""@@ColumnName"", Value = entity.@ColumnName });".Replace("@ColumnName", one.Name);
                    parameterString.Add(parameter);
                });

                functionCode = functionCode.Replace("@Value", string.Join(",", valueString)).Replace("@Parameters", string.Join("", parameterString));

                classCode += functionString.Replace("@Content", functionCode);
            }
            #endregion

            #region 删除实体类
            {
                var functionString =
                    @"
        #region 删除
        /// <summary>
        /// 逻辑删除实体
        /// </summary>
        /// <param name=""databaseConnectionString"">数据库链接字符串</param>
        /// <param name=""wherePart"">参数项</param>
        public static void LogicalDelete(string databaseConnectionString, dynamic wherePart)
        {
@Content
        }
".Replace("@TableName", tableName);

                var functionCode =
                    @"
            var sql = @""
                UPDATE [@TableName]
                SET 
                    [IsDelete] = 1,
                    [UpdateTime] = GETDATE()
                WHERE   
                    ID = @ID
            "";

            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter() { ParameterName = ""@ID"", Value = wherePart.ID });

            AgileDatabase.ExecuteNonQuery(databaseConnectionString, sql, parameters.ToArray());
".Replace("@TableName", tableName);

                classCode += functionString.Replace("@Content", functionCode);
            }
            {
                var functionString =
                    @"
        /// <summary>
        /// 物理删除实体(慎用)
        /// </summary>
        /// <param name=""databaseConnectionString"">数据库链接字符串</param>
        /// <param name=""wherePart"">参数项</param>
        public static void PhysicalDelete(string databaseConnectionString, dynamic wherePart)
        {
@Content
        }
        #endregion
".Replace("@TableName", tableName);

                var functionCode =
                    @"
            var sql = @""
                DELETE FROM [@TableName]
                WHERE   
                    ID = @ID
            "";

            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter() { ParameterName = ""@ID"", Value = wherePart.ID });

            AgileDatabase.ExecuteNonQuery(databaseConnectionString, sql, parameters.ToArray());
".Replace("@TableName", tableName);

                classCode += functionString.Replace("@Content", functionCode);
            }
            #endregion

            #region 查询实体类
            {
                var functionString =
                    @"
        #region 查询
        /// <summary>
        /// 查询全部实体
        /// </summary>
        /// <param name=""databaseConnectionString"">数据库链接字符串</param>
        public static List<En.@TableName> GetAll(string databaseConnectionString)
        {
@Content
        }
".Replace("@TableName", tableName);

                var functionCode =
                    @"
            var sql = @""
                SELECT@Column
                FROM
                    [@TableName]
                WHERE [IsDelete] = 0
            "";

            var dataTable = new DataTable();

            AgileDatabase.Fill(databaseConnectionString, sql, dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new En.@TableName()
                {@Entity
                }).ToList();
            }
            else
            {
                return null;
            }
".Replace("@TableName", tableName);

                var columnString = new List<string>();
                var entityString = new List<string>();

                columns.ForEach(one =>
                {
                    var column = $@"
                    [{one.Name}]";
                    columnString.Add(column);

                    var type = "string";

                    switch (one.Type)
                    {
                        case "nvarchar":
                            type = "string";
                            break;
                        case "varchar":
                            type = "string";
                            break;
                        case "char":
                            type = "string";
                            break;
                        case "nchar":
                            type = "string";
                            break;
                        case "decimal":
                            type = "decimal";
                            break;
                        case "datetime":
                            type = "DateTime";
                            break;
                        case "int":
                            type = "int";
                            break;
                        case "bigint":
                            type = "long";
                            break;
                        case "bit":
                            type = "bool";
                            break;
                        case "tinyint":
                            type = "byte";
                            break;
                        case "smallint":
                            type = "short";
                            break;
                    }

                    var entity =
                    $@"
                    {one.Name} = row.Field<{type}>(""{one.Name}"")";
                    entityString.Add(entity);
                });

                functionCode = functionCode.Replace("@Column", string.Join(",", columnString)).Replace("@Entity", string.Join(",", entityString));

                classCode += functionString.Replace("@Content", functionCode);
            }
            {
                var functionString =
                    @"
        /// <summary>
        /// 查询实体
        /// </summary>
        /// <param name=""databaseConnectionString"">数据库链接字符串</param>
        /// <param name=""wherePart"">参数项</param>
        public static En.@TableName GetByID(string databaseConnectionString, dynamic wherePart)
        {
@Content
        }
        #endregion
".Replace("@TableName", tableName);

                var functionCode =
                    @"
            var sql = @""
                SELECT@Column
                FROM [@TableName]
                WHERE   
                    [IsDelete] = 0
                    [ID] = @ID
            "";

            var dataTable = new DataTable();

            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter() { ParameterName = ""@ID"", Value = wherePart.ID });

            AgileDatabase.Fill(databaseConnectionString, sql, dataTable, parameters.ToArray());

            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return new En.@TableName()
                {@Entity
                };
            }
            else
            {
                return null;
            }
".Replace("@TableName", tableName);

                var columnString = new List<string>();
                var entityString = new List<string>();

                columns.ForEach(one =>
                {
                    var column = $@"
                    [{one.Name}]";
                    columnString.Add(column);

                    var type = "string";

                    switch (one.Type)
                    {
                        case "nvarchar":
                            type = "string";
                            break;
                        case "varchar":
                            type = "string";
                            break;
                        case "char":
                            type = "string";
                            break;
                        case "nchar":
                            type = "string";
                            break;
                        case "decimal":
                            type = "decimal";
                            break;
                        case "datetime":
                            type = "DateTime";
                            break;
                        case "int":
                            type = "int";
                            break;
                        case "bigint":
                            type = "long";
                            break;
                        case "bit":
                            type = "bool";
                            break;
                        case "tinyint":
                            type = "byte";
                            break;
                        case "smallint":
                            type = "short";
                            break;
                    }

                    var entity =
                    $@"
                    {one.Name} = row.Field<{type}>(""{one.Name}"")";
                    entityString.Add(entity);
                });

                functionCode = functionCode.Replace("@Column", string.Join(",", columnString)).Replace("@Entity", string.Join(",", entityString));

                classCode += functionString.Replace("@Content", functionCode);
            }
            #endregion

            return result.Replace("@EntityTemplate", classCode);
        }

        public static string GetExtraDataAccess(List<En.SysColumns> columns, string tableName)
        {
            var result = @"
        #region 其他
@Code
        #endregion
";
            var codeString = "";

            #region 获取数量
            {
                var code = @"
        /// <summary>
        /// 查询所有实体数量
        /// </summary>
        /// <param name=""databaseConnectionString"">数据库链接字符串</param>
        public static int GetCount(string databaseConnectionString)
        {
            var sql = @""
                        SELECT
	                        COUNT(*)
                        FROM
	                        @TableName
                        WHERE IsDelete = 0;
                    "";

            var dataTable = new DataTable();
            AgileDatabase.Fill(databaseConnectionString, sql, dataTable);


            return dataTable.Rows[0].Field<int>(0);
        }
".Replace("@TableName", tableName); ;

                codeString += code;
            }
            #endregion

            #region 根据分页查询实体
            {
                var code = @"
        /// <summary>
        /// 根据分页信息查询实体列表（不存在时，返回null）
        /// </summary>
        /// <param name=""databaseConnectionString"">数据库链接字符串</param>
        /// <param name=""wherePart"">参数项</param>
        public static List<En.@TableName> GetByPage(string databaseConnectionString, dynamic wherePart)
        {
            var sql = @""
                SELECT @ColumnOut
				FROM
				(
					SELECT 
						RowNum = ROW_NUMBER() OVER(ORDER BY [@OrderColumn] ASC), @ColumnIn
					FROM [@TableName]
					WHERE IsDelete = 0
				)AS T
				WHERE RowNum > @PageSize * (@PageNum - 1) AND RowNum <= @PageSize * @PageNum
                ORDER BY [@OrderColumn] ASC
                    "";
            sql = sql.Replace(""@OrderColumn"", wherePart.OrderColumn);
            
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter() { ParameterName = ""@PageNum"", Value = wherePart.PageNum });
            parameters.Add(new SqlParameter() { ParameterName = ""@PageSize"", Value = wherePart.PageSize });

            var dataTable = new DataTable();
            AgileDatabase.Fill(databaseConnectionString, sql, dataTable, parameters.ToArray());

            if (dataTable.Rows.Count > 0)
            {
                return dataTable.AsEnumerable().Select(row => new En.ProductBrandDefine()
                {@Entity
                }).ToList();
            }
            else
            {
                return null;
            }
".Replace("@TableName", tableName);

                var columnOut = new List<string>();
                var columnIn = new List<string>();
                var entityString = new List<string>();

                columns.ForEach(one =>
                {
                    var columnout = $@"
                    [{one.Name}]";
                    columnOut.Add(columnout);

                    var columnin = $@"
                        [{one.Name}]";
                    columnIn.Add(columnin);

                    var type = "string";

                    switch (one.Type)
                    {
                        case "nvarchar":
                            type = "string";
                            break;
                        case "varchar":
                            type = "string";
                            break;
                        case "char":
                            type = "string";
                            break;
                        case "nchar":
                            type = "string";
                            break;
                        case "decimal":
                            type = "decimal";
                            break;
                        case "datetime":
                            type = "DateTime";
                            break;
                        case "int":
                            type = "int";
                            break;
                        case "bigint":
                            type = "long";
                            break;
                        case "bit":
                            type = "bool";
                            break;
                        case "tinyint":
                            type = "byte";
                            break;
                        case "smallint":
                            type = "short";
                            break;
                    }

                    var entity =
                    $@"
                    {one.Name} = row.Field<{type}>(""{one.Name}"")";
                    entityString.Add(entity);
                });

                codeString += code.Replace("@ColumnOut", string.Join(",", columnOut)).Replace("@ColumnIn", string.Join(",", columnIn)).Replace("@Entity", string.Join(",", entityString));
            }
            #endregion

            #region 获取表模式
            {
                var code = @"
        /// <summary>
        /// 获取表模式
        /// </summary>
        /// <param name=""databaseConnectionString"">数据库链接字符串</param>
        public static DataTable GetSchema(string databaseConnectionString)
        {
            var sql = @""
                        SELECT @Column
                        FROM
	                        @TableName
                        WHERE 1 <> 1;
                    "";

            var dataTable = new DataTable();
            AgileDatabase.Fill(databaseConnectionString, sql, dataTable);


            return dataTable;
        }
".Replace("@TableName", tableName);
                var columnString = new List<string>();

                columns.ForEach(one =>
                {
                    var column = $@"
                            [{one.Name}]";
                    columnString.Add(column);
                });

                codeString += code.Replace("@Column", string.Join(",", columnString));
            }
            #endregion

            #region 获取数量
            {
                var code = @"
        /// <summary>
        /// 批量拷贝
        /// </summary>
        /// <param name=""databaseConnectionString"">数据库链接字符串</param>
        public static void BulkCopy(string databaseConnectionString, DataTable dataTable)
        {
            AgileSQLServer.BulkCopy(databaseConnectionString, dataTable, ""@TableName"");
        }
".Replace("@TableName", tableName);

                codeString += code;
            }
            #endregion

            return result.Replace("@Code", codeString);
        }

        public static string GenerateModel(List<En.SysColumns> columns, string tableName)
        {
            var list = new List<string>();

            var result = @"";

            columns.ForEach(one =>
            {
                var type = "string";

                switch (one.Type)
                {
                    case "nvarchar":
                        type = "string";
                        break;
                    case "varchar":
                        type = "string";
                        break;
                    case "char":
                        type = "string";
                        break;
                    case "nchar":
                        type = "string";
                        break;
                    case "decimal":
                        type = "decimal";
                        break;
                    case "datetime":
                        type = "DateTime";
                        break;
                    case "int":
                        type = "int";
                        break;
                    case "bigint":
                        type = "long";
                        break;
                    case "bit":
                        type = "bool";
                        break;
                    case "tinyint":
                        type = "byte";
                        break;
                    case "smallint":
                        type = "short";
                        break;
                }

                var propertyCode =
                @"        
        /// <summary>
        /// @Property
        /// </summary>
        public @Type @Name { get; set; }
".Replace("@Property", one.Property).Replace("@Type", type).Replace("@Name", one.Name);

                list.Add(propertyCode);
            });
            if (list.Count > 0)
            {
                list.Add(@"        
        /// <summary>
        /// @Property
        /// </summary>
        public @Type @Name { get; set; }
".Replace("@Property", "页码").Replace("@Type", "int").Replace("@Name", "PageNum"));

                list.Add(@"        
        /// <summary>
        /// @Property
        /// </summary>
        public @Type @Name { get; set; }
".Replace("@Property", "页大小").Replace("@Type", "int").Replace("@Name", "PageSize"));

                list.Add(@"        
        /// <summary>
        /// @Property
        /// </summary>
        public @Type @Name { get; set; }
".Replace("@Property", "排序列").Replace("@Type", "string").Replace("@Name", "OrderColumn"));

                var propertyCode = string.Join("\n\n", list);

                result =
                    @"
    public class @TableName
    {
        @EntityTemplate
    }
".Replace("@EntityTemplate", propertyCode).Replace("@TableName", tableName);
            }
            return result;
        }
    }
}