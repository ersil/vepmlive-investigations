﻿using System;
using System.Data.SqlClient;

namespace EPMLiveCore.Jobs.EPMLiveUpgrade.Steps
{
    public static class DdlHelper
    {
        public static bool ColumnExist(this SqlConnection sqlConnection, string tableName, string columnName)
        {
            var sql =
                $@"IF NOT EXISTS(SELECT * FROM sys.columns WHERE [name] = N'{
                        columnName
                    }' AND [object_id] = OBJECT_ID(N'{tableName}'))
BEGIN
    SELECT 1
END
ELSE SELECT 0";

            return ExecuteReader(sqlConnection, sql, reader =>
            {
                if (reader.Read())
                    return reader.GetInt32(0) != 1;
                return false;
            });
        }

        public static bool TableExist(this SqlConnection sqlConnection, string tableName)
        {
            var sql =
                $@"IF EXISTS(SELECT table_name FROM INFORMATION_SCHEMA.tables WHERE table_name = N'{tableName}')
BEGIN
    SELECT 1
END
ELSE BEGIN
    SELECT 0
END";

            return ExecuteReader(sqlConnection, sql, reader =>
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0) == 1;
                }

                return false;
            });
        }

        public static T ExecuteReader<T>(this SqlConnection sqlConnection, string sql,
            Func<SqlDataReader, T> processReaderFunc)
        {
            using (var sqlCommand = new SqlCommand(sql, sqlConnection))
            {
                using (var reader = sqlCommand.ExecuteReader())
                {
                    return processReaderFunc(reader);
                }
            }
        }

        public static void AddColumn(this SqlConnection sqlConnection, string tableName, string columnDefinition)
        {
            ExecuteNonQuery(sqlConnection, $"ALTER TABLE {tableName} ADD {columnDefinition}");
        }

        public static void ExecuteNonQuery(this SqlConnection sqlConnection, string sql)
        {
            using (var sqlCommand = new SqlCommand(sql, sqlConnection))
            {
                sqlCommand.ExecuteNonQuery();
            }
        }

        public static string GetDefinition(this SqlConnection sqlConnection, string objectName, string objType)
        {
            var sql =
                $"select definition from sys.objects o join sys.sql_modules m on m.object_id = o.object_id where o.object_id = object_id('{objectName}')  and o.type = '{objType}'";
            return ExecuteReader(sqlConnection, sql, reader =>
            {
                if (reader.Read())
                    return reader.GetString(0);
                return null;
            });
        }

        public static string GetViewDefinition(this SqlConnection sqlConnection, string viewName)
        {
            return GetDefinition(sqlConnection, viewName, "V");
        }

        public static string GetSpDefinition(this SqlConnection sqlConnection, string spName)
        {
            return GetDefinition(sqlConnection, spName, "P");
        }
        public static string IndexDefinition(this SqlConnection sqlConnection, string indexName)
        {
            return GetIndexDefinition(sqlConnection, indexName);
        }
        public static string GetIndexDefinition(this SqlConnection sqlConnection, string objectName)
        {
            var sql = string.Format("Select name from sys.indexes where name='{0}'", objectName);
            return ExecuteReader(sqlConnection, sql, reader =>
            {
                if (reader.Read())
                    return reader.GetString(0);
                return null;
            });
        }
    }
}