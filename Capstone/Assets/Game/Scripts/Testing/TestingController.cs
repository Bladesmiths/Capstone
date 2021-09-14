using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

namespace Bladesmiths.Capstone.Testing {
    public class TestingController : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private string analyticsLink; 

        [SerializeField]
        private List<string> reportedDataNames;
        [SerializeField]
        private List<TypeCode> reportedDataTypes; 

        private Dictionary<string, TestDataValue> reportedData = new Dictionary<string, TestDataValue>();
        #endregion

        public Dictionary<string, TestDataValue> ReportedData
        {
            get { return reportedData; }
        }

        void Start()
        {
            for (int i = 0; i < reportedDataTypes.Count; i++)
            {
                string name = reportedDataNames[i];
                TypeCode type = reportedDataTypes[i];

                switch (type)
                {
                    case TypeCode.Int32:
                        reportedData.Add(name, new TestDataInt(name));
                        break;
                    case TypeCode.Single:
                        reportedData.Add(name, new TestDataFloat(name));
                        break;
                    case TypeCode.Boolean:
                        reportedData.Add(name, new TestDataBool(name));
                        break;
                    case TypeCode.String:
                        reportedData.Add(name, new TestDataString(name));
                        break;
                    case TypeCode.DateTime:
                        reportedData.Add(name, new TestDataTime(name));
                        ((TestDataTime)reportedData[name]).StartTimer(this);
                        break;
                    default:
                        Debug.Log("Undefined Testing Type");
                        break;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Testing Time Tracking
            //Debug.Log(reportedData["timeToTest"].DataString); 
        }

        public void EndTest()
        {
            ReportData(); 
        }

        private void ReportData()
        {
            string completeReportedLink = analyticsLink + "/?";

            foreach (TestDataValue testData in reportedData.Values)
            {
                completeReportedLink += (testData.Report() + "&"); 
            }
            completeReportedLink.Remove(analyticsLink.LastIndexOf("&")); 

            WWWForm form = new WWWForm();
            UnityEngine.Networking.UnityWebRequest www = 
                UnityEngine.Networking.UnityWebRequest.Get(completeReportedLink);

            www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || 
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                Debug.Log("Report Successful");
            }
        }
    }
}
