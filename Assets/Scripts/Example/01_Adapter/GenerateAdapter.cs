using UnityEditor;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

public class GenerateAdapter : MonoBehaviour
{
    AppDomain Appdomain;
    //热更新的开头实例
    private SubMonoBehavior _hotFixVrCoreEntity;

    void Start()
    {
        Appdomain = new AppDomain();
        LoadHotFixCode("Assets/Game/HotFix/Hotfixdll.bytes", "Assets/Game/HotFix/Hotfix.dll.pdb.bytes");
        if(_hotFixVrCoreEntity!=null)
            _hotFixVrCoreEntity.Start();
    }

    void Update()
    {
        if (_hotFixVrCoreEntity != null)
            _hotFixVrCoreEntity.Update();
    }

    private void FixedUpdate()
    {
        if (_hotFixVrCoreEntity != null)
            _hotFixVrCoreEntity.OnFixedUpdate();
    }

    private void OnDestroy()
    {
        if (_hotFixVrCoreEntity != null)
            _hotFixVrCoreEntity.OnDestroy();
    }

    private void OnApplicationQuit()
    {
        if (_hotFixVrCoreEntity != null)
            _hotFixVrCoreEntity.OnApplicationQuit();
    }

    private bool LoadHotFixCode(string dllpath, string pdbpath)
    {
        Debug.Log(dllpath);
        //资源加载
        byte[] dll = AssetDatabase.LoadAssetAtPath<TextAsset>(dllpath).bytes;
        if (dll == null)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log("load hotfix code dll fail!");
            return false;
#endif
        }
        byte[] pdb = null;
#if UNITY_EDITOR
        //pdb不是AB文件,单独流程加载
        UnityEngine.Debug.Log(pdbpath);
        pdb = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(pdbpath).bytes;
        Appdomain.DebugService.StartDebugService(56000);
#endif
        LoadHotfixAssembly(dll, pdb);

        return true;
    }

    private void LoadHotfixAssembly(byte[] dll, byte[] pdb = null)
    {
        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(dll))
        {
            if (pdb == null)
                Appdomain.LoadAssembly(ms, null, new Mono.Cecil.Pdb.PdbReaderProvider());
            else
                using (System.IO.MemoryStream p = new System.IO.MemoryStream(pdb))
                {
                    Appdomain.LoadAssembly(ms, p, new Mono.Cecil.Pdb.PdbReaderProvider());
                }
        }

        //初始化ILRuntime
        InitializeILRuntime();
        //运行热更新的入口
        RunHotFixVrCoreEntity();
    }

    private void RunHotFixVrCoreEntity()
    {
        //热更代码入口处
        _hotFixVrCoreEntity = Appdomain.Instantiate<SubMonoBehavior>("Hotfix.HotFixMode");
#if UNITY_EDITOR
        if (_hotFixVrCoreEntity == null)
        {
            UnityEngine.Debug.LogError("_hotFixVrCoreEntity == null \n 无法进入热更代码!");
        }
#endif
    }

    void InitializeILRuntime()
    {
        ILRuntime.ILRuntimeHelper.Init(Appdomain);
    }


}
