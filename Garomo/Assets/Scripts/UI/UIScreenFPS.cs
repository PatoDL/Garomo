using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScreenFPS : MonoBehaviour
{
	[SerializeField]
	private Text fpsdisplay = null;

	[SerializeField]
	private float updateRate = 1;

	private float t;

	private float deltaAcum;

	private int frames;
	private string debugInfo = "";

	public int forceFPSto = 30;
	public int lastFPSset = 30;

	// Update is called once per frame
	void Update()
	{
		t += Time.unscaledDeltaTime;
		frames++;
		if (t > 1 / updateRate)
		{
			float fps = frames / t;
			float averageDT = t / frames * 1000;
			frames = 0;
			t -= 1 / updateRate;
			fpsdisplay.text = $"{averageDT.ToString("F2")} ms ({fps.ToString("F1")} fps) {debugInfo}";
		}

		if (forceFPSto != lastFPSset)
		{
			Application.targetFrameRate = forceFPSto;
			lastFPSset = forceFPSto;
		}
	}
}
