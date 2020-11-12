using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using FreeSql;
using LinCms.Common;
using LinCms.Data.Enums;
using LinCms.Entities;
using LinCms.Entities.Base;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Serilog;

namespace LinCms.FreeSql
{
    public static class FreeSqlExtension
    {

        public static ISelect<T> AsTable<T>(this ISelect<T> @this, string tableName, int count) where T : class
        {
            string[] tableNames = new string[] { };
            for (int i = 0; i < count; i++)
            {
                tableNames.AddIfNotContains($"{tableName}_{i}");
            }
            @this.AsTable(tableNames);
            return @this;
        }

        public static ISelect<T> AsTable<T>(this ISelect<T> @this, params string[] tableNames) where T : class
        {
            tableNames?.ToList().ForEach(tableName =>
            {
                @this.AsTable((type, oldname) =>
                {
                    if (type == typeof(T)) return tableName;
                    return null;
                });
            });
            return @this;
        }
        public static FreeSqlBuilder UseConnectionString(this FreeSqlBuilder @this, IConfiguration configuration)
        {
            IConfigurationSection dbTypeCode = configuration.GetSection("ConnectionStrings:DefaultDB");
            if (Enum.TryParse(dbTypeCode.Value, out DataType dataType))
            {
                if (!Enum.IsDefined(typeof(DataType), dataType))
                {
                    Log.Error($"数据库配置ConnectionStrings:DefaultDB:{dataType}无效");
                }
                IConfigurationSection configurationSection = configuration.GetSection($"ConnectionStrings:{dataType}");
                @this.UseConnectionString(dataType, configurationSection.Value);
            }
            else
            {
                Log.Error($"数据库配置ConnectionStrings:DefaultDB:{dbTypeCode.Value}无效");
            }
            return @this;
        }



        public static IFreeSql CreateDatabaseIfNotExists(this IFreeSql @this)
        {
            DataType dbType = @this.Ado.DataType;
            switch (dbType)
            {
                case DataType.MySql:
                    return @this.CreateDatabaseIfNotExistsMySql();
                case DataType.SqlServer:
                    return @this.CreateDatabaseIfNotExistsSqlServer();
                case DataType.PostgreSQL:
                    break;
                case DataType.Oracle:
                    break;
                case DataType.Sqlite:
                    return @this;
                case DataType.OdbcOracle:
                    break;
                case DataType.OdbcSqlServer:
                    break;
                case DataType.OdbcMySql:
                    break;
                case DataType.OdbcPostgreSQL:
                    break;
                case DataType.Odbc:
                    break;
                case DataType.OdbcDameng:
                    break;
                case DataType.MsAccess:
                    break;
                case DataType.Dameng:
                    break;
                case DataType.OdbcKingbaseES:
                    break;
                case DataType.ShenTong:
                    break;
                case DataType.KingbaseES:
                    break;
                case DataType.Firebird:
                    break;
                default:
                    break;
            }
            throw new NotSupportedException("不支持创建数据库");
        }

        public static IFreeSql CreateDatabaseIfNotExistsMySql(this IFreeSql @this)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(@this.Ado.ConnectionString);

            string createDatabaseSQL = $"USE mysql;CREATE DATABASE IF NOT EXISTS `{builder.Database}` CHARACTER SET '{builder.CharacterSet}' COLLATE 'utf8mb4_general_ci'";

            using (MySqlConnection cnn = new MySqlConnection($"Data Source={builder.Server};Port={builder.Port};User ID={builder.UserID};Password={builder.Password};Initial Catalog=mysql;Charset=utf8;SslMode=none;Max pool size=1"))
            {
                cnn.Open();
                using (MySqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = createDatabaseSQL;
                    cmd.ExecuteNonQuery();
                }
            }
            return @this;
        }


        public static IFreeSql CreateDatabaseIfNotExistsSqlServer(this IFreeSql @this)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(@this.Ado.ConnectionString);
            string name;
            string createDatabaseSQL;
            if (!string.IsNullOrEmpty(builder.AttachDBFilename))
            {
                string fileName = ExpandFileName(builder.AttachDBFilename);
                name = Path.GetFileNameWithoutExtension(fileName);
                string logFileName = Path.ChangeExtension(fileName, ".ldf");
                createDatabaseSQL = @$"CREATE DATABASE {builder.InitialCatalog}   on  primary   
                (
                    name = '{name}',
                    filename = '{fileName}'
                )
                log on
                (
                    name= '{name}_log',
                    filename = '{logFileName}'
                )";
            }
            else
            {
                createDatabaseSQL = @$"CREATE DATABASE {builder.InitialCatalog}";
            }

            using (SqlConnection cnn = new SqlConnection($"Data Source={builder.DataSource};Integrated Security = True;User ID={builder.UserID};Password={builder.Password};Initial Catalog=master;Min pool size=1"))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = $"select * from sysdatabases where name = '{builder.InitialCatalog}'";

                    SqlDataAdapter apter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    apter.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        cmd.CommandText = createDatabaseSQL;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return @this;
        }

        private static string ExpandFileName(string fileName)
        {
            if (fileName.StartsWith("|DataDirectory|", StringComparison.OrdinalIgnoreCase))
            {
                var dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
                if (string.IsNullOrEmpty(dataDirectory))
                {
                    dataDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }

                fileName = Path.Combine(dataDirectory, fileName.Substring("|DataDirectory|".Length));
            }

            return Path.GetFullPath(fileName);
        }

    }
}
