using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EXPController : MonoBehaviour
{
    [SerializeField] private EXPView expView;
    private EXPModel expModel;

    private void Awake() 
    {
        expModel = new EXPModel();

        expModel.OnChangeCurValue += expView.UpdateEXPValue;
        expModel.OnChangeMaxValue += expView.SetMaxValue;
        expModel.OnChangeLevel += expView.UpdateLevelText;

        EventCenter.AddListener<PlayerStatsSO>(EventNameTable.ONSEEDPLAYERSO,SetModel);
        EventCenter.AddListener<int>(EventNameTable.ONEXPCURUPDATE,SetCurValue);
        EventCenter.AddListener<int>(EventNameTable.ONEXPMAXBOOST,SetMaxValue);
        EventCenter.AddListener<int>(EventNameTable.ONLEVELBOOST,SetLevel);
    }

    private void Start() 
    {
        expModel.InitUI();    
    }

    public void SetModel(PlayerStatsSO playerSO)
    {
        expModel.CurValue = playerSO.curExp;
        expModel.MaxValue = playerSO.growExp;
        expModel.Level = playerSO.level;
    }

    private void SetLevel(int value)
    {
        Debug.Log("Level:" + value);
        expModel.Level = value;
    }

    private void SetCurValue(int value)
    {
        expModel.CurValue = value;
    }

    private void SetMaxValue(int value)
    {
        expModel.MaxValue = value;
    }
    
    private void OnDestroy() 
    {
        expModel.OnChangeCurValue -= expView.UpdateEXPValue;
        expModel.OnChangeMaxValue -= expView.SetMaxValue;
        expModel.OnChangeMaxValue -= expView.UpdateLevelText;

        EventCenter.RemoveListener<int>(EventNameTable.ONEXPCURUPDATE,SetCurValue);
        EventCenter.RemoveListener<int>(EventNameTable.ONEXPMAXBOOST,SetMaxValue);
        EventCenter.RemoveListener<int>(EventNameTable.ONLEVELBOOST,SetLevel);
    }
}
