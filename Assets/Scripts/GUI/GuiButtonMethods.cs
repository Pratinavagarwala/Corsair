using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;

[RequireComponent(typeof(AudioSource))]
public class GuiButtonMethods : MonoBehaviour
{
	private const string Url = "http://finalparsec.com/scores/";

    private ObjectManager objectManager;

	private Text sendWaveTime;
    private Text sendWaveName;
    
	private Text healthValue;
	private Text moneyValue;

	private GameObject optionScreen;
	private Animator optionAnimator;

	private GameObject gameOverScreen;
	private Animator gameOverAnimator;
	private Text condition;
	private Text score;

	private GameObject submitScoreScreen;
	private Animator submitScoreAnimator;

	private GameObject highScoreScreen;
	private Animator highScoreAnimator;

	private GameObject upgradeMenu;
	private Animator upgradeAnimator;

	private GameObject selectionMenu;
    private TurretSelectionMenu TurretScript;
    private Animator selectionAnimator;

    private Animator PauseButtonAnimator;

    private Image SelectedTurretIcon;

    private bool gridToggle = true;

    private GameSpeed prePauseSpeed;
    
    void Start()
    {
		objectManager = ObjectManager.GetInstance();

        // Send Wave Button
        sendWaveTime = GameObject.Find("SendWaveTime").GetComponent<Text>();
        sendWaveName = GameObject.Find ("SendWaveName").GetComponent<Text>();

		//Money And Health
		healthValue = GameObject.Find ("HealthValue").GetComponent<Text>();
		moneyValue = GameObject.Find ("MoneyValue").GetComponent<Text>();

		// Option Screen
		optionScreen = GameObject.Find ("OptionsScreen");
		optionAnimator = optionScreen.GetComponent<Animator>();
		optionScreen.SetActive (false);

		// Game Over Screen
		gameOverScreen = GameObject.Find ("GameOverScreen");
		gameOverAnimator = gameOverScreen.GetComponent<Animator>();
		condition = GameObject.Find ("GameOverCondition").GetComponent<Text> ();
		score = GameObject.Find ("GameOverScore").GetComponent<Text> ();
		gameOverScreen.SetActive (false);

		// Submit Score Screen
		submitScoreScreen = GameObject.Find ("SubmitScoreScreen");
		submitScoreAnimator = submitScoreScreen.GetComponent<Animator>();
		submitScoreScreen.SetActive (false);

		// High Score Screen
		highScoreScreen = GameObject.Find ("HighScoreScreen");
		highScoreAnimator = highScoreScreen.GetComponent<Animator>();
		highScoreScreen.SetActive (false);

		// Turret Upgrade Menu
		upgradeMenu = GameObject.Find ("TurretUpgradePanel");
		upgradeAnimator = upgradeMenu.GetComponent<Animator>();

        // Turret Upgrade Menu
        selectionMenu = GameObject.Find("TurretSelectPanel");
        TurretScript = selectionMenu.GetComponent<TurretSelectionMenu>();
        selectionAnimator = selectionMenu.GetComponent<Animator>();

        // Pause Resume Button
        PauseButtonAnimator = GameObject.Find("PauseResume").GetComponent<Animator>();

        // Selected Turret Icon
        SelectedTurretIcon = GameObject.Find("SelectedTurretIcon").GetComponent<Image>();

    }
	
	void Update()
	{
        if (objectManager.gameState.waveCount >= objectManager.gameState.numberOfWaves)
        {
            sendWaveTime.gameObject.SetActive(false);
        }
        sendWaveTime.text = objectManager.gameState.nextWaveCountDown.ToString();

		moneyValue.text = String.Format("{0:n}", objectManager.gameState.playerMoney * 1000000);
		healthValue.text = objectManager.gameState.PlayerHealth.ToString();

		if(objectManager.gameState.waveCount >= objectManager.gameState.numberOfWaves &&
		   objectManager.enemies.Count == 0 &&
		   objectManager.gameState.PlayerHealth > 0 &&
		   !objectManager.gameState.gameOver)
		{
			
			EndGame("Victory!");
            objectManager.AudioManager.PlayEndGame(true);
		}
		else if(objectManager.gameState.PlayerHealth <= 0 && !objectManager.gameState.gameOver)
		{
			EndGame("Defeat!");
            objectManager.AudioManager.PlayEndGame(false);
        }
	}

