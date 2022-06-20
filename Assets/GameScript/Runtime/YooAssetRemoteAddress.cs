using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
internal class YooAssetRemoteAddress : YooAsset.IRemoteHostServices
{
    private readonly string _remoteHost;
    private readonly string _falbackHost;
    private readonly string _gameVersion;
    public YooAssetRemoteAddress(string version, string remote, string fallback = null)
    {
        _gameVersion = version;
        _remoteHost = remote;
        _falbackHost = fallback;

        if (string.IsNullOrEmpty(_falbackHost) || string.IsNullOrWhiteSpace(_falbackHost))
        {
            _falbackHost = _remoteHost;
        }
    }

    public string GetDefaultHost()
    {
        return GetHostServerURL(_remoteHost);
    }

    public string GetFallbackHost()
    {
        return GetHostServerURL(_falbackHost);
    }


    private string GetHostServerURL(string host)
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{host}/CDN/Android/{_gameVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{host}/CDN/IPhone/{_gameVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            return $"{host}/CDN/WebGL/{_gameVersion}";
        else
            return $"{host}/CDN/PC/{_gameVersion}";
#else
		if (Application.platform == RuntimePlatform.Android)
			return $"{host}/CDN/Android/{_gameVersion}";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			return $"{host}/CDN/IPhone/{_gameVersion}";
		else if (Application.platform == RuntimePlatform.WebGLPlayer)
			return $"{host}/CDN/WebGL/{_gameVersion}";
		else
			return $"{host}/CDN/PC/{_gameVersion}";
#endif
    }
}