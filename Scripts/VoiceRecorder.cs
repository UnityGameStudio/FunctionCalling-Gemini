using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

using UnityEngine.Networking;
using UnityEditor;
// using UnityEngine.JSONSerializeModule;


using System.Threading.Tasks;


public class VoiceRecorder : MonoBehaviour
{

    public GeminiUnityManager geminiUnityManager;

    private string audioName;
    private string fileName;
    private AudioClip audioClip;
    private AudioClip clip;
    private byte[] bytes;

    private int buttonPressedOnce = 0;

    public ObjectSpawner objectSpawner;


    void Start()
    {
        buttonPressedOnce = 0;
    }

    void Update()
    {
        HandleInput();

    }

    public void HandleInput()
    {
        // This commented part is for working with Meta Quest
        // if((OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Two)) && buttonPressedOnce == 0 )
        if (Input.GetKeyDown(KeyCode.Space) && buttonPressedOnce == 0)
        {
            StartRecording();
            Debug.Log("Started Recording");
            buttonPressedOnce = 1;
        }
        
        // This commented part is for working with Meta Quest
        // if ((OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.Two)) && buttonPressedOnce == 1 )
        if (Input.GetKeyUp(KeyCode.Space) && buttonPressedOnce == 1)
        {
            Debug.Log("Stop Recording");
            StopRecording();
            buttonPressedOnce = 0;
        }
    }
    public void StartRecording()
    {
        clip = Microphone.Start(null, false, 10, 44100);
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
            using (var writer = new BinaryWriter(memoryStream)) {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples) {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }


    public void StopRecording()
    {
            var position = Microphone.GetPosition(null);
            Microphone.End(null);
            var samples = new float[position * clip.channels];
            clip.GetData(samples, 0);
            bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
            
            string objectNames = objectSpawner.GetAllObjectNames();

            string prompt = "Reply to the next audio. You are inside a 3D environment, and you have under your juridisction the next object availables only:" + objectNames;
            
            // Send recording
            StartCoroutine(geminiUnityManager.SendFunctionAudioRequestToGemini(prompt,bytes));
    }

}