# Sieve – Grasshopper Environment Manager

Sieve lets you decide what Grasshopper loads **before** it opens.

Instead of launching Grasshopper with every plugin in your library, you launch it through Sieve and either:

- **Select plugins manually** for this session, or  
- **Choose a saved “environment”** – a preset list of plugins tailored to a specific workflow.

Grasshopper then starts with only those plugins.  
That means:

- ⚡ Faster startup times  
- 🧼 A cleaner canvas focused on the tools you actually need  
- 🧩 Fewer conflicts and version issues  
- 🤝 Easier team communication when you share environments instead of huge plugin lists  

Sieve is open-source and community-driven.

---

## Features

### 🔧 Environment presets
- Create and name multiple Grasshopper environments (e.g. _Urban Analysis_, _Robotic 3D Printing_, _Visualization_).
- Each environment stores a list of plugins to load.
- Quickly switch between environments depending on the task.

### ✅ Manual plugin selection
- Browse your installed Grasshopper plugins in a searchable list.
- Tick only what you need for a one-off session.
- Great for testing new setups without touching your main environment.

### 📂 Multiple plugin locations
- Scans typical Grasshopper plugin folders.
- Supports **custom paths** you add (useful for Yak packages, shared folders, custom libraries).
- Remembers scan results to avoid repeated slow scans.

### 💾 (Planned) Smart backups & file-based environments
Planned features for future versions:

- Backup environment definitions and plugin lists.
- Build an environment **from an existing Grasshopper document** (scan the file, detect required plugins, and create an environment for it).
- Drag & drop a `.gh` file onto Sieve to load only the plugins that document needs.

---

## Installation

> **Note:** adjust this section to match how you distribute the plugin (Yak / manual `.rhp` / Food4Rhino download).

### From release / Food4Rhino
1. Download the latest `.yak` or `.rhp` from the [Releases](../../releases) or Food4Rhino page.
2. For Yak:  
   - Install using `yak` or Rhino’s Package Manager.
3. For `.rhp`:  
   - In Rhino, open **_Tools → Options → Plug-ins_**.  
   - Click **Install…**, browse to the `.rhp` file, and load it.

## Contributing

First of all, thank you for your interest in contributing to **Sieve** 💚  
This project is meant to be community-driven, so feedback, ideas, and code contributions are very welcome.

---

### Ways you can help

- 🐞 **Report bugs** – strange behaviour, crashes, UI glitches, anything.
- 💡 **Suggest features / UX improvements** – new workflows, presets, or UI ideas.
- 🧪 **Test new builds** – especially on different Rhino / Grasshopper setups.
- 💻 **Contribute code** – fix issues, refactor, or implement items from the roadmap.
- 📚 **Improve documentation** – better explanations, screenshots, GIFs, examples.

---

### Before you start contributing:D

1. **Check the Issues**
   - Look for an existing issue that matches your bug / feature idea.
   - If there isn’t one, create a new issue with a clear description.

2. **Discuss first (optional but helpful)**
   - Comment on the issue with your plan or questions.
   - This avoids duplicate work and big mismatches in expectations.

---

### Setting up the development environment

1. **Clone the repo**

   ```bash
   git clone https://github.com/toolworkslab/sieve.git
   cd sieve

