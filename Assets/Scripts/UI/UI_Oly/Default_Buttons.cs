using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Default_Buttons : MonoBehaviour
{

    private VisualElement defaultPage;
    private VisualElement settingPage;
    private VisualElement tutorialPage;
    private VisualElement gameUiPage;

    private Button settingButton;
    private Button tutorialButton;
    private Button surrenderButton;
    private Button startGameButton;

    private Button closeSettingButton;
    private Button closeTutoButton;
    
    private Label playerScores;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        defaultPage = root.Q<VisualElement>("Default_Hi-if");
        settingPage = root.Q<VisualElement>("Setting_Hi-if");
        tutorialPage = root.Q<VisualElement>("Tuto_Hi-if");

        settingButton = root.Q<Button>("Setting_Btn");
        tutorialButton = root.Q<Button>("Tuto_Btn");
        surrenderButton = root.Q<Button>("Surrender_Btn");
        startGameButton = root.Q<Button>("Start_Game");

        closeSettingButton = root.Q<Button>("Close_Setting_Btn");
        closeTutoButton = root.Q<Button>("Close_Tuto_Btn");

        playerScores = root.Q<Label>("Player_Scores");

        settingButton.clicked += SettingButtonPressed;
        tutorialButton.clicked += TutorialButtonPressed;
        surrenderButton.clicked += SurrenderButtonPressed;
        startGameButton.clicked += StartGameButtonPressed;

        closeSettingButton.clicked += CloseSettingButtonPressed;
        closeTutoButton.clicked += CloseTutoButtonPressed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PreGameHost()
    {
        // Display Start, Settings & Tutorial buttons
        Debug.Log("Display start, settings and tutorial");
    }

    public void PreGameJoin()
    {
        // Display Settings & Tutorial buttons
        Debug.Log("Settings and tutorial");
    }

    public void StartGameButtonPressed()
    {
        GameManager.Instance.Server_StartGame();                // Start the game

        defaultPage.style.display = DisplayStyle.Flex;

        settingPage.style.display = DisplayStyle.None;
    }

    void SettingButtonPressed()
    {
        defaultPage.style.display = DisplayStyle.None;

        settingPage.style.display = DisplayStyle.Flex;
    }    
    void TutorialButtonPressed()
    {
        defaultPage.style.display = DisplayStyle.None;

        tutorialPage.style.display = DisplayStyle.Flex;
    }
    void SurrenderButtonPressed()
    {
        //settingPage.style.display = DisplayStyle.None;
    }

    void CloseSettingButtonPressed()
    {
        defaultPage.style.display = DisplayStyle.Flex;

        settingPage.style.display = DisplayStyle.None;
    }
    void CloseTutoButtonPressed()
    {
        defaultPage.style.display = DisplayStyle.Flex;

        tutorialPage.style.display = DisplayStyle.None;
        
    }


    public void Scores(string winningText)
    {
        //playerScores.text = $"Player 1: {player1Score} \n\n Player 2: {player2Score} \n\n Player 3: {player3Score} \n\n Player 4: {player4Score}";
        playerScores.text = winningText;
    }
}
