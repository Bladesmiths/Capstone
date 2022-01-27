using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace Bladesmiths.Capstone.Testing
{
    /// <summary>
    /// A piece of reportable data
    /// Used mostly for polymorphism in TestingController
    /// </summary>
    public interface IReportableData
    {
        // A string representation of data
        public string DataString { get; }

        // Initializes the object
        public void Init(string name); 

        // Reports the objects data in a way that can be read by 
        public string Report();

        // Resets the data to its initial state
        public void ResetData(); 
    }

    /// <summary>
    /// Abstract class for all Testing Data to inherit from
    /// </summary>
    /// <typeparam name="T">The type of data that this contains</typeparam>
    public abstract class TestDataValue<T> : IReportableData
    {
        // The name of the data value
        protected string dataName;

        #region Properties
        // Returns the Reactive Data that backs this object
        public abstract ReactiveType<T> Data { get; }

        // Returns name of data value
        public string DataName
        {
            get { return dataName; }
        }

        // Returns data in a string format
        public virtual string DataString
        {
            get { return Data.ToString(); }
        }
        #endregion

        /// <summary>
        /// Initializes the data's name
        /// </summary>
        /// <param name="name">The name of the data value</param>
        public void Init(string name)
        {
            dataName = name;
        }

        /// <summary>
        /// Reports the data and its name in a string format
        /// </summary>
        /// <returns>Returns a string in the form of dataName=DataString</returns>
        public virtual string Report()
        {
            return $"{dataName}={DataString}";
        }

        // Resets the data to its initial value
        public void ResetData()
        {
            Data.Reset(); 
        }
    }

    /// <summary>
    /// Test Data with a backing integer
    /// </summary>
    [Serializable]
    public class TestDataInt : TestDataValue<int>
    {
        // Hiding parent's data field to get access to a more specific type
        [SerializeField]
        private ReactiveInt data;

        // Overriding parent's data property using polymorphism
        public override ReactiveType<int> Data 
        {
            get { return data; }
        }
    }

    /// <summary>
    /// Test Data with a backing float
    /// </summary>
    [Serializable]
    public class TestDataFloat : TestDataValue<float>
    {
        // Hiding parent's data field to get access to a more specific type
        [SerializeField]
        private ReactiveFloat data;

        // Overriding parent's data property using polymorphism
        public override ReactiveType<float> Data
        {
            get { return data; }
        }
    }

    /// <summary>
    /// Test Data with a backing boolean
    /// </summary>
    [Serializable]
    public class TestDataBool : TestDataValue<bool>
    {
        // Hiding parent's data field to get access to a more specific type
        [SerializeField]
        private ReactiveBool data;

        // Overriding parent's data property using polymorphism
        public override ReactiveType<bool> Data
        {
            get { return data; }
        }
    }

    /// <summary>
    /// Test Data with a backing string
    /// </summary>
    [Serializable]
    public class TestDataString : TestDataValue<string>
    {
        // Hiding parent's data field to get access to a more specific type
        [SerializeField]
        private ReactiveString data;

        // Overriding parent's data property using polymorphism
        public override ReactiveType<string> Data
        {
            get { return data; }
        }
    }

    /// <summary>
    /// Test Data backed by time
    /// </summary>
    [Serializable]
    public class TestDataTime : TestDataValue<float>
    {
        // Hiding parent's data field to get access to a more specific type
        [SerializeField]
        private ReactiveFloat data;
        
        // Is the timer counting or not
        private bool timerCounting;

        // Overriding parent's data property using polymorphism
        public override ReactiveType<float> Data
        {
            get { return data; }
        }

        // Overriding parent's DataString property to specify formatting
        public override string DataString
        {
            get { return TimeSpan.FromSeconds(data.CurrentValue).ToString(@"h\:mm\:ss"); }
        }

        /// <summary>
        /// Starts the TimeData's timer
        /// </summary>
        /// <param name="monoBehaviourInstance"></param>
        public void StartTimer(MonoBehaviour monoBehaviourInstance)
        {
            timerCounting = true;
            monoBehaviourInstance.StartCoroutine(Timer()); 
        }

        /// <summary>
        /// Stops the TimeData's timer
        /// </summary>
        public void StopTimer()
        {
            timerCounting = false;
        }

        /// <summary>
        /// Counts using Time.DeltaTime as long as the timer should be counting
        /// </summary>
        /// <returns>Coroutine Variable</returns>
        IEnumerator Timer()
        {
            while (timerCounting)
            {
                data.CurrentValue += Time.deltaTime;
                yield return null;
            }
        }
    }

}