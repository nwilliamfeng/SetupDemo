using System;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Forms;

  
// NuGet console: Install-Package WixSharp
// NuGet Manager UI: browse tab

namespace SetupDemo
{
    class Program
    {
        static void Main()
        {
            //https://csharp.hotexamples.com/examples/WixSharp/Feature/-/php-feature-class-examples.html
            var project = new ManagedProject("QCTrader",
                             new Dir(@"%ProgramFiles%\QiCheng\QCTrader",
                                 new File("Program.cs")));
            
            project.UI = WUI.WixUI_Minimal;
            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
            project.ControlPanelInfo.ProductIcon = "logo.ico";
            project.ControlPanelInfo.Manufacturer = "期程科技";
            project.ManagedUI = ManagedUI.Empty;    //no standard UI dialogs
            project.ManagedUI = ManagedUI.Default;  //all standard UI dialogs

            //custom set of standard UI dialogs
            project.ManagedUI = new ManagedUI();
            
            project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
                                            .Add(Dialogs.Licence)
                                          
                                            .Add(Dialogs.Features)
                                            .Add(Dialogs.InstallDir)
                                            .Add(Dialogs.Progress)
                                            .Add(Dialogs.Exit);

            project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
                                           .Add(Dialogs.Features)
                                           .Add(Dialogs.Progress)
                                           .Add(Dialogs.Exit);

            project.Load += Msi_Load;
            project.BeforeInstall += Msi_BeforeInstall;
            project.AfterInstall += Msi_AfterInstall;

            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";

            var msi =project.BuildMsi();
            project.Language = "zh-cn";
            string mstFile = project.BuildLanguageTransform(msi, "zh-cn");
        }

        static void Msi_Load(SetupEventArgs e)
        {
          //  if (!e.IsUISupressed && !e.IsUninstalling)
        
                MessageBox.Show(e.ToString(), "Load");
        }

        static void Msi_BeforeInstall(SetupEventArgs e)
        {
          //  if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "BeforeInstall");
        }

        static void Msi_AfterInstall(SetupEventArgs e)
        {
           // if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "AfterExecute");
        }
    }
}