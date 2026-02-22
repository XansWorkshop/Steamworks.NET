// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2022 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

// This file has been modified by Xan's Workshop for integration into Godot.

#nullable disable

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
	internal class CallbackIdentities {
		internal static int GetCallbackIdentity(System.Type callbackStruct) {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
			foreach (CallbackIdentityAttribute attribute in callbackStruct.GetCustomAttributes(typeof(CallbackIdentityAttribute), false)) {
				return attribute.Identity;
			}
#endif
			throw new System.Exception("Callback number not found for struct " + callbackStruct);
		}
	}

	[System.AttributeUsage(System.AttributeTargets.Struct, AllowMultiple = false)]
	internal class CallbackIdentityAttribute : System.Attribute {
		public int Identity { get; set; }
		public CallbackIdentityAttribute(int callbackNum) {
			Identity = callbackNum;
		}
	}
}

#endif // !DISABLESTEAMWORKS
