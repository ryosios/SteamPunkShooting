using UnityEngine;

public class ResultMaster : MonoBehaviour
{
    [SerializeField] private UIResult _uIResult;
    public SaveData _loadData { get; set; }
    private void Awake()
    {
        _loadData = SaveSystem.Load();

        if (_loadData != null)
        {
            Debug.Log($"ステージとポイント: {_loadData._stage1AndPoint},現在のステージ: {_loadData._nowStage}, クリアしたかどうかのフラグ: {_loadData._isStageCleared}");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //スコアを表示
        _uIResult.SetPoint(_loadData._stage1AndPoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
