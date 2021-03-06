﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACT
{
    public interface IActionListener
    {
        void Update(float deltaTime);
        void OnActionChanging(ActData.Action oldAction, ActData.Action newAction);
        void OnInputMove();
        void OnHitData(HitData hitData);
        void OnHurt(int damage);
        void OnBuff(UInt32 target, int id);
        void OnFaceTarget();
    }
}
