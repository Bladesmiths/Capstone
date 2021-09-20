using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public abstract class TestDataValue
    {
        protected string dataName;
        protected string dataString;

        public virtual string DataString
        {
            get { return dataString; }
        }

        public TestDataValue(string name)
        {
            dataName = name; 
        }

        public virtual string Report()
        {
            return $"{dataName}={dataString}";
        }
    }

    public class TestDataInt : TestDataValue
    {
        int data; 

        public int Data
        {
            get { return data; }
            set
            {
                data = value;
                dataString = data.ToString(); 
            }
        }
        public TestDataInt(string name) : base(name)
        {
            data = default;
        }
    }

    public class TestDataFloat : TestDataValue
    {
        float data;

        public float Data
        {
            get { return data; }
            set
            {
                data = value;
                dataString = data.ToString();
            }
        }
        public TestDataFloat(string name) : base(name)
        {
            data = default;
        }
    }

    public class TestDataBool : TestDataValue
    {
        bool data;

        public bool Data
        {
            get { return data; }
            set
            {
                data = value;
                dataString = data.ToString();
            }
        }
        public TestDataBool(string name) : base(name)
        {
            data = default;
        }
    }

    public class TestDataString : TestDataValue
    {
        string data;

        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                dataString = data;
            }
        }
        public TestDataString(string name) : base(name)
        {
            data = default;
        }
    }

    public class TestDataTime : TestDataValue
    {
        float data;
        bool timerCounting; 

        public TimeSpan Data
        {
            get { return TimeSpan.FromSeconds(data); }
            set
            {
                data = (float)value.TotalSeconds;
                dataString = TimeSpan.FromSeconds(data).ToString(@"h\:mm\:ss");
            }
        }

        public override string DataString
        {
            get { return TimeSpan.FromSeconds(data).ToString(@"h\:mm\:ss"); }
        }

        public TestDataTime(string name) : base(name)
        {
            data = default;

        }

        public void StartTimer(MonoBehaviour monoBehaviourInstance)
        {
            timerCounting = true;
            monoBehaviourInstance.StartCoroutine(Timer()); 
        }

        public override string Report()
        {
            return $"{dataName}={TimeSpan.FromSeconds(data).ToString(@"h\:mm\:ss")}";
        }

        IEnumerator Timer()
        {
            while (timerCounting)
            {
                data += Time.deltaTime;
                yield return null;
            }
        }
    }

}