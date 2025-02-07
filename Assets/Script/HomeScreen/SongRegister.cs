using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongRegister : MonoBehaviour
{
    private List<Song> _songs = new List<Song>();

    // Start is called before the first frame update
    void Start()
    {
        // Create Song.xml
        //InitSongXml();

        //RegisterSong();

        DebugSongXml(_songs);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    private void InitSongXml()
    {
        /* get parameters from TXT file */
        string[] titleList = Common.GetTXTFileLineList(FileName.SongTitleList);

        /* create song list for XML */
        foreach (string title in titleList)
        {
            _songs.Add(new Song()
            {
                Title = title,
            });

            if (title == "Birthday Song")
            {
                RegisterSong(title, 3, 120, 8); // 3 îèéq, 120 bpm, 8 îèÇÃëOët
            }
        }

        // save to XML file
        Common.ExportToXml(_songs, FileName.XmlSong);
    }

    /// <summary>
    /// Debug 
    /// </summary>
    /// <param name="songs"></param>
    private void DebugSongXml(List<Song> songs)
    {
        foreach (Song song in songs)
        {
            Debug.Log($"title: {song.Title}, beat: {song.Beat}, BPM: {song.BPM}, Clock(beat/bar): {song.Clock}");
        }
    }

    /// <summary>
    /// save songs from TXT to XML
    /// </summary>
    private void RegisterSong(string title, int beat, int bpm, int introBeat)
    {
        foreach (Song song in _songs)
        {
            song.BPM = bpm;
            song.Beat = beat;
            song.Intro = introBeat;
        }

        /* Load from TXT file */
        // format: title, bpm, beat, clock
        // clock: beat / bar
        // beat: x îèéq
        // bpm: beat / minute
    }

}
