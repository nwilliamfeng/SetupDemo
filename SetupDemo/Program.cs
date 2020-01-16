using System;
using System.Windows.Forms;
using WixSharp;
using System.Linq;
using WixSharp.Forms;
using WixSharp.CommonTasks;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp.Bootstrapper;
using System.Net;



// NuGet console: Install-Package WixSharp
// NuGet Manager UI: browse tab
//https://csharp.hotexamples.com/examples/WixSharp/Feature/-/php-feature-class-examples.html
namespace SetupDemo
{
    class Program
    {
        static void Main()
        {
            var projectName = "QCTrader";
            string productMsi = BuildMsi();
            var bootstrapper = new Bundle(projectName,
               Net461(),
                new MsiPackage(productMsi)
                { 
                    DisplayInternalUI = true,
                  
                }
                );
            var themes = new[]
{
    new Payload(@"thm_zh-CN.wxl")
    {
        Name = @"language\WixUI_zh-CN.wxl"
    },
   
};
            bootstrapper.Application.Payloads.Combine(themes);

            bootstrapper.Version = Tasks.GetVersionFromFile(productMsi); //will extract "product version"
            bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
            bootstrapper.Include(WixExtension.Util);
            bootstrapper.AddWixFragment("Wix/Bundle",
                                  new UtilRegistrySearch
                                  {
                                      Root = RegistryHive.LocalMachine,
                                      Key = @"SOFTWARE\Microsoft\Net Framework Setup\NDP\v4\Full",
                                      Value = "Release",
                                      Variable = "NetVersion"
                                  }
                                  );
            bootstrapper.Build("qctraderBootStrapper.exe");
        }

        private static ExePackage Net461()
        {
        
            string webInstaller =
              @"http://download.microsoft.com/download/3/5/9/35980F81-60F4-4DE3-88FC-8F962B97253B/NDP461-KB3102438-Web.exe";
            string net461Installer = "Net461-web.exe";
            using (var client = new WebClient())
            {
                client.DownloadFile(webInstaller, net461Installer);
            }
            ExePackage exe = new ExePackage(net461Installer)
            {
                Compressed = true,
                Vital = true,
                Name = "Net461-web.exe",
                DetectCondition ="NetVersion >= 394254",
                PerMachine = true,
                Permanent = true,
            };

            return exe;
        }

        private static string BuildMsi()
        {
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
                Actions = new WixSharp.Action[]
                {
                  //  new InstalledFileAction(@"Packages\runtime461.exe", "", Return.check, When.Before, Step.InstallFiles, Condition.NOT_Installed )  ,
             
                //{
                //    UsesProperties = "Prop=Install", // need to tunnel properties since ElevatedManagedAction is a deferred action
                //    RollbackArg = "Prop=Rollback"
                //},
                    new ManagedAction(InstallAction.ReadmeAfterInstalled, Return.ignore, When.After, Step.InstallFinalize, Condition.NOT_Installed),
                }
            };
            project.SetNetFxPrerequisite(Condition.Net461_Installed, "请先安装.net 4.6.1运行时");

            project.ManagedUI = ManagedUI.Empty;
            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
            project.LicenceFile = @"licenses\licence.rtf"; //注意必须时rtf格式文件
            AutoElements.UACWarning = "安装程序需要用户账号控制（UAC）确认，请点击确认...";

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

            project.LocalizationFile = @"language\WixUI_zh-CN.wxl";
            project.Language = "zh-cn";


            return project.BuildMsi();
        }

        static void Msi_Load(SetupEventArgs e)
        {
           //  if (!e.IsUISupressed && !e.IsUninstalling)
        
            //    MessageBox.Show(e.ToString(), "Load");
        }

        static void Msi_BeforeInstall(SetupEventArgs e)
        {
          //   if (!e.IsUISupressed && !e.IsUninstalling)
           //     MessageBox.Show(e.ToString(), "BeforeInstall");
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