using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingSounds : MonoBehaviour
{
    public AudioClip keyStroke;
    public AudioClip newLine;

    public AudioSource audioSrc;

    public void PlayTypingSound()
    {
        if(Input.GetKeyDown("return"))
        {
            audioSrc.PlayOneShot(newLine);
        }
        else
        {
            audioSrc.PlayOneShot(keyStroke);
        }
    }
}
