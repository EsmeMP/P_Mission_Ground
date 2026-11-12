
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip buttonSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip victorySound;

    public void PlayButton()
    {
        audioSource.PlayOneShot(buttonSound, 0.5f);
    }

    public void PlayCorrect()
    {
        audioSource.PlayOneShot(correctSound, 0.7f);
    }

    public void PlayWrong()
    {
        audioSource.PlayOneShot(wrongSound, 0.5f);
    }

    public void PlayVictory()
    {
        audioSource.PlayOneShot(victorySound, 0.8f);
    }
}
