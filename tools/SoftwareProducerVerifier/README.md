# SoftwareProducerVerifier

Small console tool that generates a PBKDF2 SoftwareProducer verifier string for deployment configuration.

## Format

`pbkdf2-sha256$<iterations>$<base64 salt>$<base64 hash>`

- HMAC-SHA256 via `Rfc2898DeriveBytes.Pbkdf2`
- Default iterations: 600000 (minimum accepted by the verify path)
- Salt: 16 random bytes; hash: 32 bytes

## Run (not in LogicPOS.sln)

From the repository root:

```bash
dotnet run --project tools/SoftwareProducerVerifier
```

The tool prompts for the password (no echo), asks for confirmation, and prints **only** the verifier string to stdout. Prompts go to stderr.

Do not pass the password on the command line. The tool does not write files.

## Configure the API

Set one of:

- `SoftwareProducer:Verifier` in configuration / `SoftwareProducer__Verifier` environment variable
- or `LOGICPOS_SOFTWARE_PRODUCER_VERIFIER` (used when the config value is empty)

Optional legacy migration:

- `SoftwareProducer:LegacyVerifier` — legacy `salt*digest` string from deployment only
- `SoftwareProducer:AcceptLegacy` — `true` to allow the legacy verifier as fallback (default `false`)
