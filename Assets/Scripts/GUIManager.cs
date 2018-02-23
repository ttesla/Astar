//----------------------------------------------
// File: GUIManager.cs
// Copyright © 2018 InsertCoin (www.insertcoin.info)
// Author: Omer Akyol
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour 
{
    #region EditorFields

    public MapManager MapMan;
    public Button StartSimButton;
    public Button ResetSimButton;
    public Toggle ObsToggle;
    public Toggle StartToggle;
    public Toggle EndToggle;
    public Text LogMessagesText;

    #endregion

    #region Fields
    #endregion

    #region UnityMethods

    void Awake()
    {
        StartSimButton.onClick.AddListener(() =>    
        {
            string errMsg;

            if (MapMan.StartSimulation(out errMsg))
            {
                LogMessagesText.text  = errMsg;
                LogMessagesText.color = Color.black;
            }
            else
            {
                LogMessagesText.text = errMsg;
                LogMessagesText.color = Color.red;
            }
            
        });

        ResetSimButton.onClick.AddListener(() =>    { MapMan.ResetSimulation(); });
        ObsToggle.onValueChanged.AddListener(x =>   { if (x) MapMan.SetBrush(true, false, false); });
        StartToggle.onValueChanged.AddListener(x => { if (x) MapMan.SetBrush(false, true, false); });
        EndToggle.onValueChanged.AddListener(x =>   { if (x) MapMan.SetBrush(false, false, true); });
    }

    void Start () 
    {
        MapMan.SetBrush(true, false, false);
    }

    #endregion
}
