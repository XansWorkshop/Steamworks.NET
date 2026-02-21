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

using System.Runtime.InteropServices;
#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable CS1591 // Missing documentation

namespace Steamworks {
#if THE_CONSERVATORY
	[Star3D.Security.SecurityDeny(Star3D.Security.Capability.Patching)]
#endif
	public static class SteamAPI {
		//----------------------------------------------------------------------------------------------------------------------------------------------------------//
		//	Steam API setup & shutdown
		//
		//	These functions manage loading, initializing and shutdown of the steamclient.dll
		//
		//----------------------------------------------------------------------------------------------------------------------------------------------------------//


		// Initializing the Steamworks SDK
		// -----------------------------
		//
		// There are three different methods you can use to initialize the Steamworks SDK, depending on
		// your project's environment. You should only use one method in your project.
		//
		// If you are able to include this C++ header in your project, we recommend using the following
		// initialization methods. They will ensure that all ISteam* interfaces defined in other
		// C++ header files have versions that are supported by the user's Steam Client:
		// - SteamAPI_InitEx() for new projects so you can show a detailed error message to the user
		// - SteamAPI_Init() for existing projects that only display a generic error message
		//
		// If you are unable to include this C++ header in your project and are dynamically loading
		// Steamworks SDK methods from dll/so, you can use the following method:
		// - SteamAPI_InitFlat()


		// See "Initializing the Steamworks SDK" above for how to choose an init method.
		// On success k_ESteamAPIInitResult_OK is returned. Otherwise, returns a value that can be used
		// to create a localized error message for the user. If pOutErrMsg is non-NULL,
		// it will receive an example error message, in English, that explains the reason for the failure.
		//
		// Example usage:
		//
		//   SteamErrMsg errMsg;
		//   if ( SteamAPI_Init(&errMsg) != k_ESteamAPIInitResult_OK )
		//       FatalError( "Failed to init Steam.  %s", errMsg );

