// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2022 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

// This file has been modified by Xan's Workshop for integration into Godot.

#if GODOT_WINDOWS
#define STEAMWORKS_WIN
#elif GODOT_LINUXBSD || GODOT_OSX
#define STEAMWORKS_LIN_OSX
#endif

#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS

#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable CS1591 // Missing documentation

namespace Steamworks {
#if THE_CONSERVATORY
	[Star3D.Security.SecurityDeny(Star3D.Security.Capability.Patching)]
#endif
	public static class Version {
		public const string SteamworksNETVersion = "2025.163.0";
		public const string SteamworksSDKVersion = "1.63";
		public const string SteamAPIDLLVersion = "10.24.16.75";
		public const int SteamAPIDLLSize = 274072;
		public const int SteamAPI64DLLSize = 317080;
	}
}

#endif // !DISABLESTEAMWORKS
