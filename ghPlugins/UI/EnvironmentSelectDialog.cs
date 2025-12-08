using Eto.Drawing;
using Eto.Forms;
using Sieve.Models;
using System.Collections.Generic;
using System.Linq;

public class EnvironmentSelectDialog : Dialog<EnvironmentSelectDialog.Result>
{
    public class Result
    {
        public bool IsDelete { get; set; }
        public string SelectedName { get; set; }
    }

    private readonly IList<ModeConfig> _environments;
    private readonly ListBox envList;
    private readonly ListBox pluginList;
    private readonly Label pluginHeader;

    private readonly Button okButton;
    private readonly Button deleteButton;
    private readonly Button cancelButton;

    public EnvironmentSelectDialog(IList<ModeConfig> environments)
    {
        _environments = environments;

        Title = "Select an Environment";
        ClientSize = new Size(520, 300);
        Resizable = false;

        envList = new ListBox
        {
            DataStore = _environments.Select(e => e.Name).ToList(),
            Width = 180
        };

        pluginList = new ListBox();
        pluginHeader = new Label { Text = "Plugins:", Font = new Font(SystemFont.Bold, 9) };

        envList.SelectedIndexChanged += (s, e) => UpdatePluginPreview();

        okButton = new Button { Text = "OK" };
        deleteButton = new Button { Text = "Delete" };
        cancelButton = new Button { Text = "Cancel" };

        okButton.Click += (s, e) =>
        {
            var name = envList.SelectedValue as string;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show(this, "Please select an environment.", "Sieve");
                return;
            }

            Close(new Result
            {
                IsDelete = false,
                SelectedName = name
            });
        };

        deleteButton.Click += (s, e) =>
        {
            var name = envList.SelectedValue as string;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show(this, "Please select an environment to delete.", "Sieve");
                return;
            }

            var confirm = MessageBox.Show(
                this,
                $"Delete environment '{name}'?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxType.Warning);

            if (confirm == DialogResult.Yes)
            {
                Close(new Result
                {
                    IsDelete = true,
                    SelectedName = name
                });
            }
        };

        cancelButton.Click += (s, e) => Close(null);

        var rightSide = new StackLayout
        {
            Spacing = 6,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Items =
            {
                pluginHeader,
                new StackLayoutItem(pluginList, true)
            }
        };

        Content = new TableLayout
        {
            Padding = 10,
            Spacing = new Size(10, 10),
            Rows =
            {
                new TableRow(
                    new TableCell(envList, true),
                    new TableCell(rightSide, true)
                ),
                new TableRow(
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 10,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Items = { okButton, deleteButton, cancelButton }
                    }
                )
            }
        };

        if (_environments.Count > 0)
            envList.SelectedIndex = 0;
    }

    private void UpdatePluginPreview()
    {
        var idx = envList.SelectedIndex;
        if (idx < 0 || idx >= _environments.Count)
        {
            pluginList.DataStore = null;
            pluginHeader.Text = "Plugins:";
            return;
        }

        var env = _environments[idx];
        var plugins = env.Plugins ?? new List<PluginItem>();

        pluginHeader.Text = $"Plugins ({plugins.Count}):";
        pluginList.DataStore = plugins.Select(p => p.Name).ToList();
    }
}
