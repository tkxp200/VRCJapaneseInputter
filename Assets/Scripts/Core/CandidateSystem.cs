using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CGITransLiterateUtil;
using System.Threading.Tasks;
using System.IO;
using TMPro;
using JPInputTool.Core;
using JPInputTool.Utils;
using System;

public class CandidateSystem : MonoBehaviour
{
    static string convTriePath = Path.Combine(Application.dataPath, @"dic/conversion.lptrie");
    static string connectionPath = Path.Combine(Application.dataPath, @"dic/connection_single_column.txt");
    static string bigramTriePath = Path.Combine(Application.dataPath, @"dic/bigram_jp.lptrie");
    static string bigramEngTriePath = Path.Combine(Application.dataPath, @"dic/bigram_eng.lptrie");
    static string unigramTriePath = Path.Combine(Application.dataPath, @"dic/unigram_eng.lptrie");
    [SerializeField] MainSystem mainSystem;
    [SerializeField] ActionSystem actionSystem;
    [SerializeField] TextManagementSystem textManagementSystem;
    [SerializeField] GameObject candidateButtonPrefab;
    [SerializeField] GameObject candidateParent;
    [SerializeField] RectTransform candidateTransform;
    private JPConverter converter;
    private bool isConverterLoaded = false;
    private List<GameObject> candidateObjects = new List<GameObject>();
    private int characterXSize = 22;
    private int buttonInSet = 40;
    private int defaultButtonHeight = 50;
    private bool isTriggerPressed = false;
    private Vector2 currentHitPosition;
    private Vector2 triggerPressedHitPosition;
    private Vector2 defaultCandidateParentPosition;
    private int dragThreshold = 10;
    private const string candidateTag = "Candidate";
    private int buttonPositionX;

    async void Awake()
    {
        await InitConverter();
    }

    async Task InitConverter()
    {
        converter = await JPConverter.CreateAsync(convTriePath, bigramTriePath, bigramEngTriePath, unigramTriePath, connectionPath);
        isConverterLoaded = true;
    }

    void Start()
    {
        defaultCandidateParentPosition = candidateTransform.anchoredPosition;
        actionSystem.OnTriggerDown += TriggerDown;
        actionSystem.OnTriggerUp += TriggerUp;
        actionSystem.OnHitPositionMove += HitPositionMove;
    }

    void InitCandidateParentPosition()
    {
        candidateTransform.anchoredPosition = defaultCandidateParentPosition;
    }

    void TriggerDown(Vector2 hitPosition, GameObject buttonObject)
    {
        if(buttonObject != null && buttonObject.CompareTag(candidateTag))
        {
            isTriggerPressed = true;
            triggerPressedHitPosition = hitPosition;
            currentHitPosition = hitPosition;
        }
    }

    void HitPositionMove(Vector2? hitPosition)
    {
        if(isTriggerPressed && buttonPositionX > mainSystem.GetWindowSize().x)
        {
            if(hitPosition != null)
            {
                var hitPositionDx = (float)hitPosition?.x - currentHitPosition.x;
                var currentCandidateParentPosition = candidateTransform.anchoredPosition;
                var candidatePositionX = Mathf.Clamp(currentCandidateParentPosition.x + hitPositionDx,
                    mainSystem.GetWindowSize().x-buttonPositionX, defaultCandidateParentPosition.x);
                candidateTransform.anchoredPosition = new Vector2(candidatePositionX, currentCandidateParentPosition.y);
                currentHitPosition = (Vector2)hitPosition;
            }
            else
            {
                isTriggerPressed = false;
            }
        }
    }

    void TriggerUp(Vector2 hitPosition, GameObject buttonObject)
    {
        if(isTriggerPressed)
        {
            if(Mathf.Abs(hitPosition.x - triggerPressedHitPosition.x) < dragThreshold &&
                    buttonObject!= null && buttonObject.CompareTag(candidateTag))
            {
                buttonObject.GetComponent<Button>().onClick.Invoke();
            }
            isTriggerPressed = false;
        }
    }

