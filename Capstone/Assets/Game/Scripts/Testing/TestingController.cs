using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

namespace Bladesmiths.Capstone.Testing {
    public class TestingController : SerializedMonoBehaviour
    {
        #region Fields
        [SerializeField][Required]
        private string analyticsLink;

        [SerializeField]
        private GameObject player; 
        private Transform playerStartTransform; 

        [SerializeField]
        private List<string> reportedDataNames;
        [SerializeField]
        private List<TypeCode> reportedDataTypes; 

        [OdinSerialize]
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

            playerStartTransform = player.transform; 
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
            player.transform.position = playerStartTransform.position;
            player.transform.rotation = playerStartTransform.rotation;
        }

        private void ReportData()
        {
            string completeReportedLink = analyticsLink + "/?";

            foreach (TestDataValue testData in reportedData.Values)
            {
                completeReportedLink += (testData.Report() + "&"); 
            }
            completeReportedLink.Remove(analyticsLink.Length - 1,1);

            Application.OpenURL(completeReportedLink);

            //UnityWebRequest www = UnityWebRequest.Get(completeReportedLink);

            //www.SendWebRequest();

            //if (www.result == UnityWebRequest.Result.ConnectionError || 
            //    www.result == UnityWebRequest.Result.ProtocolError)
            //{
            //    Debug.Log(www.error);
            //}
            //else
            //{
            //    Debug.Log(www.downloadHandler.text);
            //    Debug.Log("Report Successful");
            //}
        }
    }
}