		// See "Initializing the Steamworks SDK" above for how to choose an init method.
		// Returns true on success
		public static ESteamAPIInitResult InitEx(out string OutSteamErrMsg)
		{
			InteropHelp.TestIfPlatformSupported();

			const char NULL = default;
			var pszInternalCheckInterfaceVersions = new System.Text.StringBuilder();
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMUTILS_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMNETWORKINGUTILS_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMAPPS_INTERFACE_VERSION).Append(NULL);
			//pszInternalCheckInterfaceVersions.Append(Constants.STEAMCONTROLLER_INTERFACE_VERSION).Append(NULL); // ISteamController is deprecated in favor of ISteamInput.
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMFRIENDS_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMHTMLSURFACE_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMHTTP_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMINPUT_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMINVENTORY_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMMATCHMAKINGSERVERS_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMMATCHMAKING_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMMUSIC_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMNETWORKINGMESSAGES_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMNETWORKINGSOCKETS_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMNETWORKING_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMPARENTALSETTINGS_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMPARTIES_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMREMOTEPLAY_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMREMOTESTORAGE_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMSCREENSHOTS_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMUGC_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMUSERSTATS_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMUSER_INTERFACE_VERSION).Append(NULL);
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMVIDEO_INTERFACE_VERSION).Append(NULL);

			using (var pszInternalCheckInterfaceVersions2 = new InteropHelp.UTF8StringHandle(pszInternalCheckInterfaceVersions.ToString())) {
				nint SteamErrorMsgPtr = Marshal.AllocHGlobal(Constants.k_cchMaxSteamErrMsg);
				ESteamAPIInitResult initResult = NativeMethods.SteamInternal_SteamAPI_Init(pszInternalCheckInterfaceVersions2, SteamErrorMsgPtr);
				OutSteamErrMsg = InteropHelp.PtrToStringUTF8(SteamErrorMsgPtr);
				Marshal.FreeHGlobal(SteamErrorMsgPtr);

				// Steamworks.NET specific: We initialize the SteamAPI Context like this for now, but we need to do it
				// every time that Unity reloads binaries, so we also check if the pointers are available and initialized
				// before each call to any interface functions. That is in InteropHelp.cs
				if (initResult == ESteamAPIInitResult.k_ESteamAPIInitResult_OK)
				{
					bool ret = CSteamAPIContext.Init();
					if (ret) {
						CallbackDispatcher.Initialize();
					}
					else {
						initResult = ESteamAPIInitResult.k_ESteamAPIInitResult_FailedGeneric;
						OutSteamErrMsg = "[Steamworks.NET] Failed to initialize CSteamAPIContext";
					}
				}

				return initResult;
			}
		}

		public static bool Init() {
			InteropHelp.TestIfPlatformSupported();

			string SteamErrorMsg;
			return InitEx(out SteamErrorMsg) == ESteamAPIInitResult.k_ESteamAPIInitResult_OK;
		}

		// SteamAPI_Shutdown should be called during process shutdown if possible.
		public static void Shutdown() {
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamAPI_Shutdown();
			CSteamAPIContext.Clear();
			CallbackDispatcher.Shutdown();
		}

		// SteamAPI_RestartAppIfNecessary ensures that your executable was launched through Steam.
		//
		// Returns true if the current process should terminate. Steam is now re-launching your application.
		//
		// Returns false if no action needs to be taken. This means that your executable was started through
		// the Steam client, or a steam_appid.txt file is present in your game's directory (for development).
		// Your current process should continue if false is returned.
		//
		// NOTE: If you use the Steam DRM wrapper on your primary executable file, this check is unnecessary
		// since the DRM wrapper will ensure that your application was launched properly through Steam.
		public static bool RestartAppIfNecessary(AppId_t unOwnAppID) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamAPI_RestartAppIfNecessary(unOwnAppID);
		}

		// Many Steam API functions allocate a small amount of thread-local memory for parameter storage.
		// SteamAPI_ReleaseCurrentThreadMemory() will free API memory associated with the calling thread.
		// This function is also called automatically by SteamAPI_RunCallbacks(), so a single-threaded
		// program never needs to explicitly call this function.
		public static void ReleaseCurrentThreadMemory() {
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamAPI_ReleaseCurrentThreadMemory();
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------//
		//	steam callback and call-result helpers
		//
		//	The following macros and classes are used to register your application for
		//	callbacks and call-results, which are delivered in a predictable manner.
		//
		//	STEAM_CALLBACK macros are meant for use inside of a C++ class definition.
		//	They map a Steam notification callback directly to a class member function
		//	which is automatically prototyped as "void func( callback_type *pParam )".
		//
		//	CCallResult is used with specific Steam APIs that return "result handles".
		//	The handle can be passed to a CCallResult object's Set function, along with
		//	an object pointer and member-function pointer. The member function will
		//	be executed once the results of the Steam API call are available.
		//
		//	CCallback and CCallbackManual classes can be used instead of STEAM_CALLBACK
		//	macros if you require finer control over registration and unregistration.
		//
		//	Callbacks and call-results are queued automatically and are only
		//	delivered/executed when your application calls SteamAPI_RunCallbacks().
		//
		//	Note that there is an alternative, lower level callback dispatch mechanism.
		//	See SteamAPI_ManualDispatch_Init
		//----------------------------------------------------------------------------------------------------------------------------------------------------------//

		// Dispatch all queued Steamworks callbacks.
		//
		// This is safe to call from multiple threads simultaneously,
		// but if you choose to do this, callback code could be executed on any thread.
		// One alternative is to call SteamAPI_RunCallbacks from the main thread only,
		// and call SteamAPI_ReleaseCurrentThreadMemory regularly on other threads.
		public static void RunCallbacks() {
			CallbackDispatcher.RunFrame(false);
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------//
		//	steamclient.dll private wrapper functions
		//
		//	The following functions are part of abstracting API access to the steamclient.dll, but should only be used in very specific cases
		//----------------------------------------------------------------------------------------------------------------------------------------------------------//

		// SteamAPI_IsSteamRunning() returns true if Steam is currently running
		public static bool IsSteamRunning() {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamAPI_IsSteamRunning();
		}

		// returns the pipe we are communicating to Steam with
		public static HSteamPipe GetHSteamPipe() {
			InteropHelp.TestIfPlatformSupported();
			return (HSteamPipe)NativeMethods.SteamAPI_GetHSteamPipe();
		}

		public static HSteamUser GetHSteamUser() {
			InteropHelp.TestIfPlatformSupported();
			return (HSteamUser)NativeMethods.SteamAPI_GetHSteamUser();
		}
	}

#if THE_CONSERVATORY
	[Star3D.Security.SecurityDeny(Star3D.Security.Capability.Patching)]
#endif
	public static class GameServer {
		// Initialize SteamGameServer client and interface objects, and set server properties which may not be changed.
		//
		// After calling this function, you should set any additional server parameters, and then
		// call ISteamGameServer::LogOnAnonymous() or ISteamGameServer::LogOn()
		//
		// - unIP will usually be zero.  If you are on a machine with multiple IP addresses, you can pass a non-zero
		//   value here and the relevant sockets will be bound to that IP.  This can be used to ensure that
		//   the IP you desire is the one used in the server browser.
		// - usGamePort is the port that clients will connect to for gameplay.  You will usually open up your
		//   own socket bound to this port.
		// - usQueryPort is the port that will manage server browser related duties and info
		//		pings from clients.  If you pass STEAMGAMESERVER_QUERY_PORT_SHARED for usQueryPort, then it
		//		will use "GameSocketShare" mode, which means that the game is responsible for sending and receiving
		//		UDP packets for the master  server updater.  (See ISteamGameServer::HandleIncomingPacket and
		//		ISteamGameServer::GetNextOutgoingPacket.)
		// - The version string should be in the form x.x.x.x, and is used by the master server to detect when the
		//		server is out of date.  (Only servers with the latest version will be listed.)
		public static ESteamAPIInitResult InitEx(uint unIP, ushort usGamePort, ushort usQueryPort, EServerMode eServerMode, string pchVersionString, out string OutSteamErrMsg) {
			InteropHelp.TestIfPlatformSupported();

			var pszInternalCheckInterfaceVersions = new System.Text.StringBuilder();
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMUTILS_INTERFACE_VERSION).Append('\0');
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMNETWORKINGUTILS_INTERFACE_VERSION).Append('\0');
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMGAMESERVER_INTERFACE_VERSION).Append('\0');
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMGAMESERVERSTATS_INTERFACE_VERSION).Append('\0');
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMHTTP_INTERFACE_VERSION).Append('\0');
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMINVENTORY_INTERFACE_VERSION).Append('\0');
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMNETWORKING_INTERFACE_VERSION).Append('\0');
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMNETWORKINGMESSAGES_INTERFACE_VERSION).Append('\0');
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMNETWORKINGSOCKETS_INTERFACE_VERSION).Append('\0');
			pszInternalCheckInterfaceVersions.Append(Constants.STEAMUGC_INTERFACE_VERSION).Append('\0');

			using (var pchVersionString2 = new InteropHelp.UTF8StringHandle(pchVersionString))
			using (var pszInternalCheckInterfaceVersions2 = new InteropHelp.UTF8StringHandle(pszInternalCheckInterfaceVersions.ToString())) {
				nint SteamErrorMsgPtr = Marshal.AllocHGlobal(Constants.k_cchMaxSteamErrMsg);
				ESteamAPIInitResult initResult = NativeMethods.SteamInternal_GameServer_Init_V2(unIP, usGamePort, usQueryPort, eServerMode, pchVersionString2, pszInternalCheckInterfaceVersions2, SteamErrorMsgPtr);
				OutSteamErrMsg = InteropHelp.PtrToStringUTF8(SteamErrorMsgPtr);
				Marshal.FreeHGlobal(SteamErrorMsgPtr);

				// Steamworks.NET specific: We initialize the SteamAPI Context like this for now, but we need to do it
				// every time that Unity reloads binaries, so we also check if the pointers are available and initialized
				// before each call to any interface functions. That is in InteropHelp.cs
				if (initResult == ESteamAPIInitResult.k_ESteamAPIInitResult_OK)
				{
					bool ret = CSteamGameServerAPIContext.Init();
					if (ret) {
						CallbackDispatcher.Initialize();
					}
					else {
						initResult = ESteamAPIInitResult.k_ESteamAPIInitResult_FailedGeneric;
						OutSteamErrMsg = "[Steamworks.NET] Failed to initialize CSteamAPIContext";
					}
				}

				return initResult;
			}
		}

		// This function is included for compatibility with older SDK.
		// You can use it if you don't care about decent error handling
		public static bool Init(uint unIP, ushort usGamePort, ushort usQueryPort, EServerMode eServerMode, string pchVersionString) {
			InteropHelp.TestIfPlatformSupported();

			string SteamErrorMsg;
			return InitEx(unIP, usGamePort, usQueryPort, eServerMode, pchVersionString, out SteamErrorMsg) == ESteamAPIInitResult.k_ESteamAPIInitResult_OK;
		}

		// Shutdown SteamGameSeverXxx interfaces, log out, and free resources.
		public static void Shutdown() {
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamGameServer_Shutdown();
			CSteamGameServerAPIContext.Clear();
			CallbackDispatcher.Shutdown();
		}

		public static void RunCallbacks() {
			CallbackDispatcher.RunFrame(true);
		}

		// Most Steam API functions allocate some amount of thread-local memory for
		// parameter storage. Calling SteamGameServer_ReleaseCurrentThreadMemory()
		// will free all API-related memory associated with the calling thread.
		// This memory is released automatically by SteamGameServer_RunCallbacks(),
		// so single-threaded servers do not need to explicitly call this function.
		public static void ReleaseCurrentThreadMemory() {
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamGameServer_ReleaseCurrentThreadMemory();
		}

		public static bool BSecure() {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamGameServer_BSecure();
		}

		public static CSteamID GetSteamID() {
			InteropHelp.TestIfPlatformSupported();
			return (CSteamID)NativeMethods.SteamGameServer_GetSteamID();
		}

		public static HSteamPipe GetHSteamPipe() {
			InteropHelp.TestIfPlatformSupported();
			return (HSteamPipe)NativeMethods.SteamGameServer_GetHSteamPipe();
		}

		public static HSteamUser GetHSteamUser() {
			InteropHelp.TestIfPlatformSupported();
			return (HSteamUser)NativeMethods.SteamGameServer_GetHSteamUser();
		}
	}

