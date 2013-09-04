﻿using UnityEngine;
using System.Collections;

public class State : MonoBehaviour
{
    public static State script;
    public GameObject 圖案物件;
    public GameObject 構圖的滑鼠拖曳;
    public GameObject 滑鼠點擊;
    private bool 明暗初始化 = false;
    private bool 設色初始化 = false;
    // Use this for initialization
    void Start()
    {
        script = this;
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameManager.script.CurrentDrawStage)
        {
            case GameManager.DrawStage.等待中:
                break;

            case GameManager.DrawStage.構圖:
                break;

            case GameManager.DrawStage.明暗:
                if (!明暗初始化)
                {
                    明暗初始化 = true;
                    構圖的滑鼠拖曳.SetActive(false);
                    滑鼠點擊.SetActive(true);
                    //將馬跟樹閃爍
                    foreach (var pi in 圖案物件.GetComponentsInChildren<PictureInfo>())
                    {
                        if (!pi.isUsed)
                        {
                            pi.gameObject.SetActive(false);
                        }
                        else
                        {
                            if (pi.Type == PictureInfo.PictureType.馬樹)
                            {
                                pi.isBlink = true;
                                pi.CanPick = true;
                                if (!pi.GetComponent<iTween>())
                                    iTween.ValueTo(pi.gameObject, iTween.Hash("name", "PickObject", "from", 1, "to", 0.2, "time", 0.5, "loopType", "pingPong", "onupdatetarget", this.gameObject, "onupdate", "changePictureAlpha"));

                            }
                        }
                    }
                }
                break;

            case GameManager.DrawStage.設色:

                if (!設色初始化)
                {
                    設色初始化 = true;
                    //將馬跟樹閃爍
                    foreach (var pi in 圖案物件.GetComponentsInChildren<PictureInfo>())
                    {
                        if (!pi.isUsed)
                        {
                            pi.gameObject.SetActive(false);
                        }
                        else
                        {
                            if (pi.Type == PictureInfo.PictureType.馬樹)
                            {
                                pi.isBlink = true;
                                pi.CanPick = true;
                                if (!pi.GetComponent<iTween>())
                                    iTween.ValueTo(pi.gameObject, iTween.Hash("name", "PickObject", "from", 1, "to", 0.2, "time", 0.5, "loopType", "pingPong", "onupdatetarget", this.gameObject, "onupdate", "changePictureAlpha"));

                            }
                        }
                    }
                }
                break;

            case GameManager.DrawStage.淡化:
                break;

            case GameManager.DrawStage.光源:
                break;
        }
    }

    void changePictureAlpha(float newValue)
    {
        foreach (var pi in 圖案物件.GetComponentsInChildren<PictureInfo>())
        {
            if (pi.isBlink)
            {
                pi.gameObject.GetComponent<SmoothMoves.Sprite>().SetColor(new Color(1, 1, 1, 0.9f - newValue));
            }
        }
    }
}