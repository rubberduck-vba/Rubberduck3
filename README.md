### About Rubberduck3

> **This project contains code inspired by or directly taken from the [Rubberduck](https://GitHub.com/rubberduck-vba/Rubberduck) repository.**

**All current and former contributors** of the Rubberduck project are deemed to have contributed to this repository. The commit history builds on top of everything that came before it and, as such, the license remains the same GPLv3.

Long story short, Rubberduck is undergoing an almost-complete rewrite. Key points:

 - We're implementing a _language server_ (LSP) for VBA
 - We're making our own standalone editor / LSP client
 - We're writing all of it with the latest version of C# and .net
 - No, we're not going to be compiling VBA executables (this unfortunately means no debugger and designer features)... not now anyway.
 - Yes, we're still integrating with the VBE - but we're breaking free from it nonetheless
 
---

# Rubberduck 3.0

<p align="center">
 <img src="https://github.com/rubberduck-vba/Rubberduck3/assets/5751684/306d2399-f086-46c8-91d9-1997183ab495" alt="RD3 'outline ducky' logo" />
</p>
<p align="center">
 Next Release: 3.0a (TBD)
</p>

## RD3 Project News

Other than outright (forking and) cloning the repository and keeping a local build up-to-date, there are several ways to keep up with the project:

 - [rubberduckvba.blog](https://rubberduckvba.blog): the project's WordPress blog
 - Follow [@rubberduckvba](https://twitter.com/rubberduckvba) on Twitter/X
 - Support [rubberduckvba on Ko-fi](https://ko-fi.com/rubberduckvba) to unlock supporter-only mini-blog weekly-ish posts, sometimes with exclusive WIP screenshots. Ways to do this include:
   - Buy me a ko-fi (1+ CAD$), rhymes with "no fee" but the platform is awesome and someone _gifted_ me a ko-fi gold subscription, so I'm voluntarily forking 5% over to them.
   - Buy yourself a Rubberduck 10Y celebration mug or t-shirt (there'll be RD3 merch as well): I'm kinda stuck with all these mugs otherwise 😆
   - Get the [Rubberduck Style Guide](https://ko-fi.com/s/d91bfd610c) from the ko-fi shop for anything above $0.00 (or get it for free, but I'm not sure it counts)
 - [Join our Discord server](https://discord.gg/MYX9RECenJ) to chat with the devs

## Contributing

So you're outright forking and cloning the repository and keeping a local build up-to-date, that's awesome! There are a couple of things to know:

 - Rubberduck 3.0 is written in C# and targets the latest long-term service (LTS) .net version (currently 8.0), and builds with the latest version of Microsoft Visual Studio
 - Knowledge of the VBA language is not necessary, but it's pretty much the _domain model_ here so if you stick around you're going to end up learning it inside out anyway
 - We're all here to learn. Basic functional knowledge of C# and .net is of course fundamental, but by working on RD3 you're going to sharpen up your skills with asynchronous tasks and TPL Dataflow pipelines, XAML user interfaces, OmniSharp and the Language Server Protocol (LSP), and so many other concepts. Metaprogramming is a lot of fun, too!
 - The goal is for VBIDE+RD3 to have feature parity with VBIDE+v2.5.9x for an alpha 3.0 release, meaning:
   - [ ] all Rubberduck inspections have one or more equivalent RD3 diagnostics with equivalent or better configuration options
   - [ ] all quickfixes are available as code actions to the corresponding diagnostics
   - [ ] all refactoring actions are ported to v3
   - [ ] Project/Code Explorer is ported to v3
   - [ ] RD3 editor implements all VBIDE toolwindows (except debugger tools, because no debugger)
   - [ ] RD3 editor can do everything the VBIDE code panes can do (except breakpoints, because no debugger)
 - That's not the end of it: _lots_ of amazing good ideas will make v3.x and beyond very exciting to work on as well!

### Initial Setup

Unlike 2.x, the Rubberduck3 solution doesn't have a specific startup project. Which project we start/debug depends on what we're working on:

 - For integrating with the VBIDE, start the **Rubberduck.AddIn** library project by starting e.g. EXCEL.EXE. This requires additional setup to register the VBE add-in at the debug build location.
 - For working with the editor client, start the **Rubberduck.Editor** executable; the client will start a server process that can later be attached to debug client/server interactions.
 - To debug a server application like **Rubberduck.LanguageServer** or **Rubberduck.UpdateServer**, use command-line arguments to load and process a particular workspace.

Under `%LOCALAPPDATA%\Rubberduck`, create `\Workspaces` and `\Templates` folders; paste the contents of the repository's `\Templates` folder the empty `\Templates` folder you just created, and leave `\Workspaces` empty. Ultimately the workspaces and templates folders will probably end up being separated from the Rubberduck local install location and live somewhere more along the lines of "MyDocuments", but for now that's where it's at.

The VBIDE add-in in RD3 has limited but important functionality, that requires entries in the Windows Registry. Import `RegisterAddIn.reg` to set it all up (once): it creates a registry key that the VBE recognizes as an add-in to load on startup.
