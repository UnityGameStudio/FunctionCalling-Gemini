using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using TMPro;
using System.IO; 
using System;
// using Oculus.Interaction;
// using Oculus.Interaction.Input;

[System.Serializable]
public class UnityAndGeminiKey
{
    public string key;
}

[System.Serializable]
public class InlineData
{
    public string mimeType;
    public string data;
}

// Text-only part
[System.Serializable]
public class TextPart
{
    public string text;
}


[System.Serializable]
public class TextContent
{
    public string role;
    public TextPart[] parts;
}

[System.Serializable]
public class TextCandidate
{
    public TextContent content;
}

[System.Serializable]
public class TextResponse
{
    public TextCandidate[] candidates;
}

///////////////////////////////////////////////////

[System.Serializable]
public class FunctionResponse
{
    public FunctionCandidate[] candidates;
}

[System.Serializable]
public class FunctionCandidate
{
    public FunctionContent content;
}

[System.Serializable]
public class FunctionContent
{
    public string role;
    public FunctionPart[] parts;
}

[System.Serializable]
public class FunctionPart
{
    public FunctionCall functionCall;
}

[System.Serializable]
public class FunctionCall
{
    public string name;
    public Args args;
}


[System.Serializable]
public class Args
{
    public string argument;
}

//////////////////////////////////////////////////////

public class GeminiUnityManager: MonoBehaviour
{
    [Header("JSON API Configuration")]
    public TextAsset jsonApi;

    [Header("Function Declaration")]
    public ObjectSpawner objectSpawner;
    public ObjectNotAvailable objectNotAvailable;

    private string apiKey = ""; 
    private string apiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent"; // Edit it and choose your prefer model


    void Start()
    {
        UnityAndGeminiKey jsonApiKey = JsonUtility.FromJson<UnityAndGeminiKey>(jsonApi.text);
        apiKey = jsonApiKey.key;   
    }


    public IEnumerator SendPromptRequestToGemini(string promptText)
    {
        string url = $"{apiEndpoint}?key={apiKey}";
     
        string jsonData = "{\"contents\": [{\"parts\": [{\"text\": \"{" + promptText + "}\"}]}]}";

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        // Create a UnityWebRequest with the JSON data
        using (UnityWebRequest www = new UnityWebRequest(url, "POST")){
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError(www.error);
            } else {
                Debug.Log("Request complete!");
                TextResponse response = JsonUtility.FromJson<TextResponse>(www.downloadHandler.text);
                if (response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
                    {
                        //This is the response to your request
                        string text = response.candidates[0].content.parts[0].text;
                        Debug.Log(text);
                    }
                else
                {
                    Debug.Log("No text found.");
                }
            }
        }
    }


    public IEnumerator SendFunctionAudioRequestToGemini(string promptText, byte[] bytes)
    {

        string base64Media = System.Convert.ToBase64String(bytes);

        string url = $"{apiEndpoint}?key={apiKey}";

        string mimeTypeMedia = "audio/wav";



    string jsonBody = $@"
    {{
        ""contents"": [
            {{
                ""parts"": [
                    {{
                        ""text"": ""{promptText}""
                    }},
                    {{
                        ""inline_data"": {{
                            ""mime_type"": ""{mimeTypeMedia}"",
                            ""data"": ""{base64Media}""
                        }}
                    }}
                ]
            }}
        ],
        ""tools"": [
            {{
                ""functionDeclarations"": [
                    {{
                        ""name"": ""SpawnObjectWithName"",
                        ""description"": ""Spawn a 3D item in front of the user."",
                        ""parameters"": {{
                            ""type"": ""object"",
                            ""properties"": {{
                                ""argument"": {{
                                    ""type"": ""string"",
                                    ""description"": ""The object name, e.g. Apple""
                                }}
                            }},
                            ""required"": [""argument""]
                        }}
                    }},

                    {{
                        ""name"": ""ObjectNoAvailable"",
                        ""description"": ""Spawn a sorry signal to the user."",
                        ""parameters"": {{
                            ""type"": ""object"",
                            ""properties"": {{}}
                        }}
                    }},
                ]
            }}
        ]
    }}";


        Debug.Log("Sending JSON: " + jsonBody); // For debugging

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        // Create and send the request
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) 
            {
                Debug.LogError(www.error);
                Debug.LogError("Response: " + www.downloadHandler.text);
            } 
            else {
                Debug.Log("Request complete!");
                FunctionResponse response = JsonUtility.FromJson<FunctionResponse>(www.downloadHandler.text);
                if (response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
                    {
                        Debug.Log(www.downloadHandler.text);

                        if (response.candidates[0].content.parts[0].functionCall != null)
                        {
                            string functionName = response.candidates[0].content.parts[0].functionCall.name;
                            
                            if (response.candidates[0].content.parts[0].functionCall.args != null)
                            {
                                string functionArgument = response.candidates[0].content.parts[0].functionCall.args.argument;
                                Debug.Log($"Function call detected: {functionName} with argument: {functionArgument}");
                                
                                // Execute the appropriate function based on the name
                                if (functionName == "SpawnObjectWithName")
                                {
                                    objectSpawner.SpawnObjectWithName(functionArgument);
                                } 
                                else if (functionName == "ObjectNoAvailable")
                                {
                                   objectNotAvailable.NoObjects(); 
                                }
                                // else if (functionName == "spawnObjectAtIndex")
                                // {
                                //     if (int.TryParse(functionArgument, out int index))
                                //     {
                                //         objectSpawner.SpawnObjectAtIndex(index);
                                //     }
                                //     else
                                //     {
                                //         Debug.LogError($"Invalid index argument: {functionArgument}");
                                //     }
                                // }
                                else
                                {
                                    Debug.LogWarning($"Unknown function called: {functionName}");
                                }
                            }
                            else
                            {
                                Debug.Log($"Error");
                            }
                        }
                   }
            }
        }
    }

}