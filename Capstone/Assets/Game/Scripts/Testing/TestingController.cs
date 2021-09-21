using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

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
        // The position where the player starts
        private Vector3 playerStartPosition;
        // The rotation the player starts at
        private Quaternion playerStartRotation;

        // A dictionary mapping the names of data values to their containers
        [OdinSerialize]
        private Dictionary<string, IReportableData> reportedData = new Dictionary<string, IReportableData>();

        // Testing
        public float playerHealth;
        public string timeString; 
        #endregion

        // Returns the dictionary of data names mapped to data values
        public Dictionary<string, IReportableData> ReportedData
        {
            get { return reportedData; }
        }

        /// <summary>
        /// Initializes all of the reportable data
        /// </summary>
        private void Awake()
        {
            foreach (KeyValuePair<string, IReportableData> reportableDatum in reportedData)
            {
                reportableDatum.Value.Init(reportableDatum.Key); 
            }
        }

        /// <summary>
        /// Sets up the Controller's necessary values
        /// </summary>
        void Start()
        {
            playerStartPosition = player.transform.position;
            playerStartRotation = player.transform.rotation;
        }

        // Keeping for potential testing 
        void Update()
        {
            // Updating testing variables to make sure things are updated correctly
            playerHealth = float.Parse(reportedData["remainingHealth"].DataString);
            timeString = reportedData["timeToTest"].DataString;
        }

        /// <summary>
        /// Starts the timer data
        /// </summary>
        public void StartTimeData()
        {
            ((TestDataTime)reportedData["timeToTest"]).StartTimer(this);
        }

        /// <summary>
        /// End the test by reporting data and resetting the player
        /// </summary>
        public void EndTest()
        {
            ((TestDataTime)reportedData["timeToTest"]).StopTimer();
            ReportData();
            ResetPlayer();
            ResetData();
        }

        /// <summary>
        /// Report the data using the names and values that belong to the controller
        /// </summary>
        private void ReportData()
        {
            // Declare a link using the analytics link field
            string completeReportedLink = analyticsLink + "/?";

            // Assemble the rest of the link using the data values
            foreach (IReportableData testData in reportedData.Values)
            {
                completeReportedLink += (testData.Report() + "&"); 
            }

            // Remove the last character because it is an &
            completeReportedLink.Remove(analyticsLink.Length - 1,1);

            // Open the URL, sending the embedded data and opening the survey
            Application.OpenURL(completeReportedLink);
        }

        /// <summary>
        /// Reset the Player to their starting position
        /// </summary>
        private void ResetPlayer()
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.SetPositionAndRotation(playerStartPosition, playerStartRotation);
            player.GetComponent<CharacterController>().enabled = true;

            Player playerComponent = player.GetComponent<Player>();
            playerComponent.CinemachineCameraTarget.transform.rotation = Quaternion.identity;
            playerComponent._cinemachineTargetYaw = 0;
            playerComponent._cinemachineTargetPitch = 0;

            playerComponent.Health = playerComponent.MaxHealth;

            playerComponent.Grounded = true; 
        }

        /// <summary>
        /// Resets all reportable data to its initial value
        /// </summary>
        private void ResetData()
        {
            foreach (IReportableData reportableDatum in reportedData.Values)
            {
                reportableDatum.ResetData();
            }
        }
    }
}
