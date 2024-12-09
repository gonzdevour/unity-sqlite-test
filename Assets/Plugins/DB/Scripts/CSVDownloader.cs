using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CSVDownloader : MonoBehaviour
{
    public List<string> csvUrls;
    
    public IEnumerator DownloadCSV(string csvUrl, System.Action<Dictionary<string, string>> callback)
    {
        Debug.Log(csvUrl);
        UnityWebRequest request = UnityWebRequest.Get(csvUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // 檢查是否存在 Content-Disposition 標頭
            string contentDisposition = request.GetResponseHeader("Content-Disposition");
            string fileName = FileNameHelper.DecodeFileName(contentDisposition);
            CSVFileName parsedFileName = FileNameHelper.ProcessFileName(fileName);
            string csvData = request.downloadHandler.text;

            var returnValue = new Dictionary<string, string>
            {
                { "PageName", parsedFileName.PageName },
                { "CSVData", csvData },
            };
            callback(returnValue);
        }
        else
        {
            Debug.LogError("Failed to download CSV: " + request.error);
        }
    }

    // 协程：并行下载所有文件并等待完成
    public IEnumerator DownloadAllCSV(List<string> urls, System.Action<Dictionary<string, string>> callback) // 設定可選參數為 null
    {
        // 如果沒有提供 urls 參數，則使用 GoogleFileIDs
        List<string> urlsToDownload = urls ?? csvUrls;

        List<IEnumerator> downloadTasks = new();

        foreach (string url in urlsToDownload)
        {
            downloadTasks.Add(DownloadCSV(url, callback));
        }

        // 等待所有下載任務完成
        foreach (var task in downloadTasks)
        {
            yield return task; // 逐個等待所有任務完成
        }

        Debug.Log("All csv downloaded.");
    }

    //void SaveCSVToFile(string csvData)
    //{
    //    string filePath = Path.Combine(Application.persistentDataPath, "data.csv");
    //    File.WriteAllText(filePath, csvData);
    //    Debug.Log("CSV saved at: " + filePath);
    //}
}
