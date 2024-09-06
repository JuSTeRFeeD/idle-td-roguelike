using UnityEngine;

namespace Project.Runtime.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        private void Start()
        {
            Application.targetFrameRate = 60;
        }
    }
}