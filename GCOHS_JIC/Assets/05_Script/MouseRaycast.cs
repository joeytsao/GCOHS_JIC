﻿using UnityEngine;
using System.Collections;

//提供給　操作區　的貓斯射線
//點擊後　直接呼叫　"ClickObject" 的　Target　裡面改　SmoothMove的圖

public class MouseRaycast : MonoBehaviour
{

    public static MouseRaycast script;
    public Camera ViewCamera;
    private RaycastHit hit;
    public GameObject MouseTarget;
    public PictureType pictureType;
    public SetColorBoneAnimation.PictureType 潑墨顏色;
    public bool isBlink;


    //九月九號新增 當畫筆被點擊後 才能開啟變色功能
    private static bool 是否可以對操作區的物件上色 = false;

    // Use this for initialization
    void Start()
    {
        script = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.GetComponent<SmoothMoves.Sprite>())
        {
            switch (pictureType)
            {
                // 處理潑墨部分的閃爍
                case MouseRaycast.PictureType.潑墨:

                    if (!GetComponent<iTween>() && this.isBlink)
                    {
                        this.gameObject.GetComponent<SmoothMoves.Sprite>().color = new Color(1, 1, 1, 1);
                        iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.6, "to", 0, "time", 1.0, "loopType", "pingPong", "onupdate", "changePictureAlpha"));
                    }
                    else if (!this.isBlink)
                    {
                        iTween.Stop(this.gameObject);
                        this.gameObject.GetComponent<SmoothMoves.Sprite>().color = new Color(1, 1, 1, 1);
                        this.gameObject.GetComponent<SmoothMoves.Sprite>().UpdateArrays();
                    }
                    break;

                // 處理操作區物件的閃爍
                case MouseRaycast.PictureType.操作區物件:

                    //if (!GetComponent<iTween>() && 是否可以對操作區的物件上色)
                    //{
                    //    this.gameObject.GetComponent<SmoothMoves.Sprite>().color = new Color(1, 1, 1, 1);
                    //    // iTween.ValueTo(this.gameObject, iTween.Hash("from", 1, "to", 0.2, "time", 0.5, "loopType", "pingPong", "onupdate", "changePictureAlpha"));
                    //}
                    //else if (!是否可以對操作區的物件上色)
                    //{
                    //    //iTween.Stop(this.gameObject);
                    //    this.gameObject.GetComponent<SmoothMoves.Sprite>().color = new Color(1, 1, 1, 1);
                    //    this.gameObject.GetComponent<SmoothMoves.Sprite>().UpdateArrays();
                    //}
                    break;
            }
        }

        RaycastHit[] hits = Physics.RaycastAll(this.ViewCamera.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, 1), 1000);

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject == this.gameObject)
                {
                    if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
                    {
                        MouseTarget = hit.transform.gameObject;


                        if (pictureType == PictureType.潑墨)
                        {
                            //引導動畫部分
                            if (ClickObject.script.HintAnimationisInit)
                            {

                                if (GameManager.script.CurrentDrawStage == GameManager.DrawStage.明暗 || GameManager.script.CurrentDrawStage == GameManager.DrawStage.設色)
                                    PlayHandBoneAnimation.script.animationType = PlayHandBoneAnimation.AnimationType.指向引導_畫布區物件;
                                //if (GameManager.script.CurrentDrawStage == GameManager.DrawStage.淡化)
                                //    PlayHandBoneAnimation.script.animationType = PlayHandBoneAnimation.AnimationType.指向引導_土坡類;

                                //PlayHintBoneAnimation.script.animationType = PlayHintBoneAnimation.AnimationType.操作閃爍圖片;
                            }


                            if (isBlink)
                            {
                                isBlink = false;

                                //將其他潑墨閃爍
                                if (GameManager.script.CurrentDrawStage == GameManager.DrawStage.設色)
                                {
                                    //存下目前要設定的顏色
                                    GameManager.script.設色潑墨顏色 = this.潑墨顏色;

                                    //將其他潑墨閃爍
                                    foreach (Transform child in this.transform.parent.parent)
                                    {
                                        foreach (Transform innerchild in child)
                                        {
                                            if (innerchild != this.transform)
                                                innerchild.gameObject.GetComponent<MouseRaycast>().isBlink = true;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (Transform child in this.transform.parent)
                                    {
                                        if (child != this.transform)
                                            child.gameObject.GetComponent<MouseRaycast>().isBlink = true;
                                    }
                                }
                                是否可以對操作區的物件上色 = true;
                            }
                        }

                        if (pictureType == PictureType.操作區物件)
                        {
                            if (是否可以對操作區的物件上色)
                            {
                                if (GameManager.script.CurrentDrawStage == GameManager.DrawStage.明暗)
                                {
                                    // if(this.GetComponent<Step2>().enabled) 使目前開啟Step2程式的物件才能被潑墨改變顏色 ， 反之點任何物件都會換色
                                    if (this.GetComponent<Step2>())
                                    {
                                        if (this.GetComponent<Step2>().enabled)
                                        {
                                            ClickObject.script.SetPictureStep2(MouseTarget);
                                            是否可以對操作區的物件上色 = false;
                                            PlayHandBoneAnimation.script.animationType = PlayHandBoneAnimation.AnimationType.空動畫;
                                        }
                                    }
                                }
                                if (GameManager.script.CurrentDrawStage == GameManager.DrawStage.設色)
                                {
                                    //改變操作區操作物件的圖（Animation）

                                    SetColorBoneAnimation.script.pictureType = GameManager.script.設色潑墨顏色;
                                    GameManager.script.設色潑墨顏色 = SetColorBoneAnimation.script.pictureType;

                                    //改變實際操作物件的圖（Sprite）
                                    if (this.GetComponent<Step3>())
                                    {
                                        if (this.GetComponent<Step3>().enabled)
                                        {
                                            ClickObject.script.SetPictureStep3(MouseTarget);
                                            是否可以對操作區的物件上色 = false;
                                            PlayHandBoneAnimation.script.animationType = PlayHandBoneAnimation.AnimationType.空動畫;
                                        }
                                    }
                                }
                                if (GameManager.script.CurrentDrawStage == GameManager.DrawStage.淡化)
                                {
                                    if (this.GetComponent<Step4>().enabled)
                                    {
                                        ClickObject.script.SetPictureStep4();
                                        是否可以對操作區的物件上色 = false;
                                        PlayHandBoneAnimation.script.animationType = PlayHandBoneAnimation.AnimationType.空動畫;
                                    }
                                }



                                //已經引導過第一次的手指動畫 第二次將消失
                                ClickObject.script.HintAnimationisInit = false;
                                //PlayHandBoneAnimation.script.animationType = PlayHandBoneAnimation.AnimationType.空動畫;
                                //PlayHintBoneAnimation.script.animationType = PlayHintBoneAnimation.AnimationType.空動畫;

                                break;
                            }
                        }
                    }
                }
            }
        }
        //if (Physics.Raycast(this.ViewCamera.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, 1), out this.hit, 1000))
        //{
        //    if (hit.transform.gameObject == this.gameObject)
        //    {


        //    }
        //}
    }

    void changePictureAlpha(float newValue)
    {
        this.gameObject.GetComponent<SmoothMoves.Sprite>().SetColor(new Color(1, 1, 1, 1f - newValue));
    }

    void OnEnable()
    {
        this.isBlink = true;
    }

    public enum PictureType
    {
        潑墨 = 0, 操作區物件 = 1
    }
}
