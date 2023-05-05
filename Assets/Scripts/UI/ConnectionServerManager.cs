using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public class ConnectionServerManager : MonoBehaviour
    {
        public string server_data_port { get; private set; }
        public string server_camera_port { get; private set; }
        public string server_mobile_port { get; private set; }
        public const string default_server_data_port = "50055";
        public const string default_server_camera_port = "50057";
        public const string default_server_mobile_port = "50061";

        void Start()
        {
            server_data_port = (PlayerPrefs.GetString("server_data_port") != "" ? PlayerPrefs.GetString("server_data_port") : default_server_data_port);
            server_camera_port = (PlayerPrefs.GetString("server_camera_port") != "" ? PlayerPrefs.GetString("server_camera_port") : default_server_camera_port);
            server_mobile_port = (PlayerPrefs.GetString("server_mobile_port") != "" ? PlayerPrefs.GetString("server_mobile_port") : default_server_mobile_port);
            UpdateServerInfoDisplay();
        }

        public void ResetDefaultServerInfo()
        {
            server_data_port = default_server_data_port;
            server_camera_port = default_server_camera_port;
            server_mobile_port = default_server_mobile_port;
            transform.GetChild(0).GetChild(6).GetChild(1).GetComponent<Text>().text = server_data_port;
            transform.GetChild(0).GetChild(7).GetChild(1).GetComponent<Text>().text = server_camera_port;
            transform.GetChild(0).GetChild(8).GetChild(1).GetComponent<Text>().text = server_mobile_port;
            UpdateServerInfoDisplay();
        }

        void UpdateServerInfoDisplay()
        {
            transform.GetChild(0).GetChild(6).GetComponent<InputField>().text = server_data_port;
            transform.GetChild(0).GetChild(7).GetComponent<InputField>().text = server_camera_port;
            transform.GetChild(0).GetChild(8).GetComponent<InputField>().text = server_mobile_port;
        }

        public void AskToModifyServerInfo()
        {
            RequestServerInfoModification(true);
            UpdateServerInfoDisplay();
        }

        void RequestServerInfoModification(bool isModificationrequested)
        {
            transform.GetChild(0).GetChild(6).GetComponent<InputField>().interactable = isModificationrequested;
            transform.GetChild(0).GetChild(7).GetComponent<InputField>().interactable = isModificationrequested;
            transform.GetChild(0).GetChild(8).GetComponent<InputField>().interactable = isModificationrequested;

            transform.GetChild(0).GetChild(9).gameObject.SetActive(!isModificationrequested);
            transform.GetChild(0).GetChild(10).gameObject.SetActive(!isModificationrequested);
            transform.GetChild(0).GetChild(11).gameObject.SetActive(isModificationrequested);
            transform.GetChild(0).GetChild(12).gameObject.SetActive(isModificationrequested);
            transform.GetChild(0).GetChild(13).gameObject.SetActive(!isModificationrequested);

            if (isModificationrequested)
            {
                transform.GetChild(0).GetChild(6).GetChild(2).GetComponent<Text>().color = ColorsManager.black_transparent;
                transform.GetChild(0).GetChild(7).GetChild(2).GetComponent<Text>().color = ColorsManager.black_transparent;
                transform.GetChild(0).GetChild(8).GetChild(2).GetComponent<Text>().color = ColorsManager.black_transparent;
            }
            else
            {
                transform.GetChild(0).GetChild(6).GetChild(2).GetComponent<Text>().color = ColorsManager.white_transparent;
                transform.GetChild(0).GetChild(7).GetChild(2).GetComponent<Text>().color = ColorsManager.white_transparent;
                transform.GetChild(0).GetChild(8).GetChild(2).GetComponent<Text>().color = ColorsManager.white_transparent;
            }
        }

        public void ModifyServerInfo()
        {
            server_data_port = transform.GetChild(0).GetChild(6).GetComponent<InputField>().text;
            server_camera_port = transform.GetChild(0).GetChild(7).GetComponent<InputField>().text;
            server_mobile_port = transform.GetChild(0).GetChild(8).GetComponent<InputField>().text;

            RequestServerInfoModification(false);
            UpdateServerInfoDisplay();
        }

        public void CancelModifyServerInfo()
        {
            RequestServerInfoModification(false);
            UpdateServerInfoDisplay();
        }
    }
}