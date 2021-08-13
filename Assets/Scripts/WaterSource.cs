using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class WaterSource
    {
        public GameObject go;
        private bool _isEmpty;
        public bool IsEmpty
        {
            get { return _isEmpty; } set { _isEmpty = value; }
        }

        public WaterSource(GameObject go, bool isEmpty)
        {
            this.go = go;
            IsEmpty = isEmpty;
        }
    }
}