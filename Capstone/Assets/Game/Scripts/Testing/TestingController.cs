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
        // The link to the survey that may contain embedded data
        [SerializeField][Required]
        private string analyticsLink;

        // The player object
        [SerializeField]
        private GameObject player;
        // The transform where the player starts
        private Transform playerStartTransform; 

        // The names of embedded data that should be reported
        [SerializeField]
        private List<string> reportedDataNames;
        // The types of data that should be reported
        [SerializeField]
        private List<TypeCode> reportedDataTypes; 

        // A dictionary mapping the names of data values to their containers
        [OdinSerialize]
        private Dictionary<string, TestDataValue> reportedData = new Dictionary<string, TestDataValue>();
        #endregion

        // Returns the dictionary of data names mapped to data values
        public Dictionary<string, TestDataValue> ReportedData
        {
            get { return reportedData; }
        }

        /// <summary>
        /// Sets up the Controller's necessary values
        /// </summary>
        void Start()
        {
            // TO-DO: Check if Odin makes this unecessary
            // Assembles the dictionary
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

        // Keeping for potential testing 
        void Update()
        {
            
        }

        /// <summary>
        /// End the test by reporting data and resetting the player
        /// </summary>
        public void EndTest()
        {
            ReportData();
            ResetPlayer();
        }

        /// <summary>
        /// Report the data using the names and values that belong to the controller
        /// </summary>
        private void ReportData()
        {
            // Declare a link using the analytics link field
            string completeReportedLink = analyticsLink + "/?";

            // Assemble the rest of the link using the data values
            foreach (TestDataValue testData in reportedData.Values)
            {
                completeReportedLink += (testData.Report() + "&"); 
            }

            // Remove the last character because it is an &
            completeReportedLink.Remove(analyticsLink.Length - 1,1);

            // Open the URL, sending the embedded data and opening the survey
            Application.OpenURL(completeReportedLink);
        }

        private void ResetPlayer()
        {
            player.transform.position = playerStartTransform.position;
            player.transform.rotation = playerStartTransform.rotation;
        }
    }
}
