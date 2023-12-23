# Rubberduck3

> **This project contains code inspired by or directly taken from the [Rubberduck](https://GitHub.com/rubberduck-vba/Rubberduck) repository.**

**All current and former contributors** of the Rubberduck project are deemed to have contributed to this repository. The commit history builds on top of everything that came before it.

There's a giant ball than needs to get rolling, and this repository kicks it off. Let's plan this with a project to track a backlog, set up a roadmap, and make it happen.

Because Rubberduck is using IoC and DI, it could have been technically possible to surgically replace a service with a new implementation and/or to incrementally refactor the rather large code base into what we need Rubberduck 3.0 to be... but then achieving this seems to be a lot more effort than (gasp) writing Rubberduck 3.0 from a clean slate, essentially from scratch.

We start with a bare-bones VBIDE add-in that spawns a splash screen, proceeds to initialize things, and starts up the `Application`, creates a menu with commands - showing the About dialog, and the _Rubberduck Editor_, which spawns a dockable toolwindow that contains a (bare-bones) XAML document (AvalonEdit).

We have the Rubberduck.VBEditor assemblies, but by implementing our own editor we get to finally be in charge and ignore every single limitation of the VBE; we get to just _make_ Rubberduck into the tool we always wanted it to be.

---

Project started from scratch with VS2022, successfully tested with a Microsoft Excel 365 host on Windows 11.
