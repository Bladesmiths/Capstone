using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone.Testing
{
    public class TestDataValue
    {
        #region Fields
        private TestType dataType;

        private string data; 
        #endregion

        public TestDataValue(TestType type)
        {
            dataType = type;    
        }

        public void Update()
        {
            switch (dataType)
            {
                case TestType.Integer:
                    break;
                case TestType.Float:
                    break;
                case TestType.Bool:
                    break;
                case TestType.String:
                    break;
                case TestType.Time:
                    break;
                default:
                    Debug.Log("Undefined Testing Type");
                    break;
            }
        }
    }

    public enum TestType
    {
        Integer, 
        Float, 
        Bool, 
        String, 
        Time
    }
}