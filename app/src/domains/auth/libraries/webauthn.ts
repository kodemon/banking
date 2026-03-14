/*
 |--------------------------------------------------------------------------------
 | WebAuthn Utilities
 |--------------------------------------------------------------------------------
 |
 | Conversion helpers between the base64url strings the API uses in its JSON
 | responses and the ArrayBuffers the browser's WebAuthn API requires.
 |
 | Firefox has a cross-compartment wrapper bug where ArrayBuffers derived from
 | JSON.parse'd objects cannot be passed to navigator.credentials.create/get.
 | All helpers here defensively copy buffers via Uint8Array.from() to ensure
 | they are fresh allocations in the current compartment.
 |
 */

export function isPublicKeyCredential(credential: Credential | null): credential is PublicKeyCredential {
  return credential !== null && credential.type === "public-key" && "rawId" in credential && "response" in credential;
}

export function base64urlToBuffer(base64url: string): ArrayBuffer {
  const base64 = base64url.replace(/-/g, "+").replace(/_/g, "/");
  const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), "=");
  const binary = atob(padded);
  // Uint8Array.from ensures a fresh ArrayBuffer in the current JS compartment,
  // avoiding Firefox's cross-compartment wrapper rejection.
  return Uint8Array.from(binary, (c) => c.charCodeAt(0)).buffer;
}

export function bufferToBase64url(buffer: ArrayBuffer): string {
  return btoa(String.fromCharCode(...new Uint8Array(buffer)))
    .replace(/\+/g, "-")
    .replace(/\//g, "_")
    .replace(/=/g, "");
}

export function prepareCreationOptions(options: Record<string, unknown>): PublicKeyCredentialCreationOptions {
  // Build a plain object from scratch rather than spreading the parsed JSON
  // object — spreading preserves the cross-compartment taint in Firefox.
  const user = options.user as Record<string, unknown>;

  return {
    challenge: base64urlToBuffer(options.challenge as string),
    rp: options.rp as PublicKeyCredentialRpEntity,
    user: {
      id: base64urlToBuffer(user.id as string),
      name: user.name as string,
      displayName: user.displayName as string,
    },
    pubKeyCredParams: options.pubKeyCredParams as PublicKeyCredentialParameters[],
    timeout: options.timeout as number | undefined,
    attestation: options.attestation as AttestationConveyancePreference | undefined,
    authenticatorSelection: options.authenticatorSelection as AuthenticatorSelectionCriteria | undefined,
    excludeCredentials: Array.isArray(options.excludeCredentials)
      ? (options.excludeCredentials as Record<string, unknown>[]).map((c) => ({
          type: c.type as PublicKeyCredentialType,
          id: base64urlToBuffer(c.id as string),
          transports: c.transports as AuthenticatorTransport[] | undefined,
        }))
      : [],
    extensions: options.extensions as AuthenticationExtensionsClientInputs | undefined,
  };
}

export function prepareRequestOptions(options: Record<string, unknown>): PublicKeyCredentialRequestOptions {
  return {
    challenge: base64urlToBuffer(options.challenge as string),
    timeout: options.timeout as number | undefined,
    rpId: options.rpId as string | undefined,
    userVerification: options.userVerification as UserVerificationRequirement | undefined,
    allowCredentials: Array.isArray(options.allowCredentials)
      ? (options.allowCredentials as Record<string, unknown>[]).map((c) => ({
          type: c.type as PublicKeyCredentialType,
          id: base64urlToBuffer(c.id as string),
          transports: c.transports as AuthenticatorTransport[] | undefined,
        }))
      : [],
    extensions: options.extensions as AuthenticationExtensionsClientInputs | undefined,
  };
}

export function serializeAttestation(credential: PublicKeyCredential): Record<string, unknown> {
  const response = credential.response as AuthenticatorAttestationResponse;
  return {
    type: credential.type,
    id: credential.id,
    rawId: bufferToBase64url(credential.rawId),
    response: {
      attestationObject: bufferToBase64url(response.attestationObject),
      clientDataJSON: bufferToBase64url(response.clientDataJSON),
    },
  };
}

export function serializeAssertion(credential: PublicKeyCredential): Record<string, unknown> {
  const response = credential.response as AuthenticatorAssertionResponse;
  return {
    type: credential.type,
    id: credential.id,
    rawId: bufferToBase64url(credential.rawId),
    response: {
      authenticatorData: bufferToBase64url(response.authenticatorData),
      clientDataJSON: bufferToBase64url(response.clientDataJSON),
      signature: bufferToBase64url(response.signature),
      userHandle: response.userHandle ? bufferToBase64url(response.userHandle) : null,
    },
  };
}

export function getPlatformName(): string {
  const ua = navigator.userAgent;
  if (/iPhone|iPad/.test(ua)) return "iPhone / iPad";
  if (/Mac/.test(ua)) return "Mac";
  if (/Android/.test(ua)) return "Android";
  if (/Windows/.test(ua)) return "Windows";
  return "Passkey";
}
