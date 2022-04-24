using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using YooAsset;

public class Game2Scene : MonoBehaviour
{
	public GameObject CanvasRoot;
	private readonly List<AssetOperationHandle> _cachedAssetOperationHandles = new List<AssetOperationHandle>(1000);
	private SceneOperationHandle _subSceneHandle = null;

	void Start()
	{
		YooAssets.UnloadUnusedAssets();

		// 初始化窗口
		InitWindow();

		this.StartCoroutine(AsyncLoad());
	}
	void OnDestroy()
	{
		foreach (var handle in _cachedAssetOperationHandles)
		{
			handle.Release();
		}
		_cachedAssetOperationHandles.Clear();
	}
	void OnGUI()
	{
		GUIConsole.OnGUI();
	}

	void InitWindow()
	{
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

		// 异步加载主场景
		{
			var btn = CanvasRoot.transform.Find("sceneBtn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				YooAssets.LoadSceneAsync("Scene/Game1");
			});
		}

		// 异步加载子场景
		{
			var btn = CanvasRoot.transform.Find("subSceneLoadBtn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				_subSceneHandle = YooAssets.LoadSceneAsync("Scene/SubScene", UnityEngine.SceneManagement.LoadSceneMode.Additive);
			});
		}

		// 异步卸载子场景
		{
			var btn = CanvasRoot.transform.Find("subSceneUnloadBtn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				if(_subSceneHandle != null)
				{
					_subSceneHandle.UnloadAsync();
					_subSceneHandle = null;
				}				
			});
		}
	}

	/// <summary>
	/// 异步编程
	/// </summary>
	IEnumerator AsyncLoad()
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

		// 加载背景图片
		{
			var rawImage = CanvasRoot.transform.Find("texture").GetComponent<RawImage>();
			AssetOperationHandle handle = YooAssets.LoadAssetAsync<Texture>("Texture/bg3.jpeg");
			_cachedAssetOperationHandles.Add(handle);
			yield return handle;
			rawImage.texture = handle.AssetObject as Texture;
		}
	}
}