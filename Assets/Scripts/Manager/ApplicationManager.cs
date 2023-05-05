using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

namespace TeleopReachy
{
    public class ApplicationManager : MonoBehaviour
    {
        public GameObject userTracker = null;

        public GameObject canvasOnlineMenu = null;

        public Transform menuParent = null;

        public GameObject userInput = null;

        public GameObject ground = null;

        public GameObject XROrigin = null;

        void Start()
        {
            SceneManager.LoadScene("ConnectionScene", LoadSceneMode.Additive);
            EventManager.StartListening(EventNames.QuitApplication, QuitApplication);
            EventManager.StartListening(EventNames.QuitMirrorScene, OnConnectToRobot);
            EventManager.StartListening(EventNames.StartMirrorScene, LoadMirrorScene);
            EventManager.StartListening(EventNames.LoadConnectionScene, ReturnToConnectionScene);
            EventManager.StartListening(EventNames.BackToMirrorScene, ReturnToMirrorScene);
            EventManager.StartListening(EventNames.ShowXRay, ShowXRay);
            EventManager.StartListening(EventNames.HideXRay, HideXRay);
        }

        void QuitApplication()
        {
            Debug.Log("Exiting app");
            Application.Quit();
        }


        private void LoadMirrorScene()
        {
            SceneManager.UnloadSceneAsync("ConnectionScene");
            StartCoroutine(LoadTransitionRoom());
        }

        private void ReturnToMirrorScene()
        {
            ground.SetActive(true);
            StartCoroutine(BackToMirrorScene());
        }

        IEnumerator BackToMirrorScene()
        {
            SceneManager.LoadScene("MirrorScene", LoadSceneMode.Additive);
            yield return null;
            ToogleXRRayInteractors(true);
            EventManager.TriggerEvent(EventNames.MirrorSceneLoaded);
        }

        private void OnConnectToRobot()
        {
            SceneManager.UnloadSceneAsync("MirrorScene");
            ground.SetActive(false);
            ToogleXRRayInteractors(false);
        }

        private void ShowXRay()
        {
            ToogleXRRayInteractors(true);
        }

        private void HideXRay()
        {
            ToogleXRRayInteractors(false);
        }

        private void ToogleXRRayInteractors(bool activated)
        {
            XRInteractorLineVisual[] xrlines = XROrigin.GetComponentsInChildren<XRInteractorLineVisual>();
            foreach (XRInteractorLineVisual xr in xrlines)
                xr.enabled = activated;
        }

        IEnumerator LoadTransitionRoom()
        {
            userTracker.SetActive(true);

            SceneManager.LoadScene("TeleoperationScene", LoadSceneMode.Additive);
            yield return null;

            userInput.SetActive(true);
            canvasOnlineMenu.SetActive(true);
            UserEmotionInput uei = userInput.GetComponent<UserEmotionInput>();
            uei.onlineMenuManager = canvasOnlineMenu.GetComponent<OnlineMenuManager>();

            EventManager.TriggerEvent(EventNames.TeleoperationSceneLoaded);
            SceneManager.LoadScene("MirrorScene", LoadSceneMode.Additive);
            yield return null;

            EventManager.TriggerEvent(EventNames.MirrorSceneLoaded);

        }


        IEnumerator LoadTeleoperationScene()
        {
            ground.SetActive(false);
            userInput.SetActive(true);
            canvasOnlineMenu.SetActive(true);
            UserEmotionInput uei = userInput.GetComponent<UserEmotionInput>();
            uei.onlineMenuManager = canvasOnlineMenu.GetComponent<OnlineMenuManager>();

            userTracker.SetActive(true);

            SceneManager.LoadScene("TeleoperationScene", LoadSceneMode.Additive);
            yield return null;

            EventManager.TriggerEvent(EventNames.TeleoperationSceneLoaded);
        }

        private void ReturnToConnectionScene()
        {
            Debug.Log("Loading Connection Scene");
            SceneManager.UnloadSceneAsync("TeleoperationScene");
            SceneManager.UnloadSceneAsync("MirrorScene");
            userInput.SetActive(false);
            userTracker.SetActive(false);
            canvasOnlineMenu.SetActive(false);
            ground.SetActive(true);
            SceneManager.LoadScene("ConnectionScene", LoadSceneMode.Additive);
        }
    }
}
