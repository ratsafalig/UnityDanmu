using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DouyuUnityDanmuManager : MonoBehaviour, IUnityDanmuManager
{
    private UnityWebRequest www;
    public string url;

    List<Hashtable> hashtables;
    List<IUnityDanmuListener> m_ListIUnityDanmuListener;

    private class DouyuUnityDanmuEvent : IUnityDanmuEvent
    {
        private Hashtable hashtable;

        public DouyuUnityDanmuEvent(Hashtable hashtable)
        {
            this.hashtable = hashtable;
        }

        public string Get(string key)
        {
            if (hashtable.ContainsKey(key))
            {
                return (string)hashtable[key];
            }
            else
            {
                return null;
            }
        }
    }

    private void Awake()
    {
        hashtables = new List<Hashtable>();
        m_ListIUnityDanmuListener = new List<IUnityDanmuListener>();
    }

    void Start()
    {
        StartCoroutine(Send());
    }

    void Update()
    {
        int i = 0;
    }

    public IEnumerator Send()
    {
        while (true)
        {
            www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            yield return Parse();
            for (int i = 0; i < hashtables.Count; i++)
            {
                IUnityDanmuEvent unityDanmuEvent = new DouyuUnityDanmuEvent(hashtables[i]);
                Publish(unityDanmuEvent);
            }
            hashtables.Clear();
        }
    }

    public IEnumerator Parse()
    {
        string text = www.downloadHandler.text;
        bool flag = false; // 指示标志,当前字符是否在 ' ' 之内
        string key = ""; // 键
        string val = ""; // 值
        Hashtable hashtable = null; // 一条消息的 Hashtable 
        for (int i = 0; i < text.Length; i++)
        {
            // 如果字符是 { 且不在两个 ' ' 之间,则是一条新消息
            if (text[i] == '{' && !flag)
            {
                hashtable = new Hashtable();
            }
            else if (text[i] == '}' && !flag)
            {
                hashtables.Add(hashtable);
            }
            else if ((text[i] == ',' && !flag) || (text[i] == ' ' && !flag) || (text[i] == ':' && !flag))
            {
                continue;
            }
            else if (text[i] == '\'' && !flag)
            {
                flag = true;
            }
            else if (text[i] == '\'' && flag)
            {
                flag = false;
                if (key.Length == 0)
                {
                    key = val;
                    val = "";
                }
                else
                {
                    hashtable.Add(key, val);
                    key = "";
                    val = "";
                }
            }
            else
            {
                val += text[i];
            }
        }
        yield return null;
    }

    public void Register(IUnityDanmuListener m_IUnityDanmuListener)
    {
        m_ListIUnityDanmuListener.Add(m_IUnityDanmuListener);
    }

    public void Publish(IUnityDanmuEvent @event)
    {
        for (int i = 0; i < m_ListIUnityDanmuListener.Count; i++)
        {
            m_ListIUnityDanmuListener[i].OnPublish(@event);
        }
    }
}
