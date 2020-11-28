using SimpleJson;
using System.Collections.Generic;
using System.Linq;

public interface IDataStorageObject
{
    string Name { get; }
    void Init(JsonObject json);
}

public interface IDataStorage
{
    bool IsInited();
    string GetStorageName();
    void Init(JsonArray json);
    void Reset();
}

public class BaseDataStorage<TDataObj, TDataStorage> : IDataStorage where TDataObj : IDataStorageObject, new() where TDataStorage : IDataStorage, new()
{
    // Singleton
    private static TDataStorage m_Instance;
    public static TDataStorage Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new TDataStorage();
            }

            return m_Instance;
        }
    }

    // DataStorage
    protected readonly Dictionary<string, TDataObj> m_Data = new Dictionary<string, TDataObj>();
    private readonly string m_StorageName;
    private bool m_IsInited;

    public BaseDataStorage(string storageName)
    {
        m_StorageName = storageName;
    }

    public string GetStorageName()
    {
        return m_StorageName;
    }

    public void Init(JsonArray json)
    {
        if (m_IsInited)
            return;

        foreach (JsonObject objectData in json)
        {
            TDataObj obj = new TDataObj();

            obj.Init(objectData);
            m_Data.Add(obj.Name, obj);

            DataStoreObjectReaded(obj);
        }

        m_IsInited = true;
    }

    public virtual void Reset()
    {
        m_Data.Clear();
        m_IsInited = false;
    }
    
    public TDataObj GetByName(string name)
    {
        TDataObj res = default(TDataObj);
        if (name == null)
            return res;

        m_Data.TryGetValue(name, out res);
        return res;
    }
    
    public List<TDataObj> GetData()
    {
        return m_Data.Values.ToList();
    }

    protected virtual void DataStoreObjectReaded(TDataObj obj)
    { }

    public bool IsInited()
    {
        return m_IsInited;
    }
}
