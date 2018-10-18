using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


public class PlayVideo : MonoBehaviour
{
	private RawImage image;

	public VideoClip videoToPlay;
	
	public VideoPlayer videoPlayer;
	private VideoSource videoSource;

	private AudioSource audioSource;
	
	// Use this for initialization
	void Start ()
	{
		image = GetComponent<RawImage>();
		videoPlayer = GetComponent<VideoPlayer>();
		StartCoroutine(playVideo());
	}
	
	IEnumerator playVideo()
	{
		//Add AudioSource
		audioSource = gameObject.AddComponent<AudioSource>();

		//Disable Play on Awake for both Video and Audio
		videoPlayer.playOnAwake = false;
		audioSource.playOnAwake = false;
		audioSource.Pause();
        
		videoPlayer.source = VideoSource.VideoClip;

		//videoPlayer.source = VideoSource.Url;
		//videoPlayer.url = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";

		//Set Audio Output to AudioSource
		videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

		//Assign the Audio from Video to AudioSource to be played
		videoPlayer.EnableAudioTrack(0, true);
		videoPlayer.SetTargetAudioSource(0, audioSource);

		//Set video To Play then prepare Audio to prevent Buffering
		videoPlayer.clip = videoToPlay;
		videoPlayer.Prepare();

		while (!videoPlayer.isPrepared)
		{
			yield return null;
		}

		image.texture = videoPlayer.texture;

		videoPlayer.Play();
		audioSource.Play();

		while (videoPlayer.isPlaying)
		{
			yield return null;
		}

		if (!videoPlayer.isPlaying)
		{
			SceneManager.LoadScene("SampleScene");
		}
	}
}
