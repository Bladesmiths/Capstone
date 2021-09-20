using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace Bladesmiths.Capstone.Testing
{
    public interface IReportableData
    {
        public string DataString { get; }
        public void Init(string name); 
        public string Report();
    }

    public abstract class TestDataValue<T> : IReportableData
    {
        protected string dataName;
        protected string dataString;
        private ReactiveType<T> data;

        public virtual ReactiveType<T> Data 
        { 
            get { return data; }
        }

        public string DataName
        {
            get { return dataName; }
        }

        public virtual string DataString
        {
            get { return dataString; }
        }

        public void Init(string name)
        {
            dataName = name;

            Debug.Log(Data.GetType());

            Data.OnValueChanged += UpdateData;

            dataString = Data.ToString();
        }

        protected virtual void UpdateData()
        {
            dataString = Data.ToString();
        }

        public virtual string Report()
        {
            return $"{dataName}={dataString}";
        }
    }

    [Serializable]
    public class TestDataInt : TestDataValue<int>
    {
        [SerializeField]
        new private ReactiveInt data;

        public override ReactiveType<int> Data 
        {
            get { return data; }
        }
    }

    [Serializable]
    public class TestDataFloat : TestDataValue<float>
    {
        [SerializeField]
        new private ReactiveFloat data;

        public override ReactiveType<float> Data
        {
            get { return data; }
        }
    }

    [Serializable]
    public class TestDataBool : TestDataValue<bool>
    {
        [SerializeField]
        new private ReactiveBool data;

        public override ReactiveType<bool> Data
        {
            get { return data; }
        }
    }

    [Serializable]
    public class TestDataString : TestDataValue<string>
    {
        [SerializeField]
        new private ReactiveString data;

        public override ReactiveType<string> Data
        {
            get { return data; }
        }
    }

    [Serializable]
    public class TestDataTime : TestDataValue<TimeSpan>
    {
        [SerializeField]
        new private ReactiveTime data;
        
        bool timerCounting;

        public override ReactiveType<TimeSpan> Data
        {
            get { return data; }
        }

        public override string DataString
        {
            get { return data.CurrentValue.ToString(@"h\:mm\:ss"); }
        }

        public void StartTimer(MonoBehaviour monoBehaviourInstance)
        {
            timerCounting = true;
            monoBehaviourInstance.StartCoroutine(Timer()); 
        }

        public override string Report()
        {
            return $"{dataName}={data.CurrentValue.ToString(@"h\:mm\:ss")}";
        }

        IEnumerator Timer()
        {
            while (timerCounting)
            {
                data.CurrentValue += TimeSpan.FromSeconds(Time.deltaTime);
                yield return null;
            }
        }
    }

}