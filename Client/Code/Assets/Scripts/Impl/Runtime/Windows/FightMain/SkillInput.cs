﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosBaseFramework;
using UnityEngine.EventSystems;
using AosHotfixFramework;
using ActData.Helper;

namespace AosHotfixRunTime
{
    public class SkillInput : ACT.ISkillInput
    {
        const float MAX_IGNORE_TIME = 2f; //最大忽略时间

        public enum ESkillType
        {
            Normal,
            Extra,
        }

        Image mIconImg;
        ImageLoader mIconLoader;
        Image mCDImg;

        bool mIsInit;
        SkillItemLink mSkillItemLink;
        int mLinkIndex;
        SkillItem mCurrSkillItem;
        bool mIsSkillReady;
        float mCD = 0;
        int mActiveAcionCache = -1;
        int mInterruptIndex = -1;
        LocalPlayer mLocalPlayerCache;
        float mIgnoreTime = 0f;
        bool mIsSkillBtnDown = false;
        float mTryReleaseSkillTime = 0f;
        string[] mSkillActions;
        List<int> mSkillActionCaches = new List<int>();
        int mMatchSkillActionIdx = -1;
        ESkillType mSkillType;

        public void InitUI(GameObject skillGo)
        {
            mIconImg = Utility.GameObj.Find<Image>(skillGo, "Image_Icon");
            mCDImg = Utility.GameObj.Find<Image>(skillGo, "Image_CD");
            UGUIEventListener.Get(skillGo).onDown = OnSkillDown;
            UGUIEventListener.Get(skillGo).onUp = OnSkillUp;

            mIconLoader = ReferencePool.Fetch<ImageLoader>();
        }

        public void Init(SkillItemLink skillItemLink, ESkillType skillType)
        {
            mSkillType = skillType;
            mSkillItemLink = skillItemLink;
            mLinkIndex = 0;
            InitSkillByIdx(mLinkIndex);
            mIsSkillBtnDown = false;

            Game.EventMgr.Subscribe(PlayerCtrlEvent.UpdateSkillCD.EventID, OnEventUpdateSkillCD);
        }

        private void SwitchNextSkill()
        {
            if (mSkillItemLink.SkillItems.Count == 0)
            {
                return;
            }

            mLinkIndex += 1;
            mLinkIndex %= mSkillItemLink.SkillItems.Count;
            float tmpShortestCD = float.MaxValue;
            int tmpShortestCDIdx = mLinkIndex;
            var tmpCtrl = Game.ControllerMgr.Get<PlayerController>();

            for (int i = 0, max = mSkillItemLink.SkillItems.Count; i < max; ++i)
            {
                int tmpIndex = (mLinkIndex + i) % mSkillItemLink.SkillItems.Count;
                float tmpCD = tmpCtrl.GetSkillCD(mSkillItemLink.SkillItems[tmpIndex].ID);

                if (tmpCD < tmpShortestCD)
                {
                    tmpShortestCD = tmpCD;
                    tmpShortestCDIdx = tmpIndex;
                }
            }

            mLinkIndex = tmpShortestCDIdx;
            InitSkillByIdx(mLinkIndex);
        }

        private void InitSkillByIdx(int index)
        {
            mCurrSkillItem = index >= mSkillItemLink.SkillItems.Count ? null : mSkillItemLink.SkillItems[index];

            if (null != mCurrSkillItem)
            {
                mIsInit = true;
                mCurrSkillItem.SkillInput = this;
                mCD = Game.ControllerMgr.Get<PlayerController>().GetSkillCD(mCurrSkillItem.ID);

                if (mCD <= 0)
                {
                    mCDImg.gameObject.SetActive(false);
                }
                else
                {
                    mCDImg.gameObject.SetActive(true);
                    mCDImg.fillAmount = Mathf.Max(1f - mCD * 1000f / mCurrSkillItem.SkillAttrBase.CD, 0f);
                }

                mIconLoader.Load(ImageLoader.EIconType.Skill, mCurrSkillItem.SkillBase.Icon, mIconImg, null, false);
                mLocalPlayerCache = Game.ControllerMgr.Get<UnitController>().LocalPlayer;

                mSkillActions = mCurrSkillItem.SkillBase.Action.Split(',');
                mSkillActionCaches.Clear();

                for (int i = 0, max = mSkillActions.Length; i < max; ++i)
                {
                    mSkillActionCaches.Add(mLocalPlayerCache.ActStatus.ActionGroup.GetActionIdx(mSkillActions[i]));
                }

                mIsSkillReady = false;

            }
            else
            {
                Reset();
            }
        }

        public void Update(float deltaTime)
        {
            if (!mIsInit)
            {
                return;
            }

            if (mIgnoreTime < MAX_IGNORE_TIME)
            {
                mIgnoreTime += deltaTime;
            }
            else if (mIgnoreTime >= MAX_IGNORE_TIME)
            {
                if (mLinkIndex != 0 && Game.ControllerMgr.Get<PlayerController>().GetSkillCD(mSkillItemLink.SkillItems[0].ID) <= 0f)
                {
                    mLinkIndex = 0;
                    InitSkillByIdx(mLinkIndex);
                }
            }

            bool tmpCDDone = UpdateCD(deltaTime);
            bool tmpSkillLinked = UpdateSkillLink(deltaTime);

            mIsSkillReady = tmpCDDone && tmpSkillLinked;

            if (mIsSkillBtnDown)
            {
                mTryReleaseSkillTime += deltaTime;

                if (mTryReleaseSkillTime > 0.1f)
                {
                    mTryReleaseSkillTime = 0f;
                    TryReleaseSkill();
                }
            }
        }

