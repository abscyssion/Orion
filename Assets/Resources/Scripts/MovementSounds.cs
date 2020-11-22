using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using TMPro;

public class MovementSounds : MonoBehaviour
{
    public AudioSource audioSrc;
    public AudioClip[] audioClips;
    public Animator animator;

    public float deadZone = 0.3f;
    void Update()
    {
        bool horBool = Input.GetKey("w") || Input.GetKey("s");
        bool verBool = Input.GetKey("a") || Input.GetKey("d");

        if (horBool || verBool && PlayerMove.isOnGround)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }

    static int previousRandom = -1;
    public void PlaySound()
    {
        int random = 0;
        do
        {
            random = Random.Range(0, audioClips.Length - 1);
        }
        while (random == previousRandom);

        previousRandom = random;
        audioSrc.PlayOneShot(audioClips[random]);
    }
}