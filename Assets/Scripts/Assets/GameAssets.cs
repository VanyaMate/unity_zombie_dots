using System;
using UnityEngine;

namespace Assets
{
    public class GameAssets : MonoBehaviour
    {
        public const int UNITS_LAYER = 7;

        public static GameAssets Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}