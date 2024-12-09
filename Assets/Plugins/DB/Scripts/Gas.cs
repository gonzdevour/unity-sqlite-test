using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using UnityEngine.WSA;
using System.Security.Cryptography;

public class Gas : MonoBehaviour
{
    public List<string> GoogleFileIDs;

    public IEnumerator GetGoogleSheetsAsCSV(List<string> sheetIdArr, System.Action<Dictionary<string, string>> callback = null)
    {
        string GasUrl = "https://script.google.com/macros/s/AKfycbxB2GPvA1XKl7z-sMeuHsrfhdqgnJrB3Mo6xV04_51lKmj_IeB5RnHNvyM1dZg0hj-LBg/exec";
        var dataToRequest = new { sheetIdArr = sheetIdArr }; //建立doGet所需要的parameters物件

        string jsonData = JsonConvert.SerializeObject(dataToRequest); //Debug.Log(jsonData);
        string encodedJsonString = UnityWebRequest.EscapeURL(jsonData);
        string url = $"{GasUrl}?data={encodedJsonString}"; // 建立完整的request docId
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"GetGoogleSheetsAsCSV success");
            Debug.Log($"{request.downloadHandler.text}");
            string textData = request.downloadHandler.text;
            var returnValue = new Dictionary<string, string>
            {
                { "TextData", textData },
            };
            callback?.Invoke(returnValue);
        }
        else
        {
            Debug.LogError("Failed to GetGoogleSheetsAsCSV : " + request.error);
        }
    }
    public IEnumerator GetDocsInGoogleDriveFolder(string folderId, System.Action<Dictionary<string, string>> callback = null)
    {
        string GasUrl = "https://script.google.com/macros/s/AKfycbzWebO96vZCKbVUL0dMemnZm852WgyzYtT8zzlMKFzik5IT7rsFKBA8_sUf51NiMoyoHA/exec";
        var dataToRequest = new { folderId = folderId }; //建立doGet所需要的parameters物件

        string jsonData = JsonConvert.SerializeObject(dataToRequest); //Debug.Log(jsonData);
        string encodedJsonString = UnityWebRequest.EscapeURL(jsonData);
        string url = $"{GasUrl}?data={encodedJsonString}"; // 建立完整的request docId
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"GetDocsInGoogleDriveFolder success");
            Debug.Log($"{request.downloadHandler.text}");
            string textData = request.downloadHandler.text;
            var returnValue = new Dictionary<string, string>
            {
                { "TextData", textData },
            };
            callback?.Invoke(returnValue);
        }
        else
        {
            Debug.LogError("Failed to GetDocsInGoogleDriveFolder : " + request.error);
        }
    }

    public IEnumerator GetGoogleDoc(string docId, System.Action<Dictionary<string, string>> callback = null)
    {
        string GasUrl = "https://script.google.com/macros/s/AKfycbxH5HrHZxKAAX0M5PjNnDCbFm0yXikH8Tw1P5ptKQ2m6CN5tYDD8cPUSD1DO2ymRgpa2Q/exec";
        var dataToRequest = new { docId = docId }; //建立doGet所需要的parameters物件

        string jsonData = JsonConvert.SerializeObject(dataToRequest); //Debug.Log(jsonData);
        string encodedJsonString = UnityWebRequest.EscapeURL(jsonData);
        string url = $"{GasUrl}?data={encodedJsonString}"; // 建立完整的request docId
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"GetGoogleDoc success");
            Debug.Log($"{request.downloadHandler.text}");
            string textData = request.downloadHandler.text;
            var returnValue = new Dictionary<string, string>
            {
                { "TextData", textData },
            };
            callback?.Invoke(returnValue);
        }
        else
        {
            Debug.LogError("Failed to GetGoogleDoc: " + request.error);
        }
    }

    // 协程：并行下载所有文件并等待完成
    public IEnumerator GetGoogleDocAll(List<string> docIds, System.Action<Dictionary<string, string>> callback = null) // 設定可選參數為 null
    {
        // 如果沒有提供 docIds 參數，則使用 GoogleFileIDs
        List<string> IDsToDownload = docIds ?? GoogleFileIDs;

        List<IEnumerator> downloadTasks = new();

        foreach (string docId in IDsToDownload)
        {
            downloadTasks.Add(GetGoogleDoc(docId, callback));
        }

        // 等待所有下載任務完成
        foreach (var task in downloadTasks)
        {
            yield return task; // 逐個等待所有任務完成
        }

        Debug.Log("All GetGoogleDoc tasks are complete.");
    }
}
