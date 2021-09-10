using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing {
    public class TestingController : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private List<string> reportedDataNames;
        [SerializeField]
        private List<TestType> reportedDataTypes; 

        private Dictionary<string, TestDataValue> reportedData;
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < reportedDataNames.Count; i++)
            {
                string name = reportedDataNames[i];
                TestType type = reportedDataTypes[i]; 
                reportedData.Add(name, new TestDataValue(type)); 
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void EndTest()
        {

        }

        private void ReportData(string dataName)
        {

        }

        //    WWWForm form = new WWWForm();
        //    UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get("https://rit.az1.qualtrics.com/jfe/form/SV_afPgmwJySIesQxU/?action1=blah&action2=blah2");

        //    www.SendWebRequest();

        //if (www.isNetworkError || www.isHttpError)
        //{
        //            Debug.Log(www.error);
        //}
        //else
        //{
        //    Debug.Log(www.downloadHandler.text);
        //    Debug.Log("successful!");
    }
}