#if THE_CONSERVATORY
	[Star3D.Security.SecurityDeny(Star3D.Security.Capability.Patching)]
#endif
	public static class SteamEncryptedAppTicket {
		public static bool BDecryptTicket(byte[] rgubTicketEncrypted, uint cubTicketEncrypted, byte[] rgubTicketDecrypted, ref uint pcubTicketDecrypted, byte[] rgubKey, int cubKey) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_BDecryptTicket(rgubTicketEncrypted, cubTicketEncrypted, rgubTicketDecrypted, ref pcubTicketDecrypted, rgubKey, cubKey);
		}

		public static bool BIsTicketForApp(byte[] rgubTicketDecrypted, uint cubTicketDecrypted, AppId_t nAppID) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_BIsTicketForApp(rgubTicketDecrypted, cubTicketDecrypted, nAppID);
		}

		public static uint GetTicketIssueTime(byte[] rgubTicketDecrypted, uint cubTicketDecrypted) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_GetTicketIssueTime(rgubTicketDecrypted, cubTicketDecrypted);
		}

		public static void GetTicketSteamID(byte[] rgubTicketDecrypted, uint cubTicketDecrypted, out CSteamID psteamID) {
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamEncryptedAppTicket_GetTicketSteamID(rgubTicketDecrypted, cubTicketDecrypted, out psteamID);
		}

		public static uint GetTicketAppID(byte[] rgubTicketDecrypted, uint cubTicketDecrypted) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_GetTicketAppID(rgubTicketDecrypted, cubTicketDecrypted);
		}

		public static bool BUserOwnsAppInTicket(byte[] rgubTicketDecrypted, uint cubTicketDecrypted, AppId_t nAppID) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_BUserOwnsAppInTicket(rgubTicketDecrypted, cubTicketDecrypted, nAppID);
		}

		public static bool BUserIsVacBanned(byte[] rgubTicketDecrypted, uint cubTicketDecrypted) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_BUserIsVacBanned(rgubTicketDecrypted, cubTicketDecrypted);
		}

		public static byte[] GetUserVariableData(byte[] rgubTicketDecrypted, uint cubTicketDecrypted, out uint pcubUserData) {
			InteropHelp.TestIfPlatformSupported();
			nint punSecretData = NativeMethods.SteamEncryptedAppTicket_GetUserVariableData(rgubTicketDecrypted, cubTicketDecrypted, out pcubUserData);
			byte[] ret = new byte[pcubUserData];
			System.Runtime.InteropServices.Marshal.Copy(punSecretData, ret, 0, (int)pcubUserData);
			return ret;
		}

		public static bool BIsTicketSigned(byte[] rgubTicketDecrypted, uint cubTicketDecrypted, byte[] pubRSAKey, uint cubRSAKey) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_BIsTicketSigned(rgubTicketDecrypted, cubTicketDecrypted, pubRSAKey, cubRSAKey);
		}
	}

	internal static class CSteamAPIContext {
		internal static void Clear() {
			m_pSteamClient = 0;
			m_pSteamUser = 0;
			m_pSteamFriends = 0;
			m_pSteamUtils = 0;
			m_pSteamMatchmaking = 0;
			m_pSteamUserStats = 0;
			m_pSteamApps = 0;
			m_pSteamMatchmakingServers = 0;
			m_pSteamNetworking = 0;
			m_pSteamRemoteStorage = 0;
			m_pSteamHTTP = 0;
			m_pSteamScreenshots = 0;
			m_pSteamMusic = 0;
			m_pController = 0;
			m_pSteamUGC = 0;
			m_pSteamMusic = 0;
			m_pSteamHTMLSurface = 0;
			m_pSteamInventory = 0;
			m_pSteamVideo = 0;
			m_pSteamParentalSettings = 0;
			m_pSteamInput = 0;
			m_pSteamParties = 0;
			m_pSteamRemotePlay = 0;
			m_pSteamNetworkingUtils = 0;
			m_pSteamNetworkingSockets = 0;
			m_pSteamNetworkingMessages = 0;
			m_pSteamTimeline = 0;
		}

		internal static bool Init() {
			HSteamUser hSteamUser = SteamAPI.GetHSteamUser();
			HSteamPipe hSteamPipe = SteamAPI.GetHSteamPipe();
			if (hSteamPipe == (HSteamPipe)0) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMCLIENT_INTERFACE_VERSION)) {
				m_pSteamClient = NativeMethods.SteamInternal_CreateInterface(pchVersionString);
			}

			if (m_pSteamClient == 0) { return false; }

			m_pSteamUser = SteamClient.GetISteamUser(hSteamUser, hSteamPipe, Constants.STEAMUSER_INTERFACE_VERSION);
			if (m_pSteamUser == 0) { return false; }

			m_pSteamFriends = SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, Constants.STEAMFRIENDS_INTERFACE_VERSION);
			if (m_pSteamFriends == 0) { return false; }

			m_pSteamUtils = SteamClient.GetISteamUtils(hSteamPipe, Constants.STEAMUTILS_INTERFACE_VERSION);
			if (m_pSteamUtils == 0) { return false; }

			m_pSteamMatchmaking = SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, Constants.STEAMMATCHMAKING_INTERFACE_VERSION);
			if (m_pSteamMatchmaking == 0) { return false; }

			m_pSteamMatchmakingServers = SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, Constants.STEAMMATCHMAKINGSERVERS_INTERFACE_VERSION);
			if (m_pSteamMatchmakingServers == 0) { return false; }

			m_pSteamUserStats = SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, Constants.STEAMUSERSTATS_INTERFACE_VERSION);
			if (m_pSteamUserStats == 0) { return false; }

			m_pSteamApps = SteamClient.GetISteamApps(hSteamUser, hSteamPipe, Constants.STEAMAPPS_INTERFACE_VERSION);
			if (m_pSteamApps == 0) { return false; }

			m_pSteamNetworking = SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, Constants.STEAMNETWORKING_INTERFACE_VERSION);
			if (m_pSteamNetworking == 0) { return false; }

			m_pSteamRemoteStorage = SteamClient.GetISteamRemoteStorage(hSteamUser, hSteamPipe, Constants.STEAMREMOTESTORAGE_INTERFACE_VERSION);
			if (m_pSteamRemoteStorage == 0) { return false; }

			m_pSteamScreenshots = SteamClient.GetISteamScreenshots(hSteamUser, hSteamPipe, Constants.STEAMSCREENSHOTS_INTERFACE_VERSION);
			if (m_pSteamScreenshots == 0) { return false; }

			m_pSteamHTTP = SteamClient.GetISteamHTTP(hSteamUser, hSteamPipe, Constants.STEAMHTTP_INTERFACE_VERSION);
			if (m_pSteamHTTP == 0) { return false; }

			m_pSteamUGC = SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, Constants.STEAMUGC_INTERFACE_VERSION);
			if (m_pSteamUGC == 0) { return false; }

			m_pSteamMusic = SteamClient.GetISteamMusic(hSteamUser, hSteamPipe, Constants.STEAMMUSIC_INTERFACE_VERSION);
			if (m_pSteamMusic == 0) { return false; }

			m_pSteamHTMLSurface = SteamClient.GetISteamHTMLSurface(hSteamUser, hSteamPipe, Constants.STEAMHTMLSURFACE_INTERFACE_VERSION);
			if (m_pSteamHTMLSurface == 0) { return false; }

			m_pSteamInventory = SteamClient.GetISteamInventory(hSteamUser, hSteamPipe, Constants.STEAMINVENTORY_INTERFACE_VERSION);
			if (m_pSteamInventory == 0) { return false; }

			m_pSteamVideo = SteamClient.GetISteamVideo(hSteamUser, hSteamPipe, Constants.STEAMVIDEO_INTERFACE_VERSION);
			if (m_pSteamVideo == 0) { return false; }

			m_pSteamParentalSettings = SteamClient.GetISteamParentalSettings(hSteamUser, hSteamPipe, Constants.STEAMPARENTALSETTINGS_INTERFACE_VERSION);
			if (m_pSteamParentalSettings == 0) { return false; }

			m_pSteamInput = SteamClient.GetISteamInput(hSteamUser, hSteamPipe, Constants.STEAMINPUT_INTERFACE_VERSION);
			if (m_pSteamInput == 0) { return false; }

			m_pSteamParties = SteamClient.GetISteamParties(hSteamUser, hSteamPipe, Constants.STEAMPARTIES_INTERFACE_VERSION);
			if (m_pSteamParties == 0) { return false; }

			m_pSteamRemotePlay = SteamClient.GetISteamRemotePlay(hSteamUser, hSteamPipe, Constants.STEAMREMOTEPLAY_INTERFACE_VERSION);
			if (m_pSteamRemotePlay == 0) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGUTILS_INTERFACE_VERSION))
			{
				m_pSteamNetworkingUtils =
					NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString) != 0 ?
					NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString) :
					NativeMethods.SteamInternal_FindOrCreateGameServerInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingUtils == 0) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGSOCKETS_INTERFACE_VERSION))
			{
				m_pSteamNetworkingSockets = NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingSockets == 0) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGMESSAGES_INTERFACE_VERSION))
			{
				m_pSteamNetworkingMessages = NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingMessages == 0) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMTIMELINE_INTERFACE_VERSION))
			{
				m_pSteamTimeline = NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamTimeline == 0) { return false; }

			return true;
		}

		internal static nint GetSteamClient() { return m_pSteamClient; }
		internal static nint GetSteamUser() { return m_pSteamUser; }
		internal static nint GetSteamFriends() { return m_pSteamFriends; }
		internal static nint GetSteamUtils() { return m_pSteamUtils; }
		internal static nint GetSteamMatchmaking() { return m_pSteamMatchmaking; }
		internal static nint GetSteamUserStats() { return m_pSteamUserStats; }
		internal static nint GetSteamApps() { return m_pSteamApps; }
		internal static nint GetSteamMatchmakingServers() { return m_pSteamMatchmakingServers; }
		internal static nint GetSteamNetworking() { return m_pSteamNetworking; }
		internal static nint GetSteamRemoteStorage() { return m_pSteamRemoteStorage; }
		internal static nint GetSteamScreenshots() { return m_pSteamScreenshots; }
		internal static nint GetSteamHTTP() { return m_pSteamHTTP; }
		internal static nint GetSteamController() { return m_pController; }
		internal static nint GetSteamUGC() { return m_pSteamUGC; }
		internal static nint GetSteamMusic() { return m_pSteamMusic; }
		internal static nint GetSteamHTMLSurface() { return m_pSteamHTMLSurface; }
		internal static nint GetSteamInventory() { return m_pSteamInventory; }
		internal static nint GetSteamVideo() { return m_pSteamVideo; }
		internal static nint GetSteamParentalSettings() { return m_pSteamParentalSettings; }
		internal static nint GetSteamInput() { return m_pSteamInput; }
		internal static nint GetSteamParties() { return m_pSteamParties; }
		internal static nint GetSteamRemotePlay() { return m_pSteamRemotePlay; }
		internal static nint GetSteamNetworkingUtils() { return m_pSteamNetworkingUtils; }
		internal static nint GetSteamNetworkingSockets() { return m_pSteamNetworkingSockets; }
		internal static nint GetSteamNetworkingMessages() { return m_pSteamNetworkingMessages; }
		internal static nint GetSteamTimeline() { return m_pSteamTimeline; }

		private static nint m_pSteamClient;
		private static nint m_pSteamUser;
		private static nint m_pSteamFriends;
		private static nint m_pSteamUtils;
		private static nint m_pSteamMatchmaking;
		private static nint m_pSteamUserStats;
		private static nint m_pSteamApps;
		private static nint m_pSteamMatchmakingServers;
		private static nint m_pSteamNetworking;
		private static nint m_pSteamRemoteStorage;
		private static nint m_pSteamScreenshots;
		private static nint m_pSteamHTTP;
		private static nint m_pController;
		private static nint m_pSteamUGC;
		private static nint m_pSteamMusic;
		private static nint m_pSteamHTMLSurface;
		private static nint m_pSteamInventory;
		private static nint m_pSteamVideo;
		private static nint m_pSteamParentalSettings;
		private static nint m_pSteamInput;
		private static nint m_pSteamParties;
		private static nint m_pSteamRemotePlay;
		private static nint m_pSteamNetworkingUtils;
		private static nint m_pSteamNetworkingSockets;
		private static nint m_pSteamNetworkingMessages;
		private static nint m_pSteamTimeline;
	}

	internal static class CSteamGameServerAPIContext {
		internal static void Clear() {
			m_pSteamClient = 0;
			m_pSteamGameServer = 0;
			m_pSteamUtils = 0;
			m_pSteamNetworking = 0;
			m_pSteamGameServerStats = 0;
			m_pSteamHTTP = 0;
			m_pSteamInventory = 0;
			m_pSteamUGC = 0;
			m_pSteamNetworkingUtils = 0;
			m_pSteamNetworkingSockets = 0;
			m_pSteamNetworkingMessages = 0;
	}

		internal static bool Init() {
			HSteamUser hSteamUser = GameServer.GetHSteamUser();
			HSteamPipe hSteamPipe = GameServer.GetHSteamPipe();
			if (hSteamPipe == (HSteamPipe)0) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMCLIENT_INTERFACE_VERSION)) {
				m_pSteamClient = NativeMethods.SteamInternal_CreateInterface(pchVersionString);
			}
			if (m_pSteamClient == 0) { return false; }

			m_pSteamGameServer = SteamGameServerClient.GetISteamGameServer(hSteamUser, hSteamPipe, Constants.STEAMGAMESERVER_INTERFACE_VERSION);
			if (m_pSteamGameServer == 0) { return false; }

			m_pSteamUtils = SteamGameServerClient.GetISteamUtils(hSteamPipe, Constants.STEAMUTILS_INTERFACE_VERSION);
			if (m_pSteamUtils == 0) { return false; }

			m_pSteamNetworking = SteamGameServerClient.GetISteamNetworking(hSteamUser, hSteamPipe, Constants.STEAMNETWORKING_INTERFACE_VERSION);
			if (m_pSteamNetworking == 0) { return false; }

			m_pSteamGameServerStats = SteamGameServerClient.GetISteamGameServerStats(hSteamUser, hSteamPipe, Constants.STEAMGAMESERVERSTATS_INTERFACE_VERSION);
			if (m_pSteamGameServerStats == 0) { return false; }

			m_pSteamHTTP = SteamGameServerClient.GetISteamHTTP(hSteamUser, hSteamPipe, Constants.STEAMHTTP_INTERFACE_VERSION);
			if (m_pSteamHTTP == 0) { return false; }

			m_pSteamInventory = SteamGameServerClient.GetISteamInventory(hSteamUser, hSteamPipe, Constants.STEAMINVENTORY_INTERFACE_VERSION);
			if (m_pSteamInventory == 0) { return false; }

			m_pSteamUGC = SteamGameServerClient.GetISteamUGC(hSteamUser, hSteamPipe, Constants.STEAMUGC_INTERFACE_VERSION);
			if (m_pSteamUGC == 0) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGUTILS_INTERFACE_VERSION))
			{
				m_pSteamNetworkingUtils =
					NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString) != 0 ?
					NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString) :
					NativeMethods.SteamInternal_FindOrCreateGameServerInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingUtils == 0) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGSOCKETS_INTERFACE_VERSION))
			{
				m_pSteamNetworkingSockets =
					NativeMethods.SteamInternal_FindOrCreateGameServerInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingSockets == 0) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGMESSAGES_INTERFACE_VERSION))
			{
				m_pSteamNetworkingMessages =
					NativeMethods.SteamInternal_FindOrCreateGameServerInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingMessages == 0) { return false; }

			return true;
		}

		internal static nint GetSteamClient() { return m_pSteamClient; }
		internal static nint GetSteamGameServer() { return m_pSteamGameServer; }
		internal static nint GetSteamUtils() { return m_pSteamUtils; }
		internal static nint GetSteamNetworking() { return m_pSteamNetworking; }
		internal static nint GetSteamGameServerStats() { return m_pSteamGameServerStats; }
		internal static nint GetSteamHTTP() { return m_pSteamHTTP; }
		internal static nint GetSteamInventory() { return m_pSteamInventory; }
		internal static nint GetSteamUGC() { return m_pSteamUGC; }
		internal static nint GetSteamNetworkingUtils() { return m_pSteamNetworkingUtils; }
		internal static nint GetSteamNetworkingSockets() { return m_pSteamNetworkingSockets; }
		internal static nint GetSteamNetworkingMessages() { return m_pSteamNetworkingMessages; }

		private static nint m_pSteamClient;
		private static nint m_pSteamGameServer;
		private static nint m_pSteamUtils;
		private static nint m_pSteamNetworking;
		private static nint m_pSteamGameServerStats;
		private static nint m_pSteamHTTP;
		private static nint m_pSteamInventory;
		private static nint m_pSteamUGC;
		private static nint m_pSteamNetworkingUtils;
		private static nint m_pSteamNetworkingSockets;
		private static nint m_pSteamNetworkingMessages;
	}
}

#endif // !DISABLESTEAMWORKS
