using System;
using System.Collections.Generic;

/// <summary>
/// CSV 로딩 → PageData 변환 → View 타입별 관리
///
/// 사용법:
///     var repo = new DataRepository();
///     PageData data = repo.GetData<ContentView>();
///     string title = data.Get("Title");
/// </summary>
public class DataRepository
{
    /// <summary>
    /// 필드
    /// </summary>
    private string _startDataFile = "StartData.csv";
    private string _contentDataFile = "ContentData.csv";
    private string _resultDataFile = "ResultData.csv";

    // View 타입 → PageData 매핑
    private Dictionary<Type, PageData> _dataMap = new Dictionary<Type, PageData>();

    public DataRepository()
    {
        LoadAll();
    }

    #region 내부 호출 함수

    /// <summary>
    /// 모든 View에 대응하는 CSV 로딩
    /// </summary>
    private void LoadAll()
    {
        _dataMap[typeof(StartView)]   = LoadPageData(_startDataFile);
        _dataMap[typeof(ContentView)] = LoadPageData(_contentDataFile);
        _dataMap[typeof(ResultView)]  = LoadPageData(_resultDataFile);
    }

    /// <summary>
    /// CSV 파일 → PageData 변환
    /// </summary>
    private PageData LoadPageData(string fileName)
    {
        var pageData = new PageData();
        var raw = CSVParser.Read(fileName);
        pageData.SetFromDictionary(raw);
        return pageData;
    }

    #endregion

    #region 외부 호출 함수

    /// <summary>
    /// View 타입에 대응하는 PageData 조회
    /// </summary>
    /// <typeparam name="TView">BaseView를 상속한 View 타입</typeparam>
    /// <returns>해당 View의 PageData (미등록 시 null)</returns>
    public PageData GetData<TView>() where TView : BaseView
    {
        _dataMap.TryGetValue(typeof(TView), out PageData data);
        return data;
    }

    #endregion
}
