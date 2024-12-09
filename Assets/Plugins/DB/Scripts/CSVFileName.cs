// FileNameHelper.cs
using System.Net.Mime;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class CSVFileName
{
    public string FullName { get; set; }  // ��l�ɦW�]�t .csv�^
    public string ExcelName { get; set; } // �ɮפ��� Excel �W�ٳ���
    public string PageName { get; set; }  // �ɮפ��������W�ٳ���

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
        string fileName = "data";//�p�G�ѪR���X���D�h�w�]fileName��data

        if (!string.IsNullOrEmpty(contentDisposition))
        {
            // ���ոѪR RFC 5987 �s�X�]filename*=UTF-8''�^
            var rfc5987Match = Regex.Match(contentDisposition, @"filename\*\=([^']*)''(?<filename>.+)");
            if (rfc5987Match.Success)
            {
                // �����øѽX URL �s�X���ɦW
                fileName = UnityWebRequest.UnEscapeURL(rfc5987Match.Groups["filename"].Value);
            }
            else
            {
                // ���ոѪR�ǲΪ� filename ���
                var filenameMatch = Regex.Match(contentDisposition, @"filename=""(?<filename>.+)""");
                if (filenameMatch.Success)
                {
                    // �����ɦW�öi�� URL �ѽX
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
        // �R�A��k�A�B�z�ɦW
        public static CSVFileName ProcessFileName(string fileName)
    {
        // �x�s�����ɦW
        string fullName = fileName;

        // 1. �h�� ".csv" ���ɦW
        if (fileName.EndsWith(".csv"))
        {
            fileName = fileName.Substring(0, fileName.Length - 4);
        }

        // 2. �ϥ� " - " �����ɦW
        string[] nameParts = fileName.Split(new string[] { " - " }, System.StringSplitOptions.None);

        if (nameParts.Length == 2)
        {
            // ���o excelName �M pageName
            string excelName = nameParts[0];
            string pageName = nameParts[1];

            Debug.Log($"Full Name: {fullName}");
            Debug.Log($"Excel Name: {excelName}");
            Debug.Log($"Page Name: {pageName}");

            // �^�ǥ]�t�����ɦW�BexcelName �M pageName �� FileName ����
            return new CSVFileName(fullName, excelName, pageName);
        }
        else
        {
            Debug.LogError("File name format is incorrect. Expected format: 'excelName - pageName.csv'");
            return null; // �p�G�榡���~�A�^�� null
        }
    }
}
