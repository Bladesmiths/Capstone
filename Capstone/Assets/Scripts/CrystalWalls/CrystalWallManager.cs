using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Bladesmiths.Capstone
{
    public class CrystalWallManager : MonoBehaviour
    {
        public static CrystalWallManager instance;
        public List<CrystalWallGenerator> crystalWallsGroup1;
        public List<CrystalWallGenerator> crystalWallsGroup2;
        public List<CrystalWallGenerator> crystalWallsGroup3;
        public List<CrystalWallGenerator> crystalWallsGroup4;

        private List<List<CrystalWallGenerator>> allCrystalWalls;


        public void Awake()
        {
            allCrystalWalls = new List<List<CrystalWallGenerator>>();
            if (instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {
            allCrystalWalls.Add(crystalWallsGroup1);
            allCrystalWalls.Add(crystalWallsGroup2);
            allCrystalWalls.Add(crystalWallsGroup3);
            allCrystalWalls.Add(crystalWallsGroup4);

        }

        void Update()
        {

        }

        /// <summary>
        /// Swaps the walls of groups
        /// </summary>
        public void SwitchWalls(int groupNum)
        {
            foreach(CrystalWallGenerator wall in allCrystalWalls[groupNum])
            {
                wall.DissolveWall();
            }
        }
    }
}
