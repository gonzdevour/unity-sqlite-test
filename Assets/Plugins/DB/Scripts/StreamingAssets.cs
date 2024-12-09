using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using UnityEditor;
using System.Text;

public class StreamingAssets : MonoBehaviour
{
    public string GetFilePath(string fileName)
    {
        string filePath;
        // 根据平台生成不同的路径
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // 使用 UnityWebRequest 的方式读取
            filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        }
        else
        {
            // PC 或其他平台直接访问文件系统
            filePath = "file:///" + Path.Combine(Application.streamingAssetsPath, fileName);
        }
        return filePath;
    }

    public IEnumerator LoadFile(string fileName, Action<byte[]> onFileLoaded)
    {
        string filePath = GetFilePath(fileName);
        using (UnityWebRequest request = UnityWebRequest.Get(filePath))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load file: {request.error}");
                onFileLoaded?.Invoke(null);
            }
            else
            {
                onFileLoaded?.Invoke(request.downloadHandler.data);
            }
        }
    }

    public IEnumerator LoadExcel(string fileName, System.Action<List<Dictionary<string, string>>> callback = null)
    {
        byte[] fileData = null;
        yield return LoadFile(fileName, data => fileData = data);

        if (fileData != null)
        {
            Debug.Log($"File loaded successfully, size: {fileData.Length} bytes");
            List<Dictionary<string, string>> returnValue = ParseExcelToCsv(fileData); //使用 NPOI 解析 Excel
            callback?.Invoke(returnValue);
        }
        else
        {
            Debug.LogError($"Failed to load Excel file: {fileName}");
            callback?.Invoke(null);
        }
    }

    public IEnumerator LoadDocx(string fileName, System.Action<Dictionary<string, string>> callback = null)
    {
        byte[] fileData = null;
        yield return LoadFile(fileName, data => fileData = data);

        if (fileData != null)
        {
            Debug.Log($"File loaded successfully, size: {fileData.Length} bytes");
            Dictionary<string, string> returnValue = ParseDocx(fileData); //使用 NPOI 解析 Excel
            callback?.Invoke(returnValue);
        }
        else
        {
            Debug.LogError($"Failed to load Excel file: {fileName}");
            callback?.Invoke(null);
        }
    }

    public IEnumerator LoadTxt(string fileName, System.Action<Dictionary<string, string>> callback = null)
    {
        byte[] fileData = null;
        yield return LoadFile(fileName, data => fileData = data);

        if (fileData != null)
        {
            Debug.Log($"File loaded successfully, size: {fileData.Length} bytes");
            Dictionary<string, string> returnValue = ParseTxt(fileData); //解析 txt
            callback?.Invoke(returnValue);
        }
        else
        {
            Debug.LogError($"Failed to load txt file: {fileName}");
            callback?.Invoke(null);
        }
    }

    /// <summary>
    /// 解析 .xlsx 檔案，將每個分頁轉換為 Dictionary
    /// </summary>
    /// <param name="fileData">Excel 檔案的 byte[] 資料</param>
    /// <returns>包含每個分頁的 Dictionary 陣列</returns>
    public static List<Dictionary<string, string>> ParseExcelToCsv(byte[] fileData)
    {
        List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

        try
        {
            // 將 byte[] 轉換為 MemoryStream
            using (MemoryStream memoryStream = new MemoryStream(fileData))
            {
                // 使用 NPOI 讀取 Excel 檔案
                IWorkbook workbook = new XSSFWorkbook(memoryStream);

                // 遍歷每個工作表
                for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                {
                    ISheet sheet = workbook.GetSheetAt(sheetIndex);
                    string pageName = sheet.SheetName;
                    List<string> csvLines = new List<string>();

                    // 遍歷每一行
                    for (int i = 0; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        List<string> rowData = new List<string>();

                        if (row != null)
                        {
                            // 遍歷每個儲存格
                            for (int j = 0; j < row.LastCellNum; j++)
                            {
                                NPOI.SS.UserModel.ICell cell = row.GetCell(j);
                                if (cell != null)
                                {
                                    // 根據儲存格類型讀取值
                                    rowData.Add(cell.ToString());
                                }
                                else
                                {
                                    rowData.Add(""); // 若儲存格為空，添加空字串
                                }
                            }
                        }

                        // 將行資料轉換為 CSV 格式
                        csvLines.Add(string.Join(",", rowData));
                    }

                    // 將分頁名稱和 CSV 數據存入 Dictionary
                    Dictionary<string, string> pageData = new Dictionary<string, string>
                    {
                        { "PageName", pageName },
                        { "CSVData", string.Join("\n", csvLines) } // 將所有行拼接為完整的 CSV
                    };

                    result.Add(pageData);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing Excel file: {ex.Message}");
        }

        return result;
    }

    public static Dictionary<string, string> ParseDocx(byte[] fileData)
    {
        using (MemoryStream stream = new MemoryStream(fileData))
        {
            XWPFDocument docx = new XWPFDocument(stream);
            string textData = "";

            foreach (var paragraph in docx.Paragraphs)
            {
                textData += paragraph.Text + "\n";
            }
            Dictionary<string, string> returnValue = new()
            {
                { "TextData", textData },
            };

            return returnValue;
        }
    }

    public static Dictionary<string, string> ParseTxt(byte[] fileData)
    {
        // 假设文件编码为 UTF-8，可以根据需求调整编码
        string textData = Encoding.UTF8.GetString(fileData);
        Dictionary<string, string> returnValue = new()
            {
                { "TextData", textData },
            };

        return returnValue;
    }
}
