using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Sieve.UI;

/* Unmerged change from project 'Sieve (net7.0)'
Added:
using GhPlugins;
using Sieve;
*/


namespace Sieve
{
    public class GhPluginsCommand : Command
    {
        public GhPluginsCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static GhPluginsCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "Sieve";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // RhinoApp.WriteLine("RunCommand reached successfully.");

            try
            {
                var dialog = new ModeManagerDialog();
                //RhinoApp.WriteLine("😊 Sieve.");
                dialog.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow);

            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine("ERROR in dialog: " + ex.Message);
            }

            return Result.Success;
        }
    }
}
