using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.RadioScripts
{
    public class RadioController : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<Track> tracks = new List<Track>();

        private LoadTracks loader;
        private int trackNum;

        private void OnEnable()
        {
            PlayerStats.OnPlayerGameOver += BrokeRadio;
            LoadTracks.OnTrackWasLoaded += loadTracks;
        }

        private void OnDisable()
        {
            PlayerStats.OnPlayerGameOver -= BrokeRadio;
            LoadTracks.OnTrackWasLoaded -= loadTracks;
        }

        private void Start()
        {
            trackNum = 0;
            loader = GetComponent<LoadTracks>();
            if (loader == null)
            {
                Debug.LogError("Не найден компонент LoadTracks!");
                return;
            }
            loader.LoadTracksFromFolder();

        }

   
        private void BrokeRadio()
        {
            transform.gameObject.SetActive(false);
        }

        private void loadTracks()
        {
            tracks=loader.GetTracks();
            if (tracks.Count > 0)
            {
                audioSource.resource = tracks[0].clip;
                audioSource.Play();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StopTrack();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                PlayTrack();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextTrack();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PreviusTrack();
            }
        }


        public void StopTrack()
        {
            audioSource.Stop();
        }

        public void PlayTrack()
        {
            if (audioSource.resource != null)
            {
                audioSource.Play();
            }
            else
            {
                if (tracks.Count > 0)
                {
                    trackNum = 0;
                    audioSource.resource = tracks[trackNum].clip;
                    audioSource.Play();
                }
            }
     
        }

        public void NextTrack()
        {
            if(trackNum < tracks.Count-1)
            {
                trackNum++;
            }
            else
            {
                trackNum = 0;
            }
            audioSource.resource= tracks[trackNum].clip;
            audioSource.Play();
        }

        public void PreviusTrack()
        {
            if (trackNum > 0)
            {
                trackNum--;
            }
            else
            {
                trackNum = tracks.Count-1;
            }

            audioSource.resource = tracks[trackNum].clip;
            audioSource.Play();
        }


    }
}
