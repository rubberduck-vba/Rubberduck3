# Rubberduck.Updater

This application runs minimized to the system tray as a RPC server for Rubberduck add-in clients.

Its purpose as a RPC server is to respond to _version check_ requests and send _update_ notifications for add-in clients to shutdown while Rubberduck is being updated.

As a standalone application, it serves as an installer that downloads and installs the latest version of Rubberduck.

## Installer

When the application cannot detect an existing Rubberduck installation, it prompts whether to install the optional <strong>Rubberduck.Server.Telemetry</strong> component, detects the correct bitness to use, downloads and validates the assets, and proceeds with the installation.

## Updater

When an existing Rubberduck installation is found, it looks up the latest release/pre-release tags (as configured) on GitHub and compares against the current/installed one.

When the updater is running minimized / as a background process:
 - If the latest version is currently running, an optional "You are running the latest version" showMessage can be sent to the add-in client.
 - If an update is available, a "Rubberduck will update on exit / install now?" prompt can be sent to the add-in client.

When the updater is running as a GUI application, no notifications are sent to add-in clients, even if they're opted into these notifications. Instead, the application displays a fitting user interface.

---

## Requests

This RPC server accepts the following requests:

### CheckForUpdates Request

The `checkForUpdates` request is sent from the client to the server to compare the local version against the latest Rubberduck release.

#### _Request_

- method: `updater/versionCheck`
- params: `VersionCheckParams` defined as follows:

```ts
export interface VersionCheckParams extends WorkDoneProgressParams {
	/**
	* The current client release version.
	*/
	current: string;
}
```

#### _Response_

- result: `VersionCheckResult | null` defined as follows:

```ts
export interface VersionCheckResult {
	/**
	* The tag name of the latest release.
	*/
	name: string;

	/**
	* Whether the tag is a pre-release.
	*/
	prerelease: bool;

	/**
	* Whether the client should be updated.
	*/
	update: bool;
}
```

## Notifications

This RPC server sends and accepts the following notifications:


### Update Notification

The `updateg` notification is sent from the client to the server, to indicate that the client wants to update Rubberduck.

#### _Notification_

- method: `$/update`
- params: `UpdateParams` defined as follows:

```ts
export interface UpdateParams extends WorkDoneProgressParams {
	/**
	* The tag name of the newer version.
	*/
	name: string;
}
```

### Updating Notification

The `updating` notification is sent from the server to the client, to indicate that the server is starting an update process and needs all clients to shutdown.

#### _Notification_

- method: `$/updating`
- params: `UpdatingParams` defined as follows:

```ts
export interface UpdatingParams extends WorkDoneProgressParams {
	/**
	* The tag name of the newer version being installed.
	*/
	name: string;
}
```
