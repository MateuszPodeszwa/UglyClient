# Security Policy

## Supported Versions

This project is currently in active development. Security updates will be provided for the following versions:

| Version | Supported          |
| ------- | ------------------ |
| main    | :white_check_mark: |
| < 1.0   | :x:                |

## Security Considerations

### API Key Management

BeautifulClient uses **.NET User Secrets** for sensitive configuration:

- **API keys** are stored in User Secrets, **not** in `appsettings.json`
- Secrets are never committed to source control
- User Secrets path: `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json`

**Setting an API key**:

```bash
dotnet user-secrets set "ApiSettings:ApiKey" "your-api-key-here" --project BeautifulClient
```

### Configuration Security

- `appsettings.json` contains non-sensitive configuration only
- Sensitive values (API keys, connection strings) use User Secrets or environment variables
- Production deployments should use secure secret management (Azure Key Vault, AWS Secrets Manager, etc.)

### HTTP Communication

- All HTTP communication with remote APIs should use **HTTPS**
- The `RemoteAdapter` is configured to use secure endpoints
- Polly retry policies include exponential backoff to prevent DDoS-like behavior

### Dependency Security

I regularly update dependencies to patch known vulnerabilities. To check for vulnerable packages:

```bash
# Check for known vulnerabilities
dotnet list package --vulnerable

# Update to latest stable versions
dotnet outdated
```

### Local Simulation Mode

When using `LocalAdapter` (hardware simulation), no external network calls are made. This provides:

- **Offline capability** — No exposure to network-based attacks
- **Development safety** — No risk of accidentally hitting production APIs
- **Testing isolation** — Reproducible behavior without external dependencies

## Reporting a Vulnerability

I take security seriously. If you discover a security vulnerability, please follow these steps:

### 1. **Do Not** Open a Public Issue

Public disclosure of security vulnerabilities can put users at risk. Please report vulnerabilities privately.

### 2. Report Via Email (or GitHub Security Advisory)

**Preferred**: Use [GitHub Security Advisories](https://github.com/MateuszPodeszwa/UglyClient/security/advisories/new) to report privately.

**Alternative**: Email me directly with:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if known)

### 3. Response Timeline

- **Acknowledgment**: Within 48 hours
- **Initial assessment**: Within 5 business days
- **Fix and disclosure**: Coordinated timeline (typically 30-90 days)

### 4. Coordinated Disclosure

I follow **responsible disclosure** principles:

1. I'll work with you to understand and validate the vulnerability
2. I'll develop a fix and release schedule
3. I'll credit you in the security advisory (unless you prefer anonymity)
4. I'll publish a security advisory once a fix is available

## Security Best Practices for Contributors

When contributing to BeautifulClient:

### Code Review Checklist

- [ ] No hardcoded secrets (API keys, passwords, tokens)
- [ ] User input is validated before use
- [ ] External data is sanitized before rendering in TUI
- [ ] Exceptions don't leak sensitive information
- [ ] Network calls use HTTPS
- [ ] File operations validate paths (prevent directory traversal)

### Sensitive Data Handling

- **Never log sensitive data** (API keys, user credentials, PII)
- Use Serilog's `Destructure.ByTransforming()` to redact sensitive properties
- Ensure log files have appropriate file permissions

### Dependency Management

- Keep dependencies up to date
- Review security advisories for used packages
- Use `dotnet list package --vulnerable` before major releases
- Avoid packages with known vulnerabilities or unmaintained status

### Testing Security-Relevant Code

- Test error paths (ensure sensitive data isn't exposed in error messages)
- Test input validation with malicious inputs
- Test authentication/authorization logic thoroughly

## Known Limitations

### Current Security Boundaries

1. **API Key Storage**: User Secrets are **not encrypted** on disk (they're just stored outside source control). For production scenarios, use proper secret management.

2. **TUI Input**: Spectre.Console handles user input, but custom command parsing should validate/sanitize inputs.

3. **HTTP Client**: Polly retry policies can amplify traffic in failure scenarios. Monitor for unintended behavior.

4. **Local Simulation**: `HardwarePlugService` is for development only. Do not use in production without proper access controls.

## Security Updates

Security patches will be released as soon as possible after verification. Subscribe to:

- **GitHub Security Advisories** for this repository
- **GitHub Watch** (Releases only) for update notifications

## Attribution

I appreciate responsible disclosure and will publicly thank researchers who report vulnerabilities (unless they prefer anonymity).

---

**Last updated**: 2026-04-06

For questions about this security policy, please open a discussion or contact me via GitHub.
