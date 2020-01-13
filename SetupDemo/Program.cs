using System;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Forms;
using WixSharp.CommonTasks;
 


// NuGet console: Install-Package WixSharp
// NuGet Manager UI: browse tab
//https://csharp.hotexamples.com/examples/WixSharp/Feature/-/php-feature-class-examples.html
namespace SetupDemo
{
    class Program
    {
        static void Main()
        {
          
            var project = new ManagedProject("QCTrader",
                             new Dir(@"%ProgramFiles%\QiCheng\QCTrader",
                             new Dir("Resources",new Files(@"source\resources\*.*")),
                             new Files(@"source\*.dll"),
                             new Files(@"source\*.exe")));
            project.SetNetFxPrerequisite(Condition.Net461_Installed,"请先安装.net 4.6.1运行时");
            project.UI = WUI.WixUI_Minimal;
            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
            project.LicenceFile = @"licenses\licence.rtf"; //注意必须时rtf格式文件
 
            
            project.ControlPanelInfo.ProductIcon = "logo.ico";
            project.ControlPanelInfo.Comments = "期货交易客户端";
            

            project.ManagedUI = ManagedUI.Empty;     
       
 

            project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
                                           .Add(Dialogs.Licence)
                                          // .Add(Dialogs.SetupType)
                                         //  .Add(Dialogs.Features)
                                           .Add(Dialogs.InstallDir)
                                           .Add(Dialogs.Progress)
                                           .Add(Dialogs.Exit);

            project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
                                          // .Add(Dialogs.Features)
                                           .Add(Dialogs.Progress)
                                           .Add(Dialogs.Exit);

            project.Load += Msi_Load;
            project.BeforeInstall += Msi_BeforeInstall;
            project.AfterInstall += Msi_AfterInstall;
            
            project.LocalizationFile = @"language\qctrader.zh-cn.wxl";
            project.Language = "zh-cn";
            
          //  project.SourceBaseDir = "<input dir path>";
         //   project.OutDir = "<output dir path>";

            project.BuildMsi();
       
        }

        static void Msi_Load(SetupEventArgs e)
        {
             if (!e.IsUISupressed && !e.IsUninstalling)
        
                MessageBox.Show(e.ToString(), "Load");
        }

        static void Msi_BeforeInstall(SetupEventArgs e)
        {
             if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "BeforeInstall");
        }

        static void Msi_AfterInstall(SetupEventArgs e)
        {
            if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "AfterExecute");
        }
    }
}