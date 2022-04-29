using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using YooAsset;

public class Game1Scene : MonoBehaviour
{
	public GameObject CanvasRoot;

	private readonly List<AssetOperationHandle> _cachedAssetOperationHandles = new List<AssetOperationHandle>(1000);
	private readonly List<SubAssetsOperationHandle> _cachedSubAssetsOperationHandles = new List<SubAssetsOperationHandle>(1000);

	void Start()
	{
		YooAssets.UnloadUnusedAssets();

		// 初始化窗口
		InitWindow();

		// 异步编程
		this.StartCoroutine(AsyncLoad1());

		// 异步编程
		AsyncLoad2();
	}
	void OnDestroy()
	{
		foreach(var handle in _cachedAssetOperationHandles)
		{
			handle.Release();
		}
		_cachedAssetOperationHandles.Clear();

		foreach (var handle in _cachedSubAssetsOperationHandles)
		{
			handle.Release();
		}	
		_cachedSubAssetsOperationHandles.Clear();
	}
	void OnGUI()
	{
		GUIConsole.OnGUI();
	}

	void InitWindow()
	{
		var version = CanvasRoot.transform.Find("version/res_version").GetComponent<Text>();
		version.text = $"Resource ver : {YooAssets.GetResourceVersion()}";

		// 同步加载预制体
		{
			var btn = CanvasRoot.transform.Find("entity/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				var icon = CanvasRoot.transform.Find("entity/icon").GetComponent<Image>();
				AssetOperationHandle handle = YooAssets.LoadAssetSync<GameObject>("Entity/Level1/footman_Blue");
				_cachedAssetOperationHandles.Add(handle);
				GameObject go = handle.InstantiateSync(icon.transform);
				go.transform.localPosition = new Vector3(0, -50, -100);
				go.transform.localRotation = Quaternion.EulerAngles(0, 180, 0);
				go.transform.localScale = Vector3.one * 50;
			});
		}

		// 异步加载Unity官方生成的图集
		{
			var btn = CanvasRoot.transform.Find("unity_atlas/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				AssetOperationHandle handle = YooAssets.LoadAssetAsync<SpriteAtlas>("UIAtlas/unityAtlas");
				_cachedAssetOperationHandles.Add(handle);
				handle.Completed += OnUnityAtlas_Completed;
			});
		}

		// 异步加载TexturePacker生成的图集
		{
			var btn = CanvasRoot.transform.Find("tp_atlas/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				SubAssetsOperationHandle handle = YooAssets.LoadSubAssetsAsync<Sprite>("UIAtlas/tpAtlas");
				_cachedSubAssetsOperationHandles.Add(handle);
				handle.Completed += OnTpAtlasAsset_Completed;
			});
		}

		// 异步加载原生文件
		{
			var btn = CanvasRoot.transform.Find("config/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				string savePath = $"{YooAssets.GetSandboxRoot()}/config1.txt";
				RawFileOperation operation = YooAssets.GetRawFileAsync("Config/config1.txt", savePath);
				operation.Completed += OnRawFile_Completed;
			});
		}

		// 异步加载主场景
		{
			var btn = CanvasRoot.transform.Find("sceneBtn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				YooAssets.LoadSceneAsync("Scene/Game2");
			});
		}
	}

	private void OnUnityAtlas_Completed(AssetOperationHandle handle)
	{
		var icon = CanvasRoot.transform.Find("unity_atlas/icon").GetComponent<Image>();
		SpriteAtlas atlas = handle.AssetObject as SpriteAtlas;
		icon.sprite = atlas.GetSprite("Icon_Arrows_128");
	}
	private void OnTpAtlasAsset_Completed(SubAssetsOperationHandle handle)
	{
		var icon = CanvasRoot.transform.Find("tp_atlas/icon").GetComponent<Image>();
		icon.sprite = handle.GetSubAssetObject<Sprite>("Icon_Arrows_128");
	}
	private void OnRawFile_Completed(AsyncOperationBase operation)
	{
		var hint = CanvasRoot.transform.Find("config/icon/hint").GetComponent<Text>();
		RawFileOperation op = operation as RawFileOperation;
		hint.text = op.LoadFileText();
	}

	/// <summary>
	/// 异步编程方式1
	/// </summary>
	IEnumerator AsyncLoad1()
	{
		// 加载背景音乐
		{
			var audioSource = CanvasRoot.transform.Find("music").GetComponent<AudioSource>();
			AssetOperationHandle handle = YooAssets.LoadAssetAsync<AudioClip>("Music/town.mp3");
			_cachedAssetOperationHandles.Add(handle);
			yield return handle;
			audioSource.clip = handle.AssetObject as AudioClip;
			audioSource.Play();
		}
	}

	/// <summary>
	/// 异步编程方式2
	/// </summary>
	async void AsyncLoad2()
	{
		// 加载背景图片
		{
			var rawImage = CanvasRoot.transform.Find("texture").GetComponent<RawImage>();
			AssetOperationHandle handle = YooAssets.LoadAssetAsync<Texture>("Texture/bg2.jpeg");
			_cachedAssetOperationHandles.Add(handle);
			await handle.Task;
			rawImage.texture = handle.AssetObject as Texture;

			var op1 = YooAssets.GetRawFileAsync("Config/config3");
			await op1.Task;
			UnityEngine.Debug.LogWarning(op1.LoadFileText());

			var op2 = YooAssets.GetRawFileAsync("Config/config3");
			await op2.Task;
			UnityEngine.Debug.LogWarning(op2.LoadFileText());
		}
	}
}