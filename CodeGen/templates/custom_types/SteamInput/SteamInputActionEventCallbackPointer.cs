namespace Steamworks {
	[System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
	public delegate void SteamInputActionEventCallbackPointer(nint /* SteamInputActionEvent_t* */ SteamInputActionEvent);
}

#endif // !DISABLESTEAMWORKS
