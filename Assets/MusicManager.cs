using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField] private AudioClip[] musics;

    public int currentIndex = 1;
    private AudioSource musicPlayer;

    private void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        if (musics.Length > 0)
        {
            musicPlayer.PlayOneShot(musics[currentIndex]);
            musicPlayer.clip = musics[currentIndex];
            PlaySong();
        }
        else
        {
            Destroy(gameObject);
        }
        MakeThisObjectSingleton();
    }

    public void ChangeVolume(Slider slider)
    {
        musicPlayer.volume = slider.value;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            musicPlayer.Stop();
            PickNextSongToPlay();
            PlaySong();
        }
        if (!musicPlayer.isPlaying)
        {
            PlaySong();
        }
    }

    private void PickNextSongToPlay()
    {
        if (currentIndex == (musics.Length - 1))
            currentIndex = 0;
        else
            currentIndex++;
    }

    private void PlaySong()
    {
        musicPlayer.PlayOneShot(musics[currentIndex]);
        musicPlayer.clip = musics[currentIndex];
    }

    private void MakeThisObjectSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
