using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad;

        [SerializeField] private Button newGameButton;
        [SerializeField] private Button howToButton;
        [SerializeField] private Button backToMenuButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private CanvasGroup menuCanvasGroup;
        [SerializeField] private CanvasGroup howToCanvasGroup;

        [SerializeField] private Image VolumeOnIcon;
        [SerializeField] private Image VolumeMuteIcon;


        [SerializeField] private float fadeTime = .5f;


        private void Awake()
        {
#if UNITY_WEBGL
            exitButton.gameObject.SetActive(false);
            ToggleAudio(); //set audio as "Suspended"
#endif
        }


        private void Start()
        {
            FadeCanvasGroup(menuCanvasGroup, true, 0);
            FadeCanvasGroup(howToCanvasGroup, false, 0);
            newGameButton.onClick.AddListener(() => { ChangeSceneAndFadeManager.Instance.DoChangeScene(sceneToLoad); });

            howToButton.onClick.AddListener(() =>
            {
                FadeCanvasGroup(menuCanvasGroup, false, fadeTime);
                FadeCanvasGroup(howToCanvasGroup, true, fadeTime);
            });

            backToMenuButton.onClick.AddListener(() =>
            {
                FadeCanvasGroup(menuCanvasGroup, true, fadeTime);
                FadeCanvasGroup(howToCanvasGroup, false, fadeTime);
            });

            exitButton.onClick.AddListener(Application.Quit);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            VolumeMuteIcon.enabled = !_fmodEnabled;
            VolumeOnIcon.enabled = _fmodEnabled;
        }

        bool _fmodEnabled = true;
        bool firstPlayOccurred = false;

        public void ToggleAudio()
        {
        }

        private void FadeCanvasGroup(CanvasGroup cg, bool fadeIn, float time)
        {
            cg.interactable = fadeIn;
            cg.blocksRaycasts = fadeIn;
            cg.DOFade(fadeIn ? 1 : 0, time);
        }
    }
}