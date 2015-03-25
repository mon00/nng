using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;

namespace Lab70_Framework.Error
{
    public class ErrorController : MonoBehaviour
    {
        public List<ErrorData> ErrorsList { get; private set; }
        public Dictionary<int, string> SubjectsErrorSercher { set; private get; }

        private string[] separators = { "//" };
        private string savePath;

        private GameObject ErrorWindowPrefab;

        private GameObject ErrorCanvas;
        private GameObject ErrorWindow;

        private int ErrorSerialNumber = 0;

        public ErrorController(string dataPath, GameObject errorWindowPrefab)
        {
            savePath = dataPath;
            ErrorWindowPrefab = errorWindowPrefab;
            ErrorsList = new List<ErrorData>();
            SubjectsErrorSercher = new Dictionary<int, string>();

            ErrorCanvas = new GameObject();
            ErrorCanvas.name = "ErrorCanvas";
            ErrorCanvas.AddComponent<Canvas>();
            ErrorCanvas.AddComponent<GraphicRaycaster>();
            ErrorCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;


            ErrorWindow = (GameObject)Instantiate(ErrorWindowPrefab, new Vector3(Screen.width / 2 - 50, Screen.height / 2 - 50, 0), Quaternion.identity);
            ErrorWindow.name = "ErrorWindow";
            ErrorWindow.transform.SetParent(ErrorCanvas.transform);
            ErrorWindow.GetComponentInChildren<Text>().text = "Null";
            ErrorWindow.GetComponentInChildren<Button>().onClick.AddListener(() => FrameworkController.Instance.Error.CloseErrorWindow());

            ErrorCanvas.SetActive(false);

            FrameworkController.Instance.OnAppQuit += new FrameworkController.VoidEvent(SaveErrorLog);
        }

        public void GenerateError(string Subject, string Message)
        {
            ErrorSerialNumber++;
            ErrorCanvas.SetActive(true);
            ErrorCanvas.GetComponent<Canvas>().sortingOrder = 0;
            ErrorWindow.GetComponentInChildren<Text>().text = "Code: " + ErrorSerialNumber + "\r\n" + "Object: " + Subject + "\r\n" + Message;
            ErrorData Error = new ErrorData(ErrorSerialNumber, Subject, Message);
            ErrorsList.Add(Error);
            SubjectsErrorSercher.Add(ErrorSerialNumber, Subject);
        }

        public void CloseErrorWindow()
        {
            ErrorCanvas.SetActive(false);
        }

        public virtual void SaveErrorLog()
        {
            if (ErrorsList.Count == 0) return;
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            string file = DateTime.Now.ToString().Replace("\\","_") + ".txt";
            
            StreamWriter sw = new StreamWriter(savePath + file, false,System.Text.Encoding.UTF8);
            foreach (ErrorData e in ErrorsList)
            {
                string info = "Number: " + e.SerialNumber + "\r\n" + "Sbject: " + e.Subject + "\r\n" + "Mesage: " + e.Message + "\r\n" + "Time: " + e.Time + "\r\n";
                if (e.AdditionalComponents != null)
                {
                    info += "Additional:" + "\r\n";
                    for (int i = 0; i < e.AdditionalComponents.Length; i++)
                    {
                        info += e.AdditionalComponents[i] + "\r\n";
                    }
                }
                info += "\r\n";
                sw.Write(info);
            }
            sw.Close();
        }
    }
}