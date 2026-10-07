# Security policy

Report suspected security vulnerabilities privately using GitHub's **Report a vulnerability** option when available. Otherwise, contact the repository owner privately before posting details publicly. Include the affected version, runtime, relevant reconstruction options, a minimal reproducible graph/payload, and the observed impact. Do not include credentials or private application data.

Use the latest release for fixes. No independent long-term maintenance promise is made for earlier releases.

Know the source graph and the destination types your application permits. Constructors, getters, custom handlers, and activation hooks are application code. The [configuration guide](docs/configuration-and-reconstruction.md) documents controls for special stored-type activation and reconstruction behavior. Inheto does not supply an authenticated transport or encryption envelope; those policies belong to the caller.
