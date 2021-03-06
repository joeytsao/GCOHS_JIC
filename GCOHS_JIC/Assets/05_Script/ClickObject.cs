﻿using UnityEngine;
using System.Collections;

/// <summary>
/// OHS專案第二三四步驟：明暗、設色、光影
/// </summary>
/// 
public class ClickObject : MonoBehaviour
{
    public static ClickObject script;
    public Camera ViewCamera;
    public LayerMask TargetLayer;
    private GameObject Target;
    private RaycastHit[] hits;
    public MouseType currentMouseType = MouseType.初始播放引導動畫;
    RaycastHit innerHit;

    //是否可以選擇下一個　物件　的鎖定狀態
    public bool isLock;

    //提示動畫初始化
    public bool HintAnimationisInit = true;
    // Use this for initialization
    void Start()
    {
        script = this;
    }

    // Update is called once per frame
    void Update()
    {

        ProcessMouseState();
    }

    /// <summary>
    /// 處理滑鼠狀態
    /// </summary>
    private void ProcessMouseState()
    {
        switch (this.currentMouseType)
        {
            case MouseType.初始播放引導動畫:

                if (ClickObject.script.HintAnimationisInit)
                {
                    //播放引導動畫
                    if (GameManager.script.CurrentDrawStage == GameManager.DrawStage.明暗 || GameManager.script.CurrentDrawStage == GameManager.DrawStage.設色)
                        PlayHandBoneAnimation.script.animationType = PlayHandBoneAnimation.AnimationType.指向引導_馬樹類;
                    if (GameManager.script.CurrentDrawStage == GameManager.DrawStage.淡化)
                        PlayHandBoneAnimation.script.animationType = PlayHandBoneAnimation.AnimationType.指向引導_光源土坡類;
                    //PlayHintBoneAnimation.script.animationType = PlayHintBoneAnimation.AnimationType.畫布閃爍圖片;
                }

                this.currentMouseType = MouseType.無狀態;
                break;

            case MouseType.無狀態:
                if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
                    this.currentMouseType = MouseType.點擊;
                break;

            case MouseType.點擊:
                if (!Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1))
                    this.currentMouseType = MouseType.放開;
                break;

            case MouseType.放開:


                //多重物件偵測 偵測目標物件是否閃爍 若無則可以獲取
                hits = Physics.RaycastAll(this.ViewCamera.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, 1), 200, this.TargetLayer);
                if (hits.Length > 0)
                {
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.transform.gameObject.GetComponent<PictureInfo>().isBlink)
                        {
                            //改變動畫 - > 操作區潑墨
                            if (ClickObject.script.HintAnimationisInit)
                            {
                                if (GameManager.script.CurrentDrawStage == GameManager.DrawStage.淡化)
                                    PlayHandBoneAnimation.script.animationType = PlayHandBoneAnimation.AnimationType.指向引導_光源操作區潑墨;
                                else
                                    PlayHandBoneAnimation.script.animationType = PlayHandBoneAnimation.AnimationType.指向引導_操作區潑墨;
                                //PlayHintBoneAnimation.script.animationType = PlayHintBoneAnimation.AnimationType.操作閃爍潑墨;
                            }

                            innerHit = hit;
                            //如果切換圖片後　再解除Lock 才能賦予新的Target

                            //【明暗】與【淡化】部分 鎖定被選定的物件;沒變化前不解鎖
                            if (GameManager.script.CurrentDrawStage == GameManager.DrawStage.明暗 ||
                                GameManager.script.CurrentDrawStage == GameManager.DrawStage.淡化 ||
                                GameManager.script.CurrentDrawStage == GameManager.DrawStage.設色)
                            {
                                if (!this.isLock)
                                {
                                    isLock = true;
                                    ClearControlArea();//清空操作區
                                    this.Target = innerHit.transform.gameObject;
                                }
                            }
                            //【設色】部分 不上鎖
                            //else
                            //{
                            //    if (Target)
                            //    {
                            //        this.Target.GetComponent<PictureInfo>().isBlink = true;
                            //        ClearControlArea(); //清空操作區
                            //        this.Target = innerHit.transform.gameObject; // 給予新的Target
                            //    }

                            //}



                            //停止閃爍 並將顏色還原
                            this.Target.GetComponent<PictureInfo>().isBlink = false;

                            this.Target.GetComponent<SmoothMoves.Sprite>().SetColor(new Color(1, 1, 1, 1));

                            //開啟各階段程式
                            if (this.Target.GetComponent<Step2>()) this.Target.GetComponent<Step2>().enabled = GameManager.script.CurrentDrawStage == GameManager.DrawStage.明暗 ? true : false;
                            if (this.Target.GetComponent<Step3>()) this.Target.GetComponent<Step3>().enabled = GameManager.script.CurrentDrawStage == GameManager.DrawStage.設色 ? true : false;
                            if (this.Target.GetComponent<Step4>()) this.Target.GetComponent<Step4>().enabled = GameManager.script.CurrentDrawStage == GameManager.DrawStage.淡化 ? true : false;
                            if (this.Target.GetComponent<Step4_V2>()) this.Target.GetComponent<Step4_V2>().enabled = GameManager.script.CurrentDrawStage == GameManager.DrawStage.淡化 ? true : false;

                            //在設色階段中　將操作區的改變動畫片段
                            //if (GameManager.script.CurrentDrawStage == GameManager.DrawStage.設色)
                            //{
                            //    SetColorBoneAnimation.script.pictureType =
                            //        this.Target.GetComponent<Step3>().pictureType;
                            //}
                            //break;
                        }

                    }
                }

                this.currentMouseType = MouseType.無狀態;
                break;
        }


    }


    /// <summary>
    /// 改變Target的圖片 , 更換圖片後解鎖 【設色】
    /// </summary>
    public void SetPictureStep3(GameObject ChangeObject)
    {
        isLock = false;

        Target.GetComponent<Step3>().pictureType = GameManager.script.設色潑墨顏色;

        if (Target.gameObject.name == "馬1")
        {
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.馬1顏色1)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬1顏色_GUID[0]);
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.馬1顏色2)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬1顏色_GUID[1]);
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.馬1顏色3)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬1顏色_GUID[2]);
        }
        if (Target.gameObject.name == "馬2")
        {
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.馬2顏色1)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬2顏色_GUID[0]);
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.馬2顏色2)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬2顏色_GUID[1]);
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.馬2顏色3)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬2顏色_GUID[2]);
        }

        if (Target.gameObject.name == "馬3")
        {
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.馬3顏色1)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬3顏色_GUID[0]);
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.馬3顏色2)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬3顏色_GUID[1]);
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.馬3顏色3)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬3顏色_GUID[2]);
        }

        if (Target.gameObject.name == "樹1")
        {
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.樹1顏色1)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.樹1顏色_GUID[0]);
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.樹1顏色2)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.樹1顏色_GUID[1]);
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.樹1顏色3)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.樹1顏色_GUID[2]);
        }


        if (Target.gameObject.name == "樹2")
        {
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.樹2顏色1)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.樹2顏色_GUID[0]);
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.樹2顏色2)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.樹2顏色_GUID[1]);
            if (SetColorBoneAnimation.script.pictureType == SetColorBoneAnimation.PictureType.樹2顏色3)
                this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.樹2顏色_GUID[2]);
        }
        this.Target.GetComponent<PictureInfo>().isUsed = true;

        ClearControlArea();
    }

    /// <summary>
    /// 改變操作區 構圖的圖片變明暗圖 , 並將Target一同改變成明暗圖 【明暗】
    /// </summary>
    public void SetPictureStep2(GameObject ChangeObject)
    {
        isLock = false;
        if (Target.gameObject.name == "馬1")
        {
            this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬1_GUID);
            //GameObject.Find("馬1-明").GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬1_GUID);
            this.Target.GetComponent<PictureInfo>().isUsed = true;
        }

        if (Target.gameObject.name == "馬2")
        {
            this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬2_GUID);
            //GameObject.Find("馬2-明").GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬2_GUID);
            this.Target.GetComponent<PictureInfo>().isUsed = true;
        }

        if (Target.gameObject.name == "馬3")
        {
            this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬3_GUID);
            //GameObject.Find("馬3-明").GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.馬3_GUID);
            this.Target.GetComponent<PictureInfo>().isUsed = true;
        }

        if (Target.gameObject.name == "樹1")
        {
            this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.樹1_GUID);
            //GameObject.Find("樹1-明").GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.樹1_GUID);
            this.Target.GetComponent<PictureInfo>().isUsed = true;
        }

        if (Target.gameObject.name == "樹2")
        {
            this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.樹2_GUID);
            //GameObject.Find("樹2-明").GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.樹2_GUID);
            this.Target.GetComponent<PictureInfo>().isUsed = true;
        }

        ClearControlArea();
    }


    /// <summary>
    /// 改變操作區 構圖的圖片變明暗圖 , 並將Target一同改變成明暗圖 【淡化】
    /// </summary>
    public void SetPictureStep4()
    {
        isLock = false;
        if (Target.gameObject.name == "土坡1(物件)")
        {
            this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.土坡1_GUID);
            //GameObject.Find("土坡1-淡化").GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.土坡1_GUID);
            this.Target.GetComponent<PictureInfo>().isUsed = true;
        }

        if (Target.gameObject.name == "土坡2(物件)")
        {
            this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.土坡2_GUID);
            //GameObject.Find("土坡2-淡化").GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.土坡2_GUID);
            this.Target.GetComponent<PictureInfo>().isUsed = true;
        }

        if (Target.gameObject.name == "土坡3(物件)")
        {
            this.Target.GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.土坡3_GUID);
            //GameObject.Find("土坡3-淡化").GetComponent<SmoothMoves.Sprite>().SetTextureGUID(GameManager.script.土坡3_GUID);
            this.Target.GetComponent<PictureInfo>().isUsed = true;
        }
    }



    /// <summary>
    /// 清空操作區 0904加入
    /// </summary>
    public void ClearControlArea()
    {
        if (Target)
        {
            if (this.Target.GetComponent<Step2>()) this.Target.GetComponent<Step2>().enabled = false;
            if (this.Target.GetComponent<Step3>()) this.Target.GetComponent<Step3>().enabled = false;
            if (this.Target.GetComponent<Step4>()) this.Target.GetComponent<Step4>().enabled = false;
            if (this.Target.GetComponent<Step4_V2>()) this.Target.GetComponent<Step4_V2>().enabled = false;
        }
    }



    //定義滑鼠狀態
    public enum MouseType
    {
        無狀態 = 0, 點擊 = 1, 放開 = 3, 初始播放引導動畫 = 4
    }
}
