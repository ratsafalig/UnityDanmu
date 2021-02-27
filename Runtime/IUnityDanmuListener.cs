using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnityDanmuListener
{
    UnityEngine.Object OnPublish(IUnityDanmuEvent unityDanmuEvent);
}
