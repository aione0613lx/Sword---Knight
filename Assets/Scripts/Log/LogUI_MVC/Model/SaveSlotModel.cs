using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSlotModel
{
    private SaveSlotInfo saveSlotInfo;
    private bool hasSave = false;

    public event Action<SaveSlotInfo> OnGainSaveSlotInfo;

    public SaveSlotInfo SaveSlotInfo {
        get {
            return saveSlotInfo;
        }

        set {
            saveSlotInfo = value;
            hasSave = true;
            OnGainSaveSlotInfo?.Invoke(saveSlotInfo);
        }
    }

    public bool HasSave {
        get {
            return hasSave;
        }
    }
}
