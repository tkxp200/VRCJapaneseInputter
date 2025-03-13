using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CGITransLiterateUtil;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TMPro;

public class CandidateSystem : MonoBehaviour
{
    [SerializeField] MainSystem mainSystem;
    [SerializeField] ActionSystem actionSystem;
    [SerializeField] TextManagementSystem textManagementSystem;
    [SerializeField] GameObject candidateButtonPrefab;
    [SerializeField] GameObject candidateParent;
    [SerializeField] RectTransform candidateTransform;
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

    void HitPositionMove(Vector2 hitPosition)
    {
        if(isTriggerPressed && buttonPositionX > mainSystem.GetWindowSize().x)
        {
            var hitPositionDx = hitPosition.x - currentHitPosition.x;
            var currentCandidateParentPosition = candidateTransform.anchoredPosition;
            var candidatePositionX = Mathf.Clamp(currentCandidateParentPosition.x + hitPositionDx,
                mainSystem.GetWindowSize().x-buttonPositionX, defaultCandidateParentPosition.x);
            candidateTransform.anchoredPosition = new Vector2(candidatePositionX, currentCandidateParentPosition.y);
            currentHitPosition = hitPosition;
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
        Task _;
        if(mainSystem.GetUseTransLiterate()) _ = GetLiterateAsync(writingText);
    }

    private async Task GetLiterateAsync(string writingText)
    {
        if(writingText == "")
        {
            DestroyCandidateObjects();
            return;
        }
        var result = await TransLiterateSystem.GetJapaneseConversionAsync(writingText);
        CreateCandidateButton(result);
    }

    private void CreateCandidateButton(Newtonsoft.Json.Linq.JArray item)
    {
        DestroyCandidateObjects();
        InitCandidateParentPosition();
        buttonPositionX = 0;
        for(var i = 1; i <= item.Count; i++)
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
