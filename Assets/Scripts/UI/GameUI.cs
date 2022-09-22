using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Helpers.Common;
using System.Linq;

public class GameUI : MonoBehaviour
{
    [Header("UI References")]
    //public TMP_Text pingText;
	public TMP_Text regionText;
    public Button optionsButton;
    //public GameObject totalTaskBarObject;
    //public GameObject taskListObject;
    public GameObject messageScreen;
    public TMP_Text messageText;
    public TMP_Text messageTypeText;
	public TMP_Text messageNicknameText;
    public CanvasGroup messageTextCG;
    public CanvasGroup messageScreenCG;
	//public ColorSelectionUI colorUI;
	public Image messageScreenPanel, messageScreenGlow;
    //[Header("Game Settings References")]
    //public GameObject gameSettingsObject;

	private void Awake()
	{
		SetRegionText(Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion);
	}

	public void SetRegionText(string region)
	{
		regionText.text = $"Region: <color=#FFAD00>{region.ToUpper()}</color>";
	}


	public void InitPregame(Fusion.NetworkRunner runner)
    {
        //gameSettingsObject.SetActive(true);
        //pingText.gameObject.SetActive(true);
		//GameManager.im.nicknameHolder.gameObject.SetActive(true);
	}

	public void InitGame()
	{
		//colorUI.Close();
		//gameSettingsObject.SetActive(false);

		//GameManager.im.nicknameHolder.gameObject.SetActive(false);
		AudioManager.Play("SFX_RoundStarted");

		//Instantiate(GameManager.rm.playerMapIconPrefab, GameManager.im.mapIconHolder).Init(PlayerObject.Local);
	}

	public void Pause()
	{
		if (GameManager.Instance.Runner?.IsRunning == true)
		{
			UIScreen.Focus(GameManager.Instance.pauseScreen);
		}
		else
		{
			UIScreen.Focus(GameManager.Instance.optionsScreen);
		}
	}

	public void CloseOverlay(float delay = 0)
	{
		StartCoroutine(OverlayTimeout(delay, 0.5f));
	}

	//void SetOverlay(string text, string typeText, float timeout = 1)
	//{
	//	StartCoroutine(OverlayFadeIn(2.5f));
	//	messageText.text = text;
	//	messageTypeText.text = typeText;
	//	messageScreenCG.alpha = 0;
	//}

	IEnumerator OverlayFadeIn(float duration)
	{
		messageScreen.SetActive(true);
		float val = 0;
		while(val < duration)
        {
			val += Time.deltaTime;
			messageScreenCG.alpha = val;
			yield return null;
        }
		messageScreenCG.alpha = 1;
	}

	IEnumerator OverlayTimeout(float delay, float duration)
	{
		if (delay > 0) yield return new WaitForSeconds(delay);

		float val = duration;
		while (val > 0)
		{
			val -= Time.deltaTime;
			messageScreenCG.alpha = val;
			yield return null;
		}
		messageScreenCG.alpha = 0;
		yield return new WaitForSeconds(0.25f);
		messageScreen.SetActive(false);
	}
}
