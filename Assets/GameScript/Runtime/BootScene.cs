using System;
using System.Collections;
using UnityEngine;
using YooAsset;

public class BootScene : MonoBehaviour
{
	public static BootScene Instance { private set; get; }

	void Awake()
	{
		Instance = this;

		Application.targetFrameRate = 60;
		Application.runInBackground = true;
	}
	void OnDestroy()
	{
		Instance = null;
	}
	IEnumerator Start()
	{
		var createParameters = new YooAssets.HostPlayModeParameters();
		createParameters.LocationRoot = "Assets/GameRes";
		createParameters.DecryptionServices = null;
		createParameters.ClearCacheWhenDirty = false;
		createParameters.IgnoreResourceVersion = true;
		createParameters.DefaultHostServer = GetHostServerURL();
		createParameters.FallbackHostServer = GetHostServerURL();
		yield return YooAssets.InitializeAsync(createParameters);

		// 运行补丁流程
		PatchUpdater.Run();
	}
	void Update()
	{
		EventManager.Update();
		FsmManager.Update();
	}

	public static string GetHostServerURL()
	{
		//string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
		string hostServerIP = "http://127.0.0.1";

		if (Application.platform == RuntimePlatform.Android)
			return $"{hostServerIP}/CDN/Android";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			return $"{hostServerIP}/CDN/IPhone";
		else
			return $"{hostServerIP}/CDN/PC";
	}
}