using System;
using System.Collections;
using UnityEngine;
using YooAsset;

public class BootScene : MonoBehaviour
{
	public static BootScene Instance { private set; get; }
	public static YooAssets.EPlayMode GamePlayMode;

	public YooAssets.EPlayMode PlayMode = YooAssets.EPlayMode.EditorSimulateMode;

	void Awake()
	{
		Instance = this;

		Application.targetFrameRate = 60;
		Application.runInBackground = true;
	}
	void OnGUI()
	{
		GUIConsole.OnGUI();
	}
	void OnDestroy()
	{
		Instance = null;
	}
	void Update()
	{
		EventManager.Update();
		FsmManager.Update();
	}

	IEnumerator Start()
	{
		GamePlayMode = PlayMode;
		Debug.Log($"资源系统运行模式：{PlayMode}");

		// 编辑器下的模拟模式
		if (PlayMode == YooAssets.EPlayMode.EditorSimulateMode)
		{
			var createParameters = new YooAssets.EditorSimulateModeParameters();
			createParameters.LocationServices = new DefaultLocationServices("Assets/GameRes");
			//createParameters.SimulatePatchManifestPath = GetPatchManifestPath();
			yield return YooAssets.InitializeAsync(createParameters);
		}

		// 单机运行模式
		if (PlayMode == YooAssets.EPlayMode.OfflinePlayMode)
		{
			var createParameters = new YooAssets.OfflinePlayModeParameters();
			createParameters.LocationServices = new DefaultLocationServices("Assets/GameRes");
			yield return YooAssets.InitializeAsync(createParameters);
		}

		// 联机运行模式
		if (PlayMode == YooAssets.EPlayMode.HostPlayMode)
		{
			var createParameters = new YooAssets.HostPlayModeParameters();
			createParameters.LocationServices = new DefaultLocationServices("Assets/GameRes");
			createParameters.RemoteHostServices = new YooAssetRemoteAddress("v1.0", "http://127.0.0.1");
			createParameters.DecryptionServices = null;
			createParameters.ClearCacheWhenDirty = false;
			yield return YooAssets.InitializeAsync(createParameters);
		}

		// 运行补丁流程
		PatchUpdater.Run();
	}
}