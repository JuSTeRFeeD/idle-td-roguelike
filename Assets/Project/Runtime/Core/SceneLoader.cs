using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer.Unity;

namespace Project.Runtime.Core
{
    public class SceneLoader
    {
        private readonly LifetimeScope _currentScope;
        private readonly CanvasGroup _loadingCanvasGroup;
        private const float FadeTime = 0.5f;

        public SceneLoader(LifetimeScope currentScope, CanvasGroup loadingCanvasGroup)
        {
            _loadingCanvasGroup = loadingCanvasGroup;
            _loadingCanvasGroup.alpha = 1;
            _currentScope = currentScope;
        }

        private static bool _fadeInProgress = false;
        private static bool _isLoading = false;

        public IEnumerator LoadSceneAsync(string sceneName)
        {
            return LoadSceneAsync(sceneName, () => true);
        }
        
        private IEnumerator LoadSceneAsync(string sceneName, Func<bool> isActiveScene)
        {
            if (_isLoading)
            {
                yield break;
            }

            _isLoading = true;
            _fadeInProgress = false;

            _loadingCanvasGroup
                .DOFade(1, FadeTime)
                .SetEase(Ease.InOutQuart)
                .SetLink(_loadingCanvasGroup.gameObject);
            yield return new WaitForSeconds(FadeTime);
            _loadingCanvasGroup.blocksRaycasts = true;
            _loadingCanvasGroup.interactable = true;
            
            try
            {
                using (LifetimeScope.EnqueueParent(_currentScope))
                {
                    var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
                    asyncLoad.allowSceneActivation = false;

                    // Wait until the asynchronous scene fully loads
                    while (!asyncLoad.isDone)
                    {
                        if (asyncLoad.progress >= 0.9f)
                        {
                            if (!_fadeInProgress && isActiveScene.Invoke())
                            {
                                _fadeInProgress = true;
                                asyncLoad.completed += AsyncLoad_completed;
                                asyncLoad.allowSceneActivation = isActiveScene.Invoke();
                                if (asyncLoad.allowSceneActivation)
                                {
                                    _isLoading = false;
                                }
                            }
                        }

                        yield return null;
                    }
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void AsyncLoad_completed(AsyncOperation obj)
        {
            obj.completed -= AsyncLoad_completed;
            _loadingCanvasGroup
                .DOFade(0, FadeTime)
                .SetLink(_loadingCanvasGroup.gameObject)
                .SetEase(Ease.InOutQuart)
                .OnComplete(() =>
                {
                    _fadeInProgress = false;
                    _loadingCanvasGroup.blocksRaycasts = false;
                    _loadingCanvasGroup.interactable = false;
                });
        }
    }
}