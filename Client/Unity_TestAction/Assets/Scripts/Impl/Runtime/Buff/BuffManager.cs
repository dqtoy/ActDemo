﻿using System;
using System.Collections.Generic;

namespace AosHotfixRunTime
{
    public class BuffManager
    {
        Unit mOwner;
        int[] mBuffMultipy;
        int[] mBuffAdded;
        bool[] mBuffDirty;
        bool[] mBuffSpecialEffect;
        List<Buff> mBuffList = new List<Buff>();

        public Unit Owner { get { return mOwner; } }
        public bool CanHurt { get { return !mBuffSpecialEffect[(int)EBuffSpecialEffect.CanNotHurt]; } }
        public bool CanMove { get { return mBuffSpecialEffect[(int)EBuffSpecialEffect.CanMove]; } }
        public bool CanRotate { get { return mBuffSpecialEffect[(int)EBuffSpecialEffect.CanRotate]; } }

        public BuffManager(Unit owner)
        {
            mOwner = owner;
            mBuffMultipy = new int[(int)EPA.MAX];
            mBuffAdded = new int[(int)EPA.MAX];
            mBuffDirty = new bool[(int)EPA.MAX];
            mBuffSpecialEffect = new bool[(int)EBuffSpecialEffect.Max];
        }

        public void OnDestory()
        {
            foreach (Buff buff in mBuffList)
                buff.OnDestory(true);
            mBuffList.Clear();
        }

        public void AddBuff(int id)
        {
            BuffBase buffBase = BuffBaseManager.Instance.GetItem(id);
            if (buffBase == null)
            {
                UnityEngine.Debug.LogError("Buff not found in [BuffBase] table: " + id);
                return;
            }

            // remove the previous buff.
            for (int i = 0; i < mBuffList.Count; i++)
            {
                Buff buff = mBuffList[i];
                if (buff.ID == buffBase.ID)
                {
                    buff.OnDestory(true);
                    mBuffList.RemoveAt(i);
                    break;
                }
            }

            // create a new buff and attach to it.
            Buff newBuff = new Buff(this, buffBase);
            if (newBuff.OnStart())
                mBuffList.Add(newBuff);
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < mBuffList.Count; i++)
            {
                Buff buff = mBuffList[i];
                if (!buff.Update(deltaTime))
                {
                    buff.OnDestory(false);
                    mBuffList.RemoveAt(i--);
                }
            }
        }

        public void Apply(EPA attrib, int multipy, int add)
        {
            switch (attrib)
            {
                case EPA.CurHP:
                    mOwner.AddHp(mOwner.GetAttr(EPA.MaxHP) * multipy / 10000 + add);
                    break;
                default:
                    mBuffMultipy[(int)attrib] += multipy;
                    mBuffAdded[(int)attrib] += add;
                    mBuffDirty[(int)attrib] = true;
                    break;
            }
        }

        public int Apply(EPA attrib, int value)
        {
            int idx = (int)attrib;
            if (!mBuffDirty[idx])
                return value;
            return value + value * mBuffMultipy[(int)attrib] / 10000 + mBuffAdded[(int)attrib];
        }

        public void Apply(EBuffSpecialEffect effect, string param, bool end)
        {
            if (end)
                mBuffSpecialEffect[(int)effect] = false;
            else
                mBuffSpecialEffect[(int)effect] = true;
        }
    }
}