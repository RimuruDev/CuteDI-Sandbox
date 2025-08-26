using UnityEngine;
using System.Collections;
using System.ComponentModel;

namespace AbyssMoth.CuteDI.Example
{
    public interface ICoroutineRunner
    {
        public Coroutine StartCoroutine(string methodName);
        public Coroutine StartCoroutine(IEnumerator routine);
        public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value);
        
        public void StopCoroutine(IEnumerator routine);
        public void StopCoroutine(Coroutine routine);
    }
}