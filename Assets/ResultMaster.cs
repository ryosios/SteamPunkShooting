using UnityEngine;

public class ResultMaster : MonoBehaviour
{
    private void Awake()
    {
        SaveData loaded = SaveSystem.Load();

        if (loaded != null)
        {
            Debug.Log($"ステージとポイント: {loaded._stage1AndPoint},現在のステージ: {loaded._nowStage}, クリアしたかどうかのフラグ: {loaded._isStageCleared}");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
