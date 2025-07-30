using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Material[] materials;

    public GameObject canvas_input;
    public GameObject canvas_success;
    public GameObject canvas_fail;

    public string answerTyped;


    void Start()
    {
        int randomSkyBoxNumber = UnityEngine.Random.Range(0, materials.Length -1);
        RenderSettings.skybox = materials [randomSkyBoxNumber];
        inputField.text = answerTyped;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendAnswer()
    {
        inputField.text = answerTyped;
        CheckAnswer(inputField.text);
    }

    public void TryAgain()
    {
        canvas_fail.SetActive(false);
        canvas_input.SetActive(true);
    }


    public void CheckAnswer(string answer)
    {
        canvas_input.SetActive(false);
        if(RenderSettings.skybox.name == answer){
            canvas_success.SetActive(true);
        } else {
            canvas_fail.SetActive(true);
        }
    }

    //Additional functions

    public void Next()
    {
        canvas_success.SetActive(false);
        RandomEnvironment();
        canvas_input.SetActive(true);
    }


    void RandomEnvironment()
    {
        int randomSkyBoxNumber = UnityEngine.Random.Range(0, materials.Length -1);
        RenderSettings.skybox = materials [randomSkyBoxNumber];
    }
}
