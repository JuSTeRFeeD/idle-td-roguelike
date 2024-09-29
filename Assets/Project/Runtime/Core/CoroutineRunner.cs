using System.Collections;
using UnityEngine;

namespace Project.Runtime.Core
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator enumerator);
        void StopCoroutine(Coroutine coroutine);
    } 
    
    public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
    {
    }
}