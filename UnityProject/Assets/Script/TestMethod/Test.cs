using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Test : MonoBehaviour {

    [SerializeField]
    public string Timy;

    [SerializeField]
    public Test2 _tom = new Test2();
    public Test2 Tom { get { return _tom; } }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[Serializable]
public class Test2
{
    [SerializeField]
    public string Tom;
}
