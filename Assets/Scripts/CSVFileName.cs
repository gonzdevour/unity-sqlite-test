// FileNameHelper.cs
using System.Net.Mime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class CSVFileName
{
    public string FullName { get; set; }  // 原始檔名（含 .csv）
    public string ExcelName { get; set; } // 檔案中的 Excel 名稱部分
    public string PageName { get; set; }  // 檔案中的頁面名稱部分

    public CSVFileName(string fullName, string excelName, string pageName)
    {
        FullName = fullName;
        ExcelName = excelName;
        PageName = pageName;
    }

    public override string ToString()
    {
        return $"FullName: {FullName}, ExcelName: {ExcelName}, PageName: {PageName}";
    }
}

public static class FileNameHelper
{
    public static string DecodeFileName(string contentDisposition)
    {
        string fileName = "data";//如果解析不出標題則預設fileName為data

        if (!string.IsNullOrEmpty(contentDisposition))
        {
            // 嘗試解析 RFC 5987 編碼（filename*=UTF-8''）
            var rfc5987Match = Regex.Match(contentDisposition, @"filename\*\=([^']*)''(?<filename>.+)");
            if (rfc5987Match.Success)
            {
                // 提取並解碼 URL 編碼的檔名
                fileName = UnityWebRequest.UnEscapeURL(rfc5987Match.Groups["filename"].Value);
            }
            else
            {
                // 嘗試解析傳統的 filename 欄位
                var filenameMatch = Regex.Match(contentDisposition, @"filename=""(?<filename>.+)""");
                if (filenameMatch.Success)
                {
                    // 提取檔名並進行 URL 解碼
                    fileName = UnityWebRequest.UnEscapeURL(filenameMatch.Groups["filename"].Value);
                }
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                Debug.Log("Downloaded file name: " + fileName);
            }
            else
            {
                Debug.LogWarning("Failed to parse file name from Content-Disposition.");
            }
        }
        else
        {
            Debug.LogWarning("Content-Disposition header not found.");
        }
        return fileName;
    }
        // 靜態方法，處理檔名
        public static CSVFileName ProcessFileName(string fileName)
    {
        // 儲存完整檔名
        string fullName = fileName;

        // 1. 去掉 ".csv" 副檔名
        if (fileName.EndsWith(".csv"))
        {
            fileName = fileName.Substring(0, fileName.Length - 4);
        }

        // 2. 使用 " - " 分割檔名
        string[] nameParts = fileName.Split(new string[] { " - " }, System.StringSplitOptions.None);

        if (nameParts.Length == 2)
        {
            // 取得 excelName 和 pageName
            string excelName = nameParts[0];
            string pageName = nameParts[1];

            Debug.Log($"Full Name: {fullName}");
            Debug.Log($"Excel Name: {excelName}");
            Debug.Log($"Page Name: {pageName}");

            // 回傳包含完整檔名、excelName 和 pageName 的 FileName 物件
            return new CSVFileName(fullName, excelName, pageName);
        }
        else
        {
            Debug.LogError("File name format is incorrect. Expected format: 'excelName - pageName.csv'");
            return null; // 如果格式錯誤，回傳 null
        }
    }
}
