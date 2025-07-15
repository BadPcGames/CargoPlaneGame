using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LoadTracks : MonoBehaviour
{
    private string path;
    private bool isLoading = false;
    private List<Track> musicTracks = new List<Track>();

    public string folderName = "Music";
    public delegate void TrackWasLoaded();
    public static event TrackWasLoaded OnTrackWasLoaded;


    private void Awake()
    {
        path = Path.Combine(Application.streamingAssetsPath, folderName);
    }

    public void LoadTracksFromFolder()
    {
        if (isLoading) return;
        StartCoroutine(LoadAllAudioClips(path));
        OnTrackWasLoaded?.Invoke();
    }

    public List<Track> GetTracks()
    {
        return musicTracks;
    }



    private IEnumerator LoadAllAudioClips(string folderPath)
    {
        isLoading = true;
        DirectoryInfo dir = new DirectoryInfo(folderPath);
        if (!dir.Exists)
        {
            yield break;
        }

        FileInfo[] files = dir.GetFiles("*.wav");

        musicTracks.Clear();

        foreach (var file in files)
        {
            string url = "file://" + file.FullName;

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[LoadTracks] Ошибка при загрузке {file.Name}: {www.error}");
                }
                else
                {
                    var clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip == null)
                    {
                        Debug.LogError($"[LoadTracks] Не удалось получить AudioClip из {file.Name}");
                    }
                    else
                    {
                        musicTracks.Add(new Track { name = file.Name, clip = clip });
                    }
                }
            }

            yield return null; 
        }

        isLoading = false;
    }
}

[System.Serializable]
public class Track
{
    public string name;
    public AudioClip clip;
}