using System;
using System.Windows.Forms;
using WixSharp;
using System.Linq;
using WixSharp.Forms;
using WixSharp.CommonTasks;
using Microsoft.Deployment.WindowsInstaller;



// NuGet console: Install-Package WixSharp
// NuGet Manager UI: browse tab
//https://csharp.hotexamples.com/examples/WixSharp/Feature/-/php-feature-class-examples.html
namespace SetupDemo
{
    class Program
    {
        static void Main()
        {
            AutoElements.UACWarning = null;
             
            AutoElements.EnableUACRevealer = false;
           

            var projectName = "QCTrader";
            var company = "期程科技";
            var product = "期货交易";
            var dir = new System.IO.DirectoryInfo(@"source");
            var exeFileFullName = dir.GetFiles().FirstOrDefault(x => x.FullName.EndsWith(".exe")).FullName;
            var exeFileName = System.IO.Path.GetFileName(exeFileFullName);


            var project = new ManagedProject(projectName,
                             new Dir($"%ProgramFiles%\\{company}\\QCTrader",
                             new Dir("Resources", new Files(@"source\resources\*.*")),
                             new Files(@"source\*.dll"),
                             new Files(@"source\*.exe"),
                             new Files(@"source\*.config"), //app.exe.config,nlog.config
                             new File(@"source\readme.txt"),                         
                             new Dir("%Desktop%", new ExeFileShortcut(projectName, "[INSTALLDIR]" + exeFileName, "")),
                             new Dir($"%ProgramMenu%\\{company}\\{product}",
                                    new ExeFileShortcut("期货交易客户端", "[INSTALLDIR]" + exeFileName, ""),
                                    new ExeFileShortcut("使用须知", "[INSTALLDIR]" + "readme.txt", ""),
                                    new ExeFileShortcut("卸载", "[System64Folder]msiexec.exe", "/x [ProductCode]") { IconFile = @"images\uninstall.ico" }))
                             
                             )
            {
                Actions= new WixSharp.Action[]
                {
                  //  new InstalledFileAction(@"Packages\runtime461.exe", "", Return.check, When.Before, Step.InstallFiles, Condition.NOT_Installed )  ,
             
                //{
                //    UsesProperties = "Prop=Install", // need to tunnel properties since ElevatedManagedAction is a deferred action
                //    RollbackArg = "Prop=Rollback"
                //},
                    new ManagedAction(InstallAction.ReadmeAfterInstalled, Return.ignore, When.After, Step.InstallFinalize, Condition.NOT_Installed),
                }
            };
            project.SetNetFxPrerequisite(Condition.Net472_Installed,"请先安装.net 4.7.2运行时");
      
            project.ManagedUI = ManagedUI.Empty;
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
          
           
              //  System.Diagnostics.Process.Start("nodepad.exe", e.InstallDir + @"\readme.txt");
        }
    }

    public class InstallAction
    {
        /// <summary>
        /// 执行安装后打开readme
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult ReadmeAfterInstalled(Session session)
        {
            System.Diagnostics.Process.Start("Notepad.exe", session["INSTALLDIR"] + @"\readme.txt");
            return ActionResult.Success;
        }
    }
}