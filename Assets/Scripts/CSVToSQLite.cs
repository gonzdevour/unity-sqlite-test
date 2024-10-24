using System.IO;
using System.Text;
using UnityEngine;
using SQLite;
using System.Collections.Generic;
using yutokun;

public class CSVToSQLite : MonoBehaviour
{
    public void ImportCSVToDatabase(string pageName, string csvContent, string dbPath)
    {
        if (!string.IsNullOrEmpty(csvContent))
        {
            // 使用 CSVParser 解析 CSV 內容
            var sheet = CSVParser.LoadFromString(csvContent);

            if (sheet.Count > 0)
            {
                var headers = sheet[0]; // 假設第一行是標題行

                using (var db = new SQLiteConnection(dbPath))
                {
                    // 使用 pageName 动态创建表格名称
                    string tableName = pageName;

                    // 创建与分页名相对应的表格
                    CreateDynamicTable(db, tableName, headers.ToArray());

                    // 从第二行开始插入資料
                    for (int i = 1; i < sheet.Count; i++)
                    {
                        var row = sheet[i]; // 取每一行資料
                        InsertDynamicData(db, tableName, headers.ToArray(), row.ToArray());
                    }
                }

                Debug.Log($"Data imported into SQLite table {pageName}.");
            }
        }
        else
        {
            Debug.Log("CSV content is empty or not found.");
        }
    }

    private void CreateDynamicTable(SQLiteConnection db, string tableName, string[] headers)
    {
        // 先刪除舊的表格（如果存在）
        db.Execute($"DROP TABLE IF EXISTS {tableName}");

        // 然後創建新的表格
        string createTableQuery = $"CREATE TABLE IF NOT EXISTS {tableName} (Id INTEGER PRIMARY KEY AUTOINCREMENT";

        foreach (var header in headers)
        {
            createTableQuery += $", {header} TEXT"; // 假設所有欄位都是 TEXT 類型，根據需要可以調整
        }

        createTableQuery += ");";
        db.Execute(createTableQuery);
    }

    private void InsertDynamicData(SQLiteConnection db, string tableName, string[] headers, string[] values)
    {
        // 構建 INSERT OR REPLACE 語句
        string insertQuery = $"INSERT OR REPLACE INTO {tableName} ({string.Join(",", headers)}) VALUES ({string.Join(",", GeneratePlaceholders(headers.Length))});";

        // 使用參數化語句插入資料（覆蓋已有的資料）
        db.Execute(insertQuery, values);
    }


    private IEnumerable<string> GeneratePlaceholders(int count)
    {
        // 生成一系列的 `?` 占位符，用於參數化插入
        for (int i = 0; i < count; i++)
        {
            yield return "?";
        }
    }
}