	private void EndGame(string conditionText)
	{
		objectManager.Map.ad.ShowInterstitial ();
		
		gameOverScreen.SetActive (true);
		gameOverAnimator.SetTrigger("Fade In");
		condition.text = conditionText;
		score.text = "Score: " + objectManager.gameState.score;
		
		objectManager.gameState.gameOver = true;
	}

    public void TurretButtonPressed(int turretType)
	{
		objectManager.AudioManager.PlayButtonSound();
        objectManager.TurretFactory.TurretType = (TurretType)turretType;
        SelectedTurretIcon.sprite = TurretScript.turretSprites[((TurretType)turretType).ToString()];
    }

	public void SendWavePressed()
	{
        objectManager.AudioManager.PlayButtonSound();
        if (!objectManager.gameState.gameStarted)
		{
			var anim = GameObject.Find("SendWave").GetComponent<Animator>();
			anim.SetBool("StartGame", true);
			anim.speed = 20;

			sendWaveName.text = string.Empty;
            sendWaveTime.color = new Color(.2f, .2f, .2f, 1);
            objectManager.gameState.gameStarted = true;
		}
        
		objectManager.WaveManager.playerTriggeredWave = true;
			
		// only if you actually sent a wave and it isn't just almost the end of the game
		if(objectManager.gameState.waveCount < objectManager.gameState.numberOfWaves && !objectManager.gameState.gameOver)
		{
			objectManager.gameState.playerMoney += objectManager.gameState.nextWaveCountDown;
			objectManager.gameState.score += objectManager.gameState.nextWaveCountDown;
		}
	}

	public void OptionPressed()
	{
		objectManager.AudioManager.PlayButtonSound();

		if (objectManager.gameState.gameOver)
		{
			return;
		}

		objectManager.gameState.optionsOn = !optionScreen.activeSelf;
		if(!optionScreen.activeSelf){
			optionScreen.SetActive (true);
			optionAnimator.SetTrigger("Fade In");
		}else {
			optionAnimator.SetTrigger("Fade Out");
		}
	}

	public void MutePressed()
	{
        objectManager.AudioManager.PlayButtonSound();
        objectManager.gameState.isMuted = !objectManager.gameState.isMuted;
	}

	public void DisplayGridPressed()
	{
		objectManager.AudioManager.PlayButtonSound();
		gridToggle = !gridToggle;
		//objectManager.Map.SetGrid(gridToggle);
		// TODO: make work
	}

	public void QuitPressed()
	{
		objectManager.AudioManager.PlayButtonSound();
		Application.Quit ();
	}

	public void MainMenuPressed()
	{
		objectManager.AudioManager.PlayButtonSound();
		objectManager.DestroySinglton();
		Application.LoadLevel("Main Menu");
	}

	public void HighScorePressed()
	{
		objectManager.AudioManager.PlayButtonSound();
		gameOverScreen.SetActive (false);

		submitScoreScreen.SetActive (true);
		submitScoreAnimator.SetTrigger("Fade In");
	}

	public void SubmitScorePressed()
	{
		objectManager.AudioManager.PlayButtonSound();

		if (string.IsNullOrEmpty(GameObject.Find("PlayerNameTextField").GetComponent<Text>().text))
		{
			return;
		}

		submitScoreAnimator.SetTrigger ("Fade Out");

		highScoreScreen.SetActive (true);
		highScoreAnimator.SetTrigger("Fade In");
		
		this.StartCoroutine(this.SendRequest());
		Text yourScoreLabel = GameObject.Find("HighScoreScore").GetComponent<Text>();
		yourScoreLabel.text = "Your Score: " + ObjectManager.GetInstance().gameState.score.ToString();

	}

