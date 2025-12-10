using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Rhino;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;


using Sieve.services;
using Sieve.Models;
using Sieve.UI;

namespace Sieve.UI
{
    public class ModeManagerDialog : Dialog
    {
        private Button createButton;
        private Button selectPluginsButton;
        private Button selectEnvironmentButton;
        private Button launchButton;

        private List<PluginItem> allPlugins = new List<PluginItem>();
        private ModeConfig selectedEnvironment;

        public ModeManagerDialog()
        {
            Title = "Sieve";
            ClientSize = new Size(730, 200);
            Resizable = false;

            var bigBtnSize = new Size(170, 68);
            var logoSize = new Size(60, 60);

            createButton = new Button
            {
                Text = "Create New\nEnvironment",
                BackgroundColor = Colors.HotPink,
                TextColor = Colors.Black,
                Font = new Font(SystemFont.Bold, 10),
                Size = bigBtnSize
            };
            selectPluginsButton = new Button
            {
                Text = "Select Plugins",
                BackgroundColor = Colors.CornflowerBlue,
                TextColor = Colors.Black,
                Font = new Font(SystemFont.Bold, 10),
                Size = bigBtnSize
            };
            selectEnvironmentButton = new Button
            {
                Text = "Select Environment",
                BackgroundColor = Colors.Gold,
                TextColor = Colors.Black,
                Font = new Font(SystemFont.Bold, 10),
                Size = bigBtnSize
            };
            launchButton = new Button
            {
                Text = "Launch Grasshopper",
                Enabled = false,
                Font = new Font(SystemFont.Bold, 16),
                MinimumSize = new Size(100, logoSize.Height)
            };

            createButton.Click += (s, e) => CreateEnvironment();
            selectPluginsButton.Click += (s, e) => ManualPluginSelection();
            selectEnvironmentButton.Click += (s, e) => SelectSavedEnvironment();
            launchButton.Click += (s, e) => LaunchGrasshopper();

            Control logoControl;
            var asm = Assembly.GetExecutingAssembly();

            Bitmap logoBitmap = null;
            using (var stream = asm.GetManifestResourceStream("Sieve.Resources.logo.png"))
            {
                if (stream != null)
                    logoBitmap = new Bitmap(stream);
            }

            if (logoBitmap != null)
            {
                logoControl = new ImageView { Image = logoBitmap, Size = logoSize };
            }
            else
            {
                logoControl = new Panel { Size = logoSize };
            }
;
            var topRow = new TableLayout
            {
                Padding = new Padding(24, 20, 24, 10),
                Spacing = new Size(24, 0),
                Rows =
                {
                    new TableRow(
                        new TableCell(new Panel(), true),
                        new TableCell(createButton),
                        new TableCell(selectPluginsButton),
                        new TableCell(selectEnvironmentButton),
                        new TableCell(new Panel(), true)
                    )
                }
            };

            var bottomStrip = new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Spacing = 24,
                Padding = new Padding(24, 0, 24, 24),
                Items =
                {
                    new StackLayoutItem(launchButton, true),
                    new StackLayoutItem(logoControl)
                }
            };
            var bottomHost = new Panel { Content = bottomStrip, MinimumSize = new Size(0, logoSize.Height) };

            var filler = new StackLayout();

            Content = new TableLayout
            {
                Rows =
                {
                    topRow,
                    new TableRow(new TableCell(filler, true)),
                    bottomHost
                }
            };
        }

        private void CreateEnvironment()
        {
            // 1) Make sure we have a plugin list
            if (PluginScanner.pluginItems == null || PluginScanner.pluginItems.Count == 0)
            {
                var loaded = Info.Tools.LoadScan();

                if (loaded != null && loaded.Count > 0)
                {
                    PluginScanner.pluginItems = loaded;
                }
                else
                {
                    // Nothing to load → perform fresh scan
                    PluginScanner.ScanDefaultPluginFolders(); // your actual scan
                    Info.Tools.SaveScan(PluginScanner.pluginItems);
                }
            }

            // Always keep allPlugins in sync with pluginItems
            allPlugins = PluginScanner.pluginItems;

            // 2) Show the selection dialog
            var checkForm = new CheckBoxForm(
                PluginScanner.pluginItems,
                startUnchecked: true,
                onRescan: () =>
                {
                    // Optional: rescan logic if you want it here too
                    PluginScanner.ScanDefaultPluginFolders();
                    Info.Tools.SaveScan(PluginScanner.pluginItems);

                    allPlugins = PluginScanner.pluginItems;
                    return PluginScanner.pluginItems.ToList();
                });

            var result = checkForm.ShowModal(this);
            if (result != DialogResult.Ok)
                return;

            // 3) Read selected plugins from pluginItems (NOT allPlugins, which might be empty)
            var selected = PluginScanner.pluginItems
                .Where(p => p.IsSelected)
                .ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show(this,
                    "No plugins selected. Environment was not created.",
                    "Sieve");
                return;
            }