    public void GenerateCandidate(string writingText)
    {
        // Task _;
        if (isConverterLoaded) CreateCandidateButton(converter.GetCandidates(writingText, 50), writingText.Length);
        // if (mainSystem.GetUseTransLiterate()) _ = GetLiterateAsync(writingText);
    }

    public void GenerateConversion(string convertText)
    {
        if (isConverterLoaded) CreateCandidateButton(converter.GetConversion(convertText, 50), convertText.Length);
    }

    private async Task GetLiterateAsync(string writingText)
    {
        if (writingText == "")
        {
            DestroyCandidateObjects();
            return;
        }
        var result = await TransLiterateSystem.GetJapaneseConversionAsync(writingText);
        CreateCandidateButton(result);
    }

    private void CreateCandidateButton(List<(ResultPair, string)> item, int inputLength)
    {
        DestroyCandidateObjects();
        InitCandidateParentPosition();
        buttonPositionX = 0;
        for (var i = 0; i < item.Count; i++)
        {
            var candidateText = item[i].Item1.Candidate;
            var buttonWidth = candidateText.Length * characterXSize + buttonInSet;
            var candidateObject = Instantiate(candidateButtonPrefab);
            candidateObject.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonWidth, defaultButtonHeight);
            candidateObject.transform.localPosition = new Vector3(buttonPositionX, 0, 0);
            candidateObject.transform.SetParent(candidateParent.transform, false);
            candidateObject.GetComponentInChildren<TextMeshProUGUI>().text = candidateText;
            candidateObject.GetComponent<Button>().onClick.AddListener(() => OnClickCandidateButton(candidateText, inputLength));
            candidateObjects.Add(candidateObject);
            buttonPositionX += buttonWidth;
        }
    }

    private void CreateCandidateButton(List<(ResultPair, string, int)> item, int inputLength)
    {
        DestroyCandidateObjects();
        InitCandidateParentPosition();
        buttonPositionX = 0;
        for (var i = 0; i < item.Count; i++)
        {
            var candidateText = item[i].Item1.Candidate;
            var length = Math.Min(inputLength, item[i].Item3);
            var buttonWidth = candidateText.Length * characterXSize + buttonInSet;
            var candidateObject = Instantiate(candidateButtonPrefab);
            candidateObject.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonWidth, defaultButtonHeight);
            candidateObject.transform.localPosition = new Vector3(buttonPositionX, 0, 0);
            candidateObject.transform.SetParent(candidateParent.transform, false);
            candidateObject.GetComponentInChildren<TextMeshProUGUI>().text = candidateText;
            candidateObject.GetComponent<Button>().onClick.AddListener(() => OnClickCandidateButton(candidateText, length));
            candidateObjects.Add(candidateObject);
            buttonPositionX += buttonWidth;
        }
    }

    private void CreateCandidateButton(Newtonsoft.Json.Linq.JArray item)
    {
        DestroyCandidateObjects();
        InitCandidateParentPosition();
        buttonPositionX = 0;
        for (var i = 1; i <= item.Count; i++)
        {
            var candidateText = item[i].ToString();
            var buttonWidth = candidateText.Length * characterXSize + buttonInSet;
            GameObject candidateObject = Instantiate(candidateButtonPrefab) as GameObject;
            candidateObject.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonWidth, defaultButtonHeight);
            candidateObject.transform.localPosition = new Vector3(buttonPositionX, 0, 0);
            candidateObject.transform.SetParent(candidateParent.transform, false);
            candidateObject.GetComponentInChildren<TextMeshProUGUI>().text = candidateText;
            candidateObject.GetComponent<Button>().onClick.AddListener(() => OnClickCandidateButton(candidateText, item[0].ToString().Length));
            candidateObjects.Add(candidateObject);
            buttonPositionX += buttonWidth;
        }
    }

    private void OnClickCandidateButton(string text, int length)
    {
        DestroyCandidateObjects();
        InitCandidateParentPosition();
        textManagementSystem.TextTransLiterate(text, length);
    }

    private void DestroyCandidateObjects()
    {
        foreach(var obj in candidateObjects)
        {
            Destroy(obj);
        }
    }
}
