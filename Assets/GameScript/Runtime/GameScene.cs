using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using YooAsset;

public class GameScene : MonoBehaviour
{
	public GameObject CanvasRoot;

	void Start()
	{
		// 同步编程方式
		SyncLoad();

		// 异步编程方式1
		AsyncLoad1();

		// 异步编程方式2
		this.StartCoroutine(AsyncLoad2());

		// 异步编程方式3
		AsyncLoad3();
	}
	void OnGUI()
	{
		GUIConsole.OnGUI();
	}

	/// <summary>
	/// 同步编程方式
	/// </summary>
	void SyncLoad()
	{
		// 加载预制体
		{
			var btn = CanvasRoot.transform.Find("entity/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				var icon = CanvasRoot.transform.Find("entity/icon").GetComponent<Image>();
				AssetOperationHandle handle = YooAssets.LoadAssetSync<GameObject>("Entity/Level1/footman_Blue");		
				GameObject go = handle.InstantiateSync(icon.transform);
				go.transform.localPosition = new Vector3(0, -50, -100);
				go.transform.localRotation = Quaternion.EulerAngles(0, 180, 0);
				go.transform.localScale = Vector3.one * 50;
			});
		}
	}

	/// <summary>
	/// 异步编程方式1
	/// </summary>
	void AsyncLoad1()
	{
		// 加载Unity官方生成的图集
		{
			var btn = CanvasRoot.transform.Find("unity_atlas/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				AssetOperationHandle handle = YooAssets.LoadAssetAsync<SpriteAtlas>("UIAtlas/unityAtlas");
				handle.Completed += OnUnityAtlas_Completed;
			});
		}

		// 加载TexturePacker生成的图集
		{
			var btn = CanvasRoot.transform.Find("tp_atlas/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				SubAssetsOperationHandle handle = YooAssets.LoadSubAssetsAsync<Sprite>("UIAtlas/tpAtlas");
				handle.Completed += OnTpAtlasAsset_Completed;
			});
		}

		// 加载原生文件
		{
			var btn = CanvasRoot.transform.Find("config/btn").GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				string savePath = $"{YooAssets.GetSandboxRoot()}/config1.txt";
				RawFileOperation operation = YooAssets.LoadRawFileAsync("Config/config1.txt", savePath);
				operation.Completed += OnRawFile_Completed;
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
		hint.text = op.GetFileText();
	}

	/// <summary>
	/// 异步编程方式2
	/// </summary>
	IEnumerator AsyncLoad2()
	{
		// 加载背景音乐
		{
			var audioSource = CanvasRoot.transform.Find("music").GetComponent<AudioSource>();
			AssetOperationHandle handle = YooAssets.LoadAssetAsync<AudioClip>("Music/town.mp3");
			yield return handle;
			audioSource.clip = handle.AssetObject as AudioClip;
			audioSource.Play();
		}
	}

	/// <summary>
	/// 异步编程方式3
	/// </summary>
	async void AsyncLoad3()
	{
		// 加载背景图片
		{
			var rawImage = CanvasRoot.transform.Find("texture").GetComponent<RawImage>();
			AssetOperationHandle handle = YooAssets.LoadAssetAsync<Texture>("Texture/bg2.jpeg");
			await handle.Task;
			rawImage.texture = handle.AssetObject as Texture;
		}
	}
}