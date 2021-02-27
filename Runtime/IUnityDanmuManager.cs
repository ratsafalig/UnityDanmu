using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnityDanmuManager
{
    IEnumerator Send();
    IEnumerator Parse();
    void Register(IUnityDanmuListener m_IUnityDanmuListener);
    void Publish(IUnityDanmuEvent @event);
}