	private IEnumerator SendRequest()
	{
		Text nameList = GameObject.Find ("HighScoreNames").GetComponent<Text>();
		Text scoreList = GameObject.Find ("HighScoreScores").GetComponent<Text>();

		string playerName = GameObject.Find("PlayerNameTextField").GetComponent<Text>().text;
		int score = ObjectManager.GetInstance().gameState.score;
		
		string leaderboardName = string.Format(
			"Aurora TD {0} {1} {2}",
			ObjectManager.GetInstance().gameState.MapType,
			ObjectManager.GetInstance().gameState.friendlyDifficulty,
			ObjectManager.GetInstance().gameState.numberOfWaves == 300
			? "Endless"
			: ObjectManager.GetInstance().gameState.numberOfWaves.ToString());
		
		string modifiedUrl = Url + string.Format("{0}?limit={1}&player_name={2}&score={3}", leaderboardName, 10, playerName, score);
		modifiedUrl = modifiedUrl.Replace(" ", "%20");
		Debug.Log(modifiedUrl);
		
		WWW www = new WWW(modifiedUrl);
		
		yield return www;
		
		JSONNode jsonNode = JSON.Parse(www.text);
		
		nameList.text = string.Empty;
		scoreList.text = string.Empty;
		int position = 1;
		for (int x = 0; x < jsonNode["competitors"].Count; x++)
		{
			//format list of names with numbers and names
			nameList.text += position + ":\t" + jsonNode["competitors"][x]["player_name"].Value + "\n";
			
			//dump list of scores
			scoreList.text += jsonNode["competitors"][x]["score"].AsInt + "\n";

			position++;
		}
	}
    
    public void TurretMenuToggelPressed()
    {

        if (!selectionAnimator.GetCurrentAnimatorStateInfo(0).IsName("ScreenSwipeRightIn"))
        {
            objectManager.AudioManager.PlayButtonSound();
            selectionAnimator.SetTrigger("Swipe Right In");

            if(objectManager.TurretFocusMenu.SelectedTurret != null)
            {
                objectManager.TurretFocusMenu.SelectedTurret = null;
            }
        }
        else
        {
            objectManager.AudioManager.PlayButtonSound();
            selectionAnimator.SetTrigger("Swipe Right Out");
        }
    }

    public void CloseTurretMenu()
    {
        if (selectionAnimator.GetCurrentAnimatorStateInfo(0).IsName("ScreenSwipeRightIn"))
        {
            selectionAnimator.SetTrigger("Swipe Right Out");
        }
    }

    public void RemoveTurretUpgradeMenu()
    {
        objectManager.TurretFocusMenu.isActive = false;
        upgradeAnimator.SetTrigger("Swipe Out");
    }

    public void TurretUpgradeToggelPressed()
    {

        if (!upgradeAnimator.GetCurrentAnimatorStateInfo(0).IsName("ScreenSwipeRightIn"))
        {
            objectManager.TurretFocusMenu.isActive = true;
            objectManager.AudioManager.PlayButtonSound();
            upgradeAnimator.SetTrigger("Swipe Right In");
        }
        else
        {
            objectManager.TurretFocusMenu.isActive = false;
            objectManager.AudioManager.PlayButtonSound();
            upgradeAnimator.SetTrigger("Swipe Right Out");
        }
    }

    public void CycleSpeed()
    {
        objectManager.AudioManager.PlayButtonSound();
        switch (objectManager.gameState.GameSpeed)
        {
            case GameSpeed.X1:
                objectManager.gameState.GameSpeed = GameSpeed.X2;
                break;

            case GameSpeed.X2:
                objectManager.gameState.GameSpeed = GameSpeed.X3;
                break;

            case GameSpeed.X3:
                objectManager.gameState.GameSpeed = GameSpeed.X1;
                break;
        }
    }

    public void PauseResume()
    {
        objectManager.AudioManager.PlayButtonSound();
        if (objectManager.gameState.GameSpeed == GameSpeed.Paused)
        {
            objectManager.gameState.GameSpeed = prePauseSpeed;
        }
        else
        {
            prePauseSpeed = objectManager.gameState.GameSpeed;
            objectManager.gameState.GameSpeed = GameSpeed.Paused;
        }
        PauseButtonAnimator.SetTrigger("toggle");
    }

    // Called when the sell button is pressed.
    public void Sell()
    {
        if (objectManager.TurretFocusMenu.SelectedTurret == null)
            return;

        objectManager.AudioManager.PlaySellSound();

        objectManager.gameState.playerMoney += objectManager.TurretFocusMenu.SelectedTurret.Msrp;
        objectManager.NodeManager.UnBlockNode(objectManager.TurretFocusMenu.SelectedTurret.transform.position);
        Destroy(objectManager.TurretFocusMenu.SelectedTurret.gameObject);
        objectManager.TurretFocusMenu.SelectedTurret = null;
    }
}