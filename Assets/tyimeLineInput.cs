using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class tyimeLineInput : MonoBehaviour
{
    private PlayableDirector _director;

    private void Start()
    {
        _director = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _director.Play();  
        }
    }
}
