# Rubberduck 3.0  

Version 3.0 reinvents Rubberduck by separating not only the VBIDE add-in from the user interface, but also by moving the "brains" into a separate _language server_ process. The VBE becomes a mere debugger as a result, while the bulk of the code editing moves to the _Rubberduck Editor_.

- Website: [rubberduckvba.com](https://rubberduckvba.com)
- Repository: [Rubberduck3](https://github.com/rubberduck-vba/Rubberduck3)


## Thank You!  
 
 The initial release of Rubberduck 3.0 could not have happened without our users and supporters, who consistently show up in greater numbers whenever a new Rubberduck version is issued. **Thank you** for your continued support!  Rubberduck 3.x is unlocking every single IDE feature we ever dreamed to implement, and radically enhances the experience of _programming in VBA_ in this century, which was the entire idea all along.  

 We hope you enjoy using Rubberduck as much as we enjoy making it!

<a href='https://ko-fi.com/N4N2IWEIG' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://storage.ko-fi.com/cdn/kofi1.png?v=3' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>

Get the [Rubberduck Style Guide (Extended Edition)](https://ko-fi.com/s/d91bfd610c) if you haven't already - it's free!

---

# Release notes

Up until release, this section contains the in-progress release notes for the next release; upon release this note and any placeholder items should be removed for packaging, and the commented-out placeholder section should then be copied on top of the completed release.

For release note history, consult this file's history on GitHub.

---

# 🏷️ **NEXT RELEASE**: 2024-xx-xx Rubberduck 3.0 alpha1

### Initial alpha release

- Tag: [Rubberduck3.0-alpha1](https://github.com/rubberduck-vba/Rubberduck3/tags/Rubberduck3.0-alpha1)
- Pull request: [#1234](https://github.com/rubberduck-vba/Rubberduck3/pull/1234)


## 🧩 **Rubberduck Editor**

The **Rubberduck Editor** is a standalone WPF application that acts as a lightweight LSP server process for the VBIDE add-in and aims to fully replace the VBE in _edit mode_; having no debugger, _break mode_ is out of scope.

### 🤩 New Features
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Added...  

### 😎 Enhancements
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Changed...

### 🐛 Bugs Fixed
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Fixed... by changing...


## 🧩 **Language Server**

The **Rubberduck Language Server** is a LSP server for VBA that provides all the functionality to the *Rubberduck Editor* while consuming its own out-of-process resources.

### 🤩 New Features
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Added...  

### 😎 Enhancements
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Changed...

### 🐛 Bugs Fixed
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Fixed... by changing...

## 🧩 **Update Server**

Updating Rubberduck could be made to happen without even exiting the VBE by running a separate **Update Server** process that takes the version check feature under its mantle, but orchestrating an entire Rubberduck update is in-scope for this project.

### 🤩 New Features
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Added...  

### 😎 Enhancements
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Changed...

### 🐛 Bugs Fixed
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Fixed... by changing...

## 🧩 **VBIDE Add-In**

The **Rubberduck** VBIDE add-in is *much* smaller than its 2.x counterpart, and uses a fraction of the in-process resources, too. Most of the startup delay consists of .net loading and initializing. The 3.x add-in is only responsible for interacting with the VBE when needed, for example when synchronizing workspace files.

### 🤩 New Features
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Added...  

### 😎 Enhancements
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Changed...

### 🐛 Bugs Fixed
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Fixed... by changing...

## 🧩 **Miscellaneous**

Changes to other libraries, for example a COM server / type library, additions the extensibility API, or fixes in the internal shared API would be listed here.

### 🤩 New Features
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Added...  

### 😎 Enhancements
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Changed...

### 🐛 Bugs Fixed
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Fixed... by changing...

---
⭐ [Star us on GitHub!]((https://github.com/rubberduck-vba/Rubberduck3))  
> 🥒 **Long live the cucumber!**  
> Don't ask, just nod along.

<!-- 
# 🏷️ **NEXT RELEASE**: 2024-xx-xx Rubberduck 3.0 alpha1

- Tag: [Rubberduck3.0-alpha1](https://github.com/rubberduck-vba/Rubberduck3/tags/Rubberduck3.0-alpha1)
- Pull request: [#1234](https://github.com/rubberduck-vba/Rubberduck3/pull/1234)


## 🧩 **Rubberduck Editor**

The **Rubberduck Editor** is a standalone WPF application that acts as a lightweight LSP server process for the VBIDE add-in and aims to fully replace the VBE in _edit mode_; having no debugger, _break mode_ is out of scope.

### 🤩 New Features
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Added...  

### 😎 Enhancements
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Changed...

### 🪲 Fixed Bugs
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Fixed... by changing...


## 🧩 **Language Server**

The **Rubberduck Language Server** is a LSP server for VBA that provides all the functionality to the *Rubberduck Editor* while consuming its own out-of-process resources.

### 🤩 New Features
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Added...  

### 😎 Enhancements
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Changed...

### 🪲 Fixed Bugs
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Fixed... by changing...

## 🧩 **Update Server**

Updating Rubberduck could be made to happen without even exiting the VBE by running a separate **Update Server** process that takes the version check feature under its mantle, but orchestrating an entire Rubberduck update is in-scope for this project.

### 🤩 New Features
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Added...  

### 😎 Enhancements
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Changed...

### 🪲 Fixed Bugs
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Fixed... by changing...

## 🧩 **VBIDE Add-In**

The **Rubberduck** VBIDE add-in is *much* smaller than its 2.x counterpart, and uses a fraction of the in-process resources, too. Most of the startup delay consists of .net loading and initializing. The 3.x add-in is only responsible for interacting with the VBE when needed, for example when synchronizing workspace files.

### 🤩 New Features
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Added...  

### 😎 Enhancements
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Changed...

### 🪲 Fixed Bugs
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Fixed... by changing...

## 🧩 **Miscellaneous**

Changes to other libraries, for example a COM server / type library, additions the extensibility API, or fixes in the internal shared API would be listed here.

### 🤩 New Features
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Added...  

### 😎 Enhancements
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Changed...

### 🪲 Fixed Bugs
- [#1234](https://github.com/rubberduck-vba/Rubberduck3/issues/1234) **Title** (author)  
  Fixed... by changing...

---
[⭐ Star us on GitHub!]((https://github.com/rubberduck-vba/Rubberduck3))  
> 🥒 **Long live the cucumber!**  
> Don't ask, just nod along.
>