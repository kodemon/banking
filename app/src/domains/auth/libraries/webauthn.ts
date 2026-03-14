/*
 |--------------------------------------------------------------------------------
 | WebAuthn Utilities
 |--------------------------------------------------------------------------------
 |
 | Conversion helpers between the base64url strings Zitadel uses in its JSON API
 | and the ArrayBuffers the browser's WebAuthn API requires.
 |
 */

export function isPublicKeyCredential(credential: Credential | null): credential is PublicKeyCredential {
  return credential !== null && credential.type === "public-key" && "rawId" in credential && "response" in credential;
}

export function base64urlToBuffer(base64url: string): ArrayBuffer {
  const base64 = base64url.replace(/-/g, "+").replace(/_/g, "/");
  const binary = atob(base64);
  const buffer = new Uint8Array(binary.length);
  for (let i = 0; i < binary.length; i++) buffer[i] = binary.charCodeAt(i);
  return buffer.buffer;
}

export function bufferToBase64url(buffer: ArrayBuffer): string {
  return btoa(String.fromCharCode(...new Uint8Array(buffer)))
    .replace(/\+/g, "-")
    .replace(/\//g, "_")
    .replace(/=/g, "");
}

export function prepareCreationOptions(options: Record<string, unknown>): PublicKeyCredentialCreationOptions {
  const pub = { ...options } as Record<string, unknown>;

  if (typeof pub.challenge === "string") {
    pub.challenge = base64urlToBuffer(pub.challenge);
  }

  if (pub.user && typeof (pub.user as Record<string, unknown>).id === "string") {
    pub.user = {
      ...(pub.user as Record<string, unknown>),
      id: base64urlToBuffer((pub.user as Record<string, unknown>).id as string),
    };
  }

  if (Array.isArray(pub.excludeCredentials)) {
    pub.excludeCredentials = pub.excludeCredentials.map((c: Record<string, unknown>) => ({
      ...c,
      id: typeof c.id === "string" ? base64urlToBuffer(c.id) : c.id,
    }));
  }

  return pub as unknown as PublicKeyCredentialCreationOptions;
}

export function prepareRequestOptions(options: Record<string, unknown>): PublicKeyCredentialRequestOptions {
  const pub = { ...options } as Record<string, unknown>;

  if (typeof pub.challenge === "string") {
    pub.challenge = base64urlToBuffer(pub.challenge);
  }

  if (Array.isArray(pub.allowCredentials)) {
    pub.allowCredentials = pub.allowCredentials.map((c: Record<string, unknown>) => ({
      ...c,
      id: typeof c.id === "string" ? base64urlToBuffer(c.id) : c.id,
    }));
  }

  return pub as unknown as PublicKeyCredentialRequestOptions;
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