            // 4) Ask for environment name
            string envName = InputBox("Name this environment:");
            if (string.IsNullOrWhiteSpace(envName))
                return;

            // 5) Save the environment
            var environments = ModeManager.LoadEnvironments();
            var newEnv = new ModeConfig(envName, selected);
            environments.Add(newEnv);
            ModeManager.SaveEnvironments(environments);

            selectedEnvironment = newEnv; // so Launch uses it directly
            launchButton.Enabled = true;

            RhinoApp.WriteLine("Environment '{0}' created with {1} plugins.", envName, selected.Count);
        }


        private void ManualPluginSelection()
        {
            if (PluginScanner.pluginItems == null || PluginScanner.pluginItems.Count == 0)
            {

                /* Unmerged change from project 'Sieve (net7.0)'
                Before:
                                var loaded = Info.Tools.LoadScan();
                After:
                                var loaded = Tools.LoadScan();
                */
                var loaded = Info.Tools.LoadScan();

                if (loaded != null && loaded.Count > 0)
                {
                    PluginScanner.pluginItems = loaded;
                }
                else
                {
                    // Nothing to load → perform fresh scan
                    PluginScanner.ScanDefaultPluginFolders(); // your real scan

                    // Save the new scan result

                    /* Unmerged change from project 'Sieve (net7.0)'
                    Before:
                                        Info.Tools.SaveScan(PluginScanner.pluginItems);
                                    }
                    After:
                                        Tools.SaveScan(PluginScanner.pluginItems);
                                    }
                    */
                    Info.Tools.SaveScan(PluginScanner.pluginItems);
                }

                allPlugins = PluginScanner.pluginItems;
            }

            var checkForm = new CheckBoxForm(
    PluginScanner.pluginItems,
    startUnchecked: true,
    onRescan: () =>
    {
        // No need to reset allPlugins here; focus on updating PluginScanner.pluginItems
        PluginScanner.ScanDefaultPluginFolders();

        // Save new result

        /* Unmerged change from project 'Sieve (net7.0)'
        Before:
                Info.Tools.SaveScan(PluginScanner.pluginItems);
        After:
                Tools.SaveScan(PluginScanner.pluginItems);
        */
        Info.Tools.SaveScan(PluginScanner.pluginItems);

        // IMPORTANT: return a *copy*, not the original list
        allPlugins = PluginScanner.pluginItems;
        return PluginScanner.pluginItems.ToList();
    });
            if (checkForm.ShowModal(this) == DialogResult.Ok)
            {
                selectedEnvironment = new ModeConfig(
                    "Manual",
                    PluginScanner.pluginItems.Where(p => p.IsSelected).ToList()
                );

                launchButton.Enabled = selectedEnvironment.Plugins != null
                                       && selectedEnvironment.Plugins.Count > 0;
            }
        }

        private void SelectSavedEnvironment()
        {
            var environments = ModeManager.LoadEnvironments();
            if (environments.Count == 0)
            {
                MessageBox.Show("No environments saved.");
                return;
            }

            var dialog = new EnvironmentSelectDialog(environments);
            var result = dialog.ShowModal(this);
            if (result == null)
                return; // cancelled

            if (result.IsDelete)
            {
                var toDelete = environments.FirstOrDefault(e => e.Name == result.SelectedName);
                if (toDelete != null)
                {
                    environments.Remove(toDelete);
                    ModeManager.SaveEnvironments(environments);

                    MessageBox.Show(this,
                        $"Environment '{result.SelectedName}' deleted.",
                        "Sieve");

                    if (selectedEnvironment != null && selectedEnvironment.Name == result.SelectedName)
                    {
                        selectedEnvironment = null;
                        launchButton.Enabled = false;
                    }
                }
                return;
            }

            // ---------- NORMAL SELECTION PATH ----------

            var env = environments.FirstOrDefault(e => e.Name == result.SelectedName);
            if (env == null)
                return;

            selectedEnvironment = env;

            // 1) Make sure allPlugins is populated
            if (allPlugins == null || allPlugins.Count == 0)
            {
                if (PluginScanner.pluginItems != null && PluginScanner.pluginItems.Count > 0)
                {
                    allPlugins = PluginScanner.pluginItems;
                }
                else
                {
                    var loaded = Info.Tools.LoadScan();
                    if (loaded != null && loaded.Count > 0)
                    {
                        PluginScanner.pluginItems = loaded;
                        allPlugins = loaded;
                    }
                    else
                    {
                        // last fallback: scan now (same behavior as Manual selection / Create)
                        PluginScanner.ScanDefaultPluginFolders();
                        Info.Tools.SaveScan(PluginScanner.pluginItems);
                        allPlugins = PluginScanner.pluginItems;
                    }
                }
            }

            // 2) Map environment plugins → IsSelected flags on allPlugins
            var selectedNames = new HashSet<string>(
                env.Plugins.Select(p => p.Name),
                StringComparer.OrdinalIgnoreCase);

            foreach (var p in allPlugins)
            {
                if (p == null) continue;
                p.IsSelected = selectedNames.Contains(p.Name);
            }

            launchButton.Enabled = selectedEnvironment.Plugins != null &&
                                   selectedEnvironment.Plugins.Count > 0;
        }


        public void LaunchGrasshopper()
        {
            var env = selectedEnvironment ?? new ModeConfig("Manual", allPlugins.Where(p => p.IsSelected).ToList());

            // GhPluginBlocker.applyPluginDisable(allPlugins, env);
            GhPluginBlocker.ApplyBlocking(allPlugins);
            ScanReport.Save(allPlugins);

            try
            {
                RhinoApp.WriteLine("[Sieve] Environment applied.");
                RhinoApp.Idle += LaunchGrasshopperOnIdle;
                Close();
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine("ERROR in LaunchGrasshopper: " + ex);
                MessageBox.Show(this, "Failed to launch Grasshopper. See Rhino command line for details.", "Mode Manager");
            }
        }

        private void LaunchGrasshopperOnIdle(object sender, EventArgs e)
        {
            RhinoApp.Idle -= LaunchGrasshopperOnIdle;

            try
            {
                RhinoApp.RunScript("-_Grasshopper _Load _Enter", false);

                dynamic gh = null;
                try { gh = RhinoApp.GetPlugInObject("Grasshopper"); } catch { }

                bool editorLoaded = false;
                if (gh != null)
                {
                    try { editorLoaded = gh.IsEditorLoaded(); } catch { }
                    if (!editorLoaded)
                    {
                        try { gh.LoadEditor(); } catch { }
                    }
                }

                var t = new UITimer { Interval = 0.60 };
                t.Elapsed += (s2, e2) =>
                {
                    t.Stop();
                    try { gh?.ShowEditor(true); } catch { }
                    RhinoApp.RunScript("-_Grasshopper _Editor _Enter", false);
                    RhinoApp.RunScript("_Grasshopper", false);
                };
                t.Start();
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine("ERROR launching Grasshopper: " + ex);
            }
        }

        private string InputBox(string message)
        {
            var prompt = new Dialog<string>
            {
                Title = message,
                ClientSize = new Size(200, 140),
                Resizable = false
            };

            var input = new TextBox
            {
                Width = 200   // fixed width so centering is visible
            };

            var ok = new Button { Text = "OK" };
            
            string result = null;

            ok.Click += (s, e) => { result = input.Text; prompt.Close(); };
            

            prompt.Content = new StackLayout
            {
                Padding = new Padding(10),
                Spacing = 8,
                HorizontalContentAlignment = HorizontalAlignment.Center, // << center children
                Items =
        {
            new Label
            {
                Text = message,
                TextAlignment = TextAlignment.Center
            },
            input,
            new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Spacing = 10,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Items = { ok }
            }
        }
            };

            prompt.ShowModal(this);
            return result;
        }

    }
}