        public void Reset()
        {
            mCDImg.fillAmount = 0f;
            Utility.GameObj.SetActive(mIconImg, false);
            mIconImg.sprite = null;

            if (null != mCurrSkillItem)
            {
                mCurrSkillItem.SkillInput = null;
            }

            mCurrSkillItem = null;
            mIsInit = false;

            Game.EventMgr.Unsubscribe(PlayerCtrlEvent.UpdateSkillCD.EventID, OnEventUpdateSkillCD);
        }

        public void Release()
        {
            Reset();

            if (null != mIconLoader)
            {
                ReferencePool.Recycle(mIconLoader);
                mIconLoader = null;
            }
        }

        private bool UpdateCD(float deltaTime)
        {
            bool tmpFlag = false;

            if (mCD <= 0)
            {
                tmpFlag = true;
            }
            else
            {
                mCD -= deltaTime;
                mCDImg.fillAmount = Mathf.Max(1 - mCD * 1000f / mCurrSkillItem.SkillAttrBase.CD, 0f);

                if (mCD <= 0f)
                {

                }
            }

            return tmpFlag;
        }

        private bool UpdateSkillLink(float deltaTime)
        {
            bool tmpFlag = false;
            var tmpActiveAction = mLocalPlayerCache.ActStatus.ActiveAction;

            if (tmpActiveAction.ActionCache != mActiveAcionCache)
            {
                mInterruptIndex = -1;
                mActiveAcionCache = tmpActiveAction.ActionCache;

                for (int i = 0, max = tmpActiveAction.ActionInterrupts.Count; i < max; ++i)
                {
                    var tmpInterrupt = tmpActiveAction.ActionInterrupts[i];

                    for (int j = 0, jmax = mSkillActionCaches.Count; j < jmax; ++j)
                    {
                        if (tmpInterrupt.ActionCache == mSkillActionCaches[j])
                        {
                            mInterruptIndex = i;
                            mMatchSkillActionIdx = j;
                            break;
                        }
                    }

                    if (mInterruptIndex != -1)
                    {
                        break;
                    }
                }
            }

            if (mInterruptIndex >= 0 && mLocalPlayerCache.ActStatus.GetInterruptEnabled(mInterruptIndex))
            {
                tmpFlag = true;
            }

            return tmpFlag;
        }

        private void OnSkillDown(PointerEventData arg)
        {
            if (ESkillType.Normal == mSkillType)
            {
                mIsSkillBtnDown = true;
                mTryReleaseSkillTime = 1f;
            }
            else if (ESkillType.Extra == mSkillType)
            {
                if (mIsSkillReady)
                {
                    mCD = mCurrSkillItem.SkillAttrBase.CD * 0.001f;
                    Game.ControllerMgr.Get<PlayerController>().SetSkillCD(mCurrSkillItem.ID, mCD);
                    TryReleaseExtraSkill(ACT.EOperation.EO_Block, ACT.EInputType.EIT_Click);
                }
            }
        }

        private void OnSkillUp(PointerEventData arg)
        {
            if (ESkillType.Normal == mSkillType)
            {
                mIsSkillBtnDown = false;
            }
            else if (ESkillType.Extra == mSkillType)
            {
                TryReleaseExtraSkill(ACT.EOperation.EO_Block, ACT.EInputType.EIT_Release);
            }
        }

        private void TryReleaseSkill()
        {
            mIgnoreTime = 0f;

            if (!mIsInit)
            {
                return;
            }

            LocalPlayer tmpLocalPlayer = Game.ControllerMgr.Get<UnitController>().LocalPlayer;

            if (!mIsSkillReady)
            {
                var tmpInterruptIdx = tmpLocalPlayer.ActStatus.ActiveAction.GetActionInterruptIdx(ACT.EOperation.EO_Attack);

                if (-1 != tmpInterruptIdx)
                {
                    tmpLocalPlayer.LinkSkill(null, tmpInterruptIdx);
                }

                return;
            }

            tmpLocalPlayer.LinkSkill(this, mInterruptIndex);
        }

        private void TryReleaseExtraSkill(ACT.EOperation operation, ACT.EInputType inputType)
        {
            LocalPlayer tmpLocalPlayer = Game.ControllerMgr.Get<UnitController>().LocalPlayer;
            var tmpInterruptIdx = tmpLocalPlayer.ActStatus.ActiveAction.GetActionInterruptIdx(operation, inputType);

            if (-1 != tmpInterruptIdx)
            {
                tmpLocalPlayer.LinkSkill(null, tmpInterruptIdx);
                tmpLocalPlayer.ActStatus.SkillItem = mCurrSkillItem;
            }
        }

        public void PlaySkill()
        {
            mCD = mCurrSkillItem.SkillAttrBase.CD * 0.001f;
            Game.ControllerMgr.Get<PlayerController>().SetSkillCD(mCurrSkillItem.ID, mCD);
            mLocalPlayerCache.PlaySkill(mCurrSkillItem, mSkillActions[mMatchSkillActionIdx]);
            SwitchNextSkill();
        }

        public void OnHitTarget(ACT.IActUnit target)
        {
        }

        public void OnHit(ACT.IActUnit target)
        {
        }

        public void OnHurt(ACT.IActUnit target)
        {
        }

        private void OnEventUpdateSkillCD(object sender, GameEventArgs arg)
        {
            var tmpEventArg = arg as PlayerCtrlEvent.UpdateSkillCD;

            if (null == tmpEventArg || null == mCurrSkillItem)
            {
                return;
            }

            if (tmpEventArg.SkillID == mCurrSkillItem.ID)
            {
                mCD = tmpEventArg.CD;
            }
        }
    }
}
