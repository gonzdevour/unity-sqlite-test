using System;
using System.Collections.Generic;
using UnityEngine;
using SQLite;
using System.Reflection;

public class SQLiteManager
{
    private string dbPath;

    // 构造函数，传入数据库路径
    public SQLiteManager(string dbPath)
    {
        this.dbPath = dbPath;
    }

    // 创建表
    public void CreateTable(string tableName, string[] headers)
    {
        using (var dbConnection = new SQLiteConnection(dbPath))
        {
            string createTableQuery = $"CREATE TABLE IF NOT EXISTS {tableName} (Id INTEGER PRIMARY KEY AUTOINCREMENT";

            foreach (var header in headers)
            {
                createTableQuery += $", {header} TEXT"; // 假设所有列为 TEXT，必要时可以调整
            }

            createTableQuery += ");";
            dbConnection.Execute(createTableQuery);
            Debug.Log($"Table {tableName} created or already exists.");
        } // dbConnection 会在此处自动关闭
    }

    // 插入数据
    public void InsertData(string tableName, string[] headers, object[] values)
    {
        using (var dbConnection = new SQLiteConnection(dbPath))
        {
            string insertQuery = $"INSERT INTO {tableName} ({string.Join(",", headers)}) VALUES ({string.Join(",", GeneratePlaceholders(headers.Length))});";
            dbConnection.Execute(insertQuery, values);
            Debug.Log("Data inserted successfully.");
        } // dbConnection 会在此处自动关闭
    }
    public void InsertDataFromObject(string tableName, object obj)
    {
        Type objType = obj.GetType();
        PropertyInfo[] properties = objType.GetProperties();

        List<string> headers = new();
        List<string> values = new();

        foreach (var prop in properties)
        {
            if (prop.GetValue(obj) != null)
            {
                headers.Add(prop.Name);  // 属性名作为列名
                values.Add(prop.GetValue(obj).ToString());  // 属性值作为插入的数据
            }
        }

        InsertData(tableName, headers.ToArray(), values.ToArray());
    }
    // 查询数据
    public List<T> QueryTable<T>(string tableName, string condition = "", string orderBy = "") where T : new()
    {
        using (var dbConnection = new SQLiteConnection(dbPath))
        {
            // 构建查询语句，始终使用 SELECT * 查询所有列
            string query = $"SELECT * FROM {tableName}";

            // 如果条件不为空，则添加 WHERE 子句
            if (!string.IsNullOrEmpty(condition))
            {
                query += $" WHERE {condition}";
            }

            // 如果有排序参数，则添加 ORDER BY 子句
            if (!string.IsNullOrEmpty(orderBy))
            {
                query += $" ORDER BY {orderBy}";
            }

            // 执行查询并返回结果
            var result = dbConnection.Query<T>(query);
            return result;
        } // dbConnection 会在此处自动关闭
    }

    // 更新数据
    public void UpdateData(string tableName, string[] headers, string[] values, string condition)
    {
        using var dbConnection = new SQLiteConnection(dbPath);
        string updateQuery = $"UPDATE {tableName} SET ";

        // 构建 SET 语句
        for (int i = 0; i < headers.Length; i++)
        {
            updateQuery += $"{headers[i]} = ?";
            if (i < headers.Length - 1) updateQuery += ", ";
        }

        // 加入条件字符串
        updateQuery += $" WHERE {condition};";

        // 执行查询
        dbConnection.Execute(updateQuery, values);

        Debug.Log("Data updated successfully.");
    }


    // 删除数据
    public void DeleteData(string tableName, string condition)
    {
        using var dbConnection = new SQLiteConnection(dbPath);

        // 构造动态 SQL 查询
        string query = $"DELETE FROM {tableName} WHERE {condition};";

        dbConnection.Execute(query);
        Debug.Log($"Data with condition '{condition}' deleted from {tableName}.");
    }

    // 生成占位符 `?`
    private IEnumerable<string> GeneratePlaceholders(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return "?";
        }
    }
}
