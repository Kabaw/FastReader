using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Program : MonoBehaviour
{
    [Header("Parameters")]

    [Tooltip("Start delay in seconds")]
    [SerializeField] private float startDelay;

    [Tooltip("Word change speed in seconds")]
    [SerializeField] private float wordChangeSpeed;
    [SerializeField] private string displayText;
    [SerializeField] private int rewindAmount;
    [SerializeField] private int fastForwardAmount;
    [SerializeField] private float readSpeedChangeAmount = 0.02f;

    [Header("Inputs")]
    [SerializeField] private float readDelay = 0.02f;
    [SerializeField] private KeyCode pauseKey = KeyCode.Space;
    [SerializeField] private KeyCode rewindKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode fastForwardKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode increaseReadSpeed = KeyCode.UpArrow;
    [SerializeField] private KeyCode decreaseReadSpeed = KeyCode.DownArrow;

    [Header("References")]
    [SerializeField] private TMP_Text textElement;
    [SerializeField] private TMP_InputField textElementParameter;
    [SerializeField] private TMP_InputField rewindTextElementParameter;
    [SerializeField] private TMP_InputField fastForwardTextElementParameter;
    [SerializeField] private TMP_InputField readSpeedTextElementParameter;

    private readonly string defaultText = "--------";

    int counter;
    private bool isPaused = false;
    private Coroutine coroutine;
    List<string> wordsList;

    public bool IsRunning { get { return (coroutine != null); } }

    private void Start()
    {
        StartCoroutine(CommandReaderRoutine());
    }

    private void Update()
    {
        EvaluateParameterChanges();
    }

    private void EvaluateParameterChanges()
    {
        int number;
        float floatNumber;

        if(textElementParameter.text != displayText)
        {
            StopReading();
            displayText = textElementParameter.text;
        }

        number = int.Parse(rewindTextElementParameter.text);
        if (number != rewindAmount)
            rewindAmount = number;

        number = int.Parse(fastForwardTextElementParameter.text);
        if (number != fastForwardAmount)
            fastForwardAmount = number;

        floatNumber = float.Parse(readSpeedTextElementParameter.text);
        if (floatNumber != readSpeedChangeAmount)
        {
            print(readSpeedTextElementParameter.text);
            print(floatNumber);
            readSpeedChangeAmount = floatNumber;
        }
    }

    private void ActivateCommand()
    {
        if (!IsRunning)
            return;

        if (!Input.anyKey)
            return;

        if (Input.GetKeyDown(pauseKey))
        {
            PauseReading();
            return;
        }

        if (Input.GetKeyDown(rewindKey))
        {
            Rewind();
            return;
        }

        if (Input.GetKeyDown(fastForwardKey))
        {
            FastForward();
            return;
        }

        if (Input.GetKeyDown(increaseReadSpeed))
        {
            IncreaseReadSpeed();
            return;
        }

        if (Input.GetKeyDown(decreaseReadSpeed))
        {
            DecreaseReadSpeed();
            return;
        }
    }

    public void StartReading()
    {
        coroutine = StartCoroutine(ReadingRoutine());
    }

    public void StopReading()
    {
        if (!IsRunning)
            return;
        
        StopCoroutine(coroutine);
        coroutine = null;

        textElement.text = defaultText;
    }

    public void PauseReading()
    {
        if (!IsRunning)
            return;

        isPaused = !isPaused;
    }

    public void Rewind()
    {
        if (!IsRunning)
            return;

        counter -= rewindAmount;
        SetText();
    }

    public void FastForward()
    {
        if (!IsRunning)
            return;

        counter += fastForwardAmount;
        SetText();
    }

    public void IncreaseReadSpeed()
    {
        SetReadSpeed(wordChangeSpeed -= readSpeedChangeAmount);
    }
    
    public void DecreaseReadSpeed()
    {
        SetReadSpeed(wordChangeSpeed += readSpeedChangeAmount);
    }

    private void SetReadSpeed(float readSpeed)
    {
        if (!IsRunning)
            return;

        wordChangeSpeed = Mathf.Clamp(readSpeed, 0.1f, 1);
        wordChangeSpeed = (float) Math.Round(wordChangeSpeed, 2);
    }

    private void SetText()
    {
        if (!IsRunning)
            return;

        counter = Mathf.Clamp(counter, 0, wordsList.Count - 1);
        textElement.text = wordsList[counter];
    }    

    private IEnumerator ReadingRoutine()
    {       
        WaitForSeconds startDelayWait = new(startDelay);        

        wordsList = new(displayText.Split(new char[] { ' ', '\n' }));

        yield return startDelayWait;

        counter = 0;
        while (counter < wordsList.Count)
        {
            while (isPaused)
                yield return null;

            SetText();
            yield return new WaitForSeconds(wordChangeSpeed);

            counter++;
        }
    }
    private IEnumerator CommandReaderRoutine()
    {
        while (true)
        {
            //yield return new WaitForSeconds(readDelay);

            ActivateCommand();
            yield return null;
        }
    }
}
