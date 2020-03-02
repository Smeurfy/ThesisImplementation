using UnityEngine;

public class MusicManager : MonoBehaviour 
{
    public static MusicManager instance;

    [SerializeField] private AudioClip[] musics;

    private int currentIndex = -1;
    private AudioSource musicPlayer;

    private void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        if(musics.Length > 0)
        {
            int newIndex = Random.Range(0, musics.Length);
            currentIndex = newIndex;
            musicPlayer.PlayOneShot(musics[currentIndex]);
            PlaySong();
        }
        else
        {
            Destroy(gameObject);
        }
        MakeThisObjectSingleton();
    }

    public void ChangeVolume(float volume)
    {
        musicPlayer.volume = volume;
    }

    private void Update()
    {
        if (!musicPlayer.isPlaying)
        {
            PickNextSongToPlay();
            PlaySong();
        }
    }

    private void PickNextSongToPlay()
    {
        if(musics.Length > 1)
        {
            int newIndex = Random.Range(0, musics.Length);
            while(newIndex == currentIndex)
            {
                newIndex = Random.Range(0, musics.Length);
            }
            currentIndex = newIndex;
        }
    }

    private void PlaySong()
    {
        musicPlayer.PlayOneShot(musics[currentIndex]);
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
        DontDestroyOnLoad(gameObject);
    }
}
