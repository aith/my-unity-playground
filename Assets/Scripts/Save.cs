using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }
    public void SaveGame()
    {
        SavingService.SaveGame("saveGame.json");
        
    }
    public void LoadGame()
    {
        SavingService.LoadGame("saveGame.json");
        
    }
}
