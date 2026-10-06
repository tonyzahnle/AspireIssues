# Aspire 13.6.0: hyphenated resource names emit a duplicate `ConnectionStrings__` env var

## Repro

```bash
aspire publish -o infra
```

Uses the single-file `apphost.cs` in this folder (Aspire CLI 13.6.0). No Azure credentials needed.

## Expected (13.5.3 behavior)

`infra/app/app.bicep` contains one connection string env var per referenced resource:

```bicep
{ name: 'ConnectionStrings__key-vault', value: key_vault_outputs_vaulturi }
{ name: 'KEY_VAULT_URI',                value: key_vault_outputs_vaulturi }
{ name: 'ConnectionStrings__vault',     value: vault_outputs_vaulturi }
{ name: 'VAULT_URI',                    value: vault_outputs_vaulturi }
```

## Actual (13.6.0)

For the resource with a hyphen in its name, an extra underscore-normalized copy is emitted:

```diff
             {
               name: 'ConnectionStrings__key-vault'
               value: key_vault_outputs_vaulturi
             }
+            {
+              name: 'ConnectionStrings__key_vault'
+              value: key_vault_outputs_vaulturi
+            }
             {
               name: 'KEY_VAULT_URI'
               value: key_vault_outputs_vaulturi
             }
```

The control resource `vault` (no hyphen) is unaffected.

## Notes

- To compare against 13.5.3, change the three `@13.6.0` directives in `apphost.cs` to `@13.5.3` and re-run `aspire publish -o infra`. The only difference in the output is the added `ConnectionStrings__key_vault` entry.
- Not specific to Key Vault: also observed with `AddAzureAppConfiguration("blackout-app-configuration")` (emits `ConnectionStrings__blackout_app_configuration`) and with project / Azure Functions project resources, not just containers.
- Deployment still succeeds; the issue is the unexpected, redundant env var in generated infra.
- `Directory.Build.props` / `Directory.Packages.props` in this folder only isolate the repro from the host repo's build settings and central package management. They're not needed if this folder is copied somewhere standalone.
