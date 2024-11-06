﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 定义一个与 events 表结构相匹配的类
public class Event
{
    public int Id { get; set; }
    public string 日期 { get; set; }
    public int 日期編號 { get; set; }
    public string 歷史 { get; set; }
}

public class Demo : MonoBehaviour
{
    public CSVDownloader csvDownloader;

    private SQLiteManager dbManager;

    void Start()
    {
        StartCoroutine(Test());
        //// 创建表
        //string[] headers = { "Name", "Score" };
        //dbManager.CreateTable("players", headers);
    }
    public IEnumerator Test()
    {
        // 下載所有列表中的csv，並依表單header和檔名各自加進db裡
        //yield return csvDownloader.DownloadAllCSV(csvDownloader.csvUrls);
        // 不給值就直接使用csvDownloader.csvUrls
        yield return csvDownloader.DownloadAllCSV();
        // 傳入db檔案連結，初始化dbManager
        dbManager = new SQLiteManager(Path.Combine(Application.persistentDataPath, "dynamicDatabase.db"));
        Db_PrintAll("events");
        Db_InsertFromHeaderValue();
        Db_PrintAll("events");
        Db_InsertFromObj();
        Db_PrintAll("events");
        Db_Query();

        Db_Delete();
        Db_PrintAll("events");
        Db_Update();
        Db_PrintAll("events");
    }
    public void Db_PrintAll(string pageName) 
    {
        // 基於Event class取得"events"表單中的資料映射到allEvents物件中
        List<Event> allEvents = dbManager.QueryTable<Event>(pageName);
        // 印出allEvents物件中的各列指定內容
        foreach (var eventItem in allEvents)
        {
            Debug.Log($"Event: {eventItem.歷史}, Date: {eventItem.日期}");
        }
    }
    public void Db_InsertFromHeaderValue()
    {
        Debug.Log("---insert header-value---");
        // 依header-value插入数据
        string[] headers = { "日期", "日期編號", "歷史" };
        object[] values = { "2024-03-08", 20240308, "婦女節" };
        dbManager.InsertData("events", headers, values);
    }
    public void Db_InsertFromObj()
    {
        Debug.Log("---insert obj---");
        Event myEvent = new()
        {
            日期 = "2024-01-01",
            日期編號 = 20240101,
            歷史 = "新年慶祝活動"
        };
        // 依物件插入数据
        dbManager.InsertDataFromObject("events", myEvent);
    }
    public void Db_Query()
    {
        Debug.Log("---query---");
        // 查詢數據
        // 基於Event class取得"events"表單中的資料映射到allEvents物件中
        List<Event> allEvents = dbManager.QueryTable<Event>("events", "日期編號 < 20240101", "日期編號 ASC");//DESC
        // 印出allEvents物件中的各列指定內容
        foreach (var eventItem in allEvents)
        {
            Debug.Log($"Event: {eventItem.歷史}, Date: {eventItem.日期}");
        }
    }
    public void Db_Delete()
    {
        Debug.Log("---delete---");
        // 刪除數據
        dbManager.DeleteData("events", "日期編號 < 20231231");
    }
    public void Db_Update()
    {
        Debug.Log("---update---");
        // 更新數據
        string[] headers = { "日期", "歷史" };
        string[] values = { "2024-02-14", "情人節" };
        dbManager.UpdateData("events", headers, values, "日期編號 = 20240101");//依id更新："Id = 1"
    }
}