﻿namespace Henke37.Win32.Tokens {
	internal enum TokenInformationClass {
		User=1,
		Groups,
		Privileges,
		Owner,
		PrimaryGroup,
		DefaultDacl,
		Source,
		Type,
		ImpersonationLevel,
		Statistics,
		RestrictedSids,
		SessionId,
		GroupsAndPrivileges,
		SessionReference,
		SandBoxInert,
		AuditPolicy,
		Origin,
		ElevationType,
		LinkedToken,
		Elevation,
		HasRestrictions,
		AccessInformation,
		VirtualizationAllowed,
		VirtualizationEnabled,
		IntegrityLevel,
		UIAccess,
		MandatoryPolicy,
		LogonSid,
		IsAppContainer,
		Capabilities,
		AppContainerSid,
		AppContainerNumber,
		UserClaimAttributes,
		DeviceClaimAttributes,
		RestrictedUserClaimAttributes,
		RestrictedDeviceClaimAttributes,
		DeviceGroups,
		RestrictedDeviceGroups,
		SecurityAttributes,
		IsRestricted,
		ProcessTrustLevel,
		PrivateNameSpace,
		SingletonAttributes,
		BnoIsolation,
		ChildProcessFlags,
		MaxTokenInfoClass,
		IsLessPrivilegedAppContainer,
		IsSandboxed,
		OriginatingProcessTrustLevel
	}
}