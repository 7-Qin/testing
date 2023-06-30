using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GUIEvents : MonoBehaviour {

	void Update () {
		if (name == "FaceBook" || name == "Share" || name == "FaceBookLogout") {
			if (!LevelManager.THIS.FacebookEnable)
				gameObject.SetActive (false);
		}
	}

	public void Settings () {
		SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

		GameObject.Find ("CanvasGlobal").transform.Find ("Settings").gameObject.SetActive (true);

	}

	public void ClaimMCC () {
		SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

		GameObject.Find ("CanvasGlobal").transform.Find ("ClaimMenu").gameObject.SetActive (true);

	}

	//ChuciQin
	public void HaveToConncetWalletBar () {
		SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

		GameObject.Find ("CanvasGlobal").transform.Find ("HaveToConncetWalletBar").gameObject.SetActive (true);

	}

	//ChuciQin
	public void CloseHaveToConncetWalletBar () {
		SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

		GameObject.Find ("CanvasGlobal").transform.Find ("HaveToConncetWalletBar").gameObject.SetActive (false);

	}

	public void Play () {
		SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

		transform.Find ("Loading").gameObject.SetActive (true);
		SceneManager.LoadScene ("game");
	}

	public void OpenTwitter () {
        // 在这里替换为你想要跳转的网址
        Application.OpenURL("https://twitter.com/AzukiCrush");
	}

	public void OpenTelegram () {
        // 在这里替换为你想要跳转的网址
        Application.OpenURL("https://t.me/AzukiCrush");
	}

	public void Pause () {
		SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);

		if (LevelManager.THIS.gameStatus == GameState.Playing)
			GameObject.Find ("CanvasGlobal").transform.Find ("MenuPause").gameObject.SetActive (true);

	}

	public void FaceBookLogin () {
#if FACEBOOK

		FacebookManager.THIS.CallFBLogin ();
#endif
	}

	public void FaceBookLogout () {
		#if FACEBOOK
		FacebookManager.THIS.CallFBLogout ();

		#endif
	}

	public void Share () {
#if FACEBOOK

		FacebookManager.THIS.Share ();
#endif
	}

}
