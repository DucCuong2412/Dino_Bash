using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBBDownloader : MonoBehaviour
{
	private string current_message = "Loading marvelous adventures, please stand by ...";

	public Font font;

	public SpriteRenderer blende;

	public SpriteRenderer hourglass;

	public Transform disableAfterLoad;

	private List<GoTween> tweens = new List<GoTween>();

	public float opacity { get; set; }

	private string getTitle_text()
	{
		switch (Application.systemLanguage)
		{
		case SystemLanguage.English:
			return "Loading incredible adventures, please stand by ...";
		case SystemLanguage.French:
			return "Chargement d'aventures merveilleuses, merci de patienter...";
		case SystemLanguage.Portuguese:
			return "Carregando aventuras incríveis, aguarde...";
		case SystemLanguage.German:
			return "Lade wunderbare Abenteuer ...";
		case SystemLanguage.Spanish:
			return "Espera un momento, estamos cargando aventuras maravillosas...";
		case SystemLanguage.Italian:
			return "Caricamento avventure meravigliose. Attendere, prego...";
		case SystemLanguage.Russian:
			return "Идет загрузка невероятных приключений, немного подождите...";
		default:
			return "Loading incredible adventures, please stand by ...";
		}
	}

	private string getStorageNotAvailiable_text()
	{
		switch (Application.systemLanguage)
		{
		case SystemLanguage.English:
			return "External storage is not available!";
		case SystemLanguage.French:
			return "Stockage externe indisponible !";
		case SystemLanguage.Portuguese:
			return "Armazenamento externo não disponível!";
		case SystemLanguage.German:
			return "Externer Speicher nicht verfügbar!";
		case SystemLanguage.Spanish:
			return "¡El almacenamiento externo no está disponible!";
		case SystemLanguage.Italian:
			return "Memoria esterna non disponibile!";
		case SystemLanguage.Russian:
			return "Внешняя память недоступна!";
		default:
			return "External storage is not available!";
		}
	}

	private string getCheckingData_text()
	{
		switch (Application.systemLanguage)
		{
		case SystemLanguage.English:
			return "Checking data ...";
		case SystemLanguage.French:
			return "Vérification des données...";
		case SystemLanguage.Portuguese:
			return "Conferindo dados...";
		case SystemLanguage.German:
			return "Überprüfe Daten ...";
		case SystemLanguage.Spanish:
			return "Comprobando datos…";
		case SystemLanguage.Italian:
			return "Controllo dati...";
		case SystemLanguage.Russian:
			return "Проверка данных...";
		default:
			return "Checking data ...";
		}
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		opacity = 1f;
		current_message = getTitle_text();
		StartCoroutine(LoadLevel());
	}

	private IEnumerator OBBLoaderCoroutine()
	{
		string expPath = GooglePlayDownloader.GetExpansionFilePath();
		if (expPath == null)
		{
			Debug.LogError("External storage is not available!");
			current_message = getStorageNotAvailiable_text();
			yield break;
		}
		string mainPath = GooglePlayDownloader.GetMainOBBPath(expPath);
		Debug.Log("expPath = " + expPath);
		Debug.Log("Main = ..." + ((mainPath != null) ? mainPath.Substring(expPath.Length) : " NOT AVAILABLE"));
		if (mainPath == null)
		{
			Debug.Log("Trying to fetch OBB ...");
			GooglePlayDownloader.FetchOBB();
			Debug.Log("OBB Download suposendly done, lets check it");
			current_message = getCheckingData_text();
			while (GooglePlayDownloader.GetMainOBBPath(expPath) == null)
			{
				yield return new WaitForSeconds(0.1f);
			}
			while (Resources.Load("obb_test_resource") == null)
			{
				yield return new WaitForSeconds(0.1f);
			}
			StartCoroutine(LoadLevel());
		}
		else
		{
			Debug.Log("OBB already on device");
			StartCoroutine(LoadLevel());
		}
	}

	private IEnumerator LoadLevel()
	{
		Application.LoadLevelAdditive(1);
		yield return null;
		bool wait = true;
		while (wait)
		{
			wait = ScreenManager.GetScreen<StartScreen>() == null;
			yield return null;
		}
		Object.Destroy(disableAfterLoad.gameObject);
		Color c2 = blende.color;
		c2.a = 0f;
		tweens.Add(Go.to(blende, 0.3f, new GoTweenConfig().colorProp("color", c2)));
		c2 = hourglass.color;
		c2.a = 0f;
		tweens.Add(Go.to(hourglass, 0.3f, new GoTweenConfig().colorProp("color", c2)));
		tweens.Add(Go.to(this, 0.3f, new GoTweenConfig().floatProp("opacity", 0f)));
	}

	private void OnLevelWasLoaded()
	{
		tweens.ForEach(delegate(GoTween t)
		{
			t.destroy();
		});
		Object.Destroy(base.gameObject);
	}

	private void OnGUI()
	{
		if (!string.IsNullOrEmpty(current_message))
		{
			GUIStyle style = GUI.skin.GetStyle("Label");
			style.font = font;
			style.fontSize = 30;
			style.alignment = TextAnchor.UpperCenter;
			style.normal.textColor = new Color(1f, 1f, 1f, opacity);
			GUI.Label(new Rect(10f, (float)Screen.height * 0.025f, Screen.width - 20, Screen.height - 20), current_message, style);
		}
	}
}
