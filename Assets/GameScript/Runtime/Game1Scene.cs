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
		foreach (var handle in _cachedAssetOperationHandles)
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
		var resVersion = CanvasRoot.transform.Find("res_version/label").GetComponent<Text>();
		resVersion.text = $"资源版本 : {YooAssets.GetResourceVersion()}";

		var playMode = CanvasRoot.transform.Find("play_mode/label").GetComponent<Text>();
		if (BootScene.GamePlayMode == YooAssets.EPlayMode.EditorSimulateMode)
			playMode.text = "编辑器下模拟模式";
		else if (BootScene.GamePlayMode == YooAssets.EPlayMode.OfflinePlayMode)
			playMode.text = "离线运行模式";
		else if (BootScene.GamePlayMode == YooAssets.EPlayMode.HostPlayMode)
			playMode.text = "网络运行模式";
		else
			throw new NotImplementedException();

		// 同步加载预制体
		{
			var btn = CanvasRoot.transform.Find("load_npc/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				var icon = CanvasRoot.transform.Find("load_npc/icon").GetComponent<Image>();
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
			var btn = CanvasRoot.transform.Find("load_unity_atlas/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				AssetOperationHandle handle = YooAssets.LoadAssetAsync<SpriteAtlas>("UIAtlas/UnityPacker/unityAtlas");
				_cachedAssetOperationHandles.Add(handle);
				handle.Completed += OnUnityAtlas_Completed;
			});
		}

		// 异步加载TexturePacker生成的图集
		{
			var btn = CanvasRoot.transform.Find("load_tp_atlas/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				SubAssetsOperationHandle handle = YooAssets.LoadSubAssetsAsync<Sprite>("UIAtlas/TexturePacker/tpAtlas1");
				_cachedSubAssetsOperationHandles.Add(handle);
				handle.Completed += OnTpAtlasAsset_Completed;
			});
		}

		// 异步加载原生文件
		{
			var btn = CanvasRoot.transform.Find("load_rawfile/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				string savePath = $"{YooAssets.GetSandboxRoot()}/config1.txt";
				RawFileOperation operation = YooAssets.GetRawFileAsync("Config/config1.txt", savePath);
				operation.Completed += OnRawFile_Completed;
			});
		}

		// 异步加载主场景
		{
			var btn = CanvasRoot.transform.Find("load_scene").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				YooAssets.LoadSceneAsync("Scene/Game2.unity");
			});
		}
	}

	private void OnUnityAtlas_Completed(AssetOperationHandle handle)
	{
		var icon = CanvasRoot.transform.Find("load_unity_atlas/icon").GetComponent<Image>();
		SpriteAtlas atlas = handle.AssetObject as SpriteAtlas;
		icon.sprite = atlas.GetSprite("Icon_Arrows_128");
	}
	private void OnTpAtlasAsset_Completed(SubAssetsOperationHandle handle)
	{
		var icon = CanvasRoot.transform.Find("load_tp_atlas/icon").GetComponent<Image>();
		icon.sprite = handle.GetSubAssetObject<Sprite>("Icon_Arrows_128");
	}
	private void OnRawFile_Completed(AsyncOperationBase operation)
	{
		var hint = CanvasRoot.transform.Find("load_rawfile/icon/hint").GetComponent<Text>();
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

		// 通过资源标签加载资源
		LoadAssetsByTagOperation<GameObject> op = new LoadAssetsByTagOperation<GameObject>("sphere");
		YooAssets.ProcessOperaiton(op);
		yield return op;
		foreach (var assetObj in op.AssetObjects)
		{
			Debug.Log("LoadAssetsByTag : " + assetObj.name);
		}
	}

	/// <summary>
	/// 异步编程方式2
	/// </summary>
	async void AsyncLoad2()
	{
		// 加载背景图片
		{
			var rawImage = CanvasRoot.transform.Find("background").GetComponent<RawImage>();
			AssetOperationHandle handle = YooAssets.LoadAssetAsync<Texture>("Texture/bg2.jpeg");
			_cachedAssetOperationHandles.Add(handle);
			await handle.Task;
			rawImage.texture = handle.AssetObject as Texture;
		}
	}
}