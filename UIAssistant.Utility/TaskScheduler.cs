using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using Microsoft.Win32.TaskScheduler;

namespace UIAssistant.Utility
{
    public static class AutoRunAtLoginScheduler
    {
        static TaskScheduler _taskScheduler = new TaskScheduler("UIAssistant", "Autostart UIAssistant at login");

        public static void Register()
        {
            _taskScheduler.AddTaskSchedule();
        }

        public static void Unregister()
        {
            _taskScheduler.RemoveTaskSchedule();
        }

        public static bool Exists()
        {
            return _taskScheduler.AlreadyExists();
        }
    }

    internal sealed class TaskScheduler
    {
        private string Author { get; set; }
        private string ScheduleName { get; set; }
        private string Description { get; set; }
        private string Documentation { get; set; }

        public TaskScheduler(string author, string scheduleName, string description = "", string documentation = "")
        {
            Author = author;
            ScheduleName = scheduleName;
            Description = description;
            Documentation = documentation;
        }

        private string ScheduleNameWithUserName
        {
            get
            {
                return $@"{ScheduleName} for {Environment.UserName}";
            }
        }

        public bool AlreadyExists()
        {
            using (TaskService service = new TaskService())
            {
                return service.FindTask(ScheduleNameWithUserName) != null;
            }
        }

        public void AddTaskSchedule()
        {
            var user = $@"{Environment.UserDomainName}\{Environment.UserName}";
            var appPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            var appDirPath = System.IO.Directory.GetParent(appPath).FullName;

            using (var service = new TaskService())
            {
                Version ver = service.HighestSupportedVersion;
                bool newVer = (ver >= new Version(1, 2));

                try
                {
                    TaskDefinition td = service.NewTask();
                    td.Principal.UserId = user;
                    td.Principal.LogonType = TaskLogonType.InteractiveToken;
                    td.RegistrationInfo.Author = Author;
                    td.RegistrationInfo.Description = Description;
                    td.RegistrationInfo.Documentation = Documentation;
                    td.Settings.DisallowStartIfOnBatteries = false;
                    td.Settings.Enabled = true;
                    td.Settings.Hidden = false;
                    td.Settings.IdleSettings.RestartOnIdle = false;
                    td.Settings.IdleSettings.StopOnIdleEnd = false;
                    td.Settings.Priority = System.Diagnostics.ProcessPriorityClass.Normal;
                    td.Settings.RunOnlyIfIdle = false;
                    td.Settings.RunOnlyIfNetworkAvailable = false;
                    td.Settings.StopIfGoingOnBatteries = false;
                    if (newVer)
                    {
                        if (UAC.IsAdministrator())
                        {
                            td.Principal.RunLevel = TaskRunLevel.Highest;
                        }
                        else
                        {
                            td.Principal.RunLevel = TaskRunLevel.LUA;
                        }
                        td.Settings.AllowDemandStart = true;
                        td.Settings.AllowHardTerminate = true;
                        td.Settings.Compatibility = TaskCompatibility.V2;
                        td.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
                        td.Settings.StartWhenAvailable = false;
                        td.Settings.WakeToRun = false;
                        td.Settings.ExecutionTimeLimit = TimeSpan.Zero;
                    }

                    LogonTrigger lTrigger = (LogonTrigger)td.Triggers.Add(new LogonTrigger());
                    if (newVer)
                    {
                        lTrigger.UserId = user;
                        lTrigger.Enabled = true;
                        lTrigger.StartBoundary = DateTime.MinValue;
                    }

                    td.Actions.Add(new ExecAction(appPath, null, appDirPath));

                    using (var folder = service.RootFolder)
                    {
                        folder.RegisterTaskDefinition(ScheduleNameWithUserName, td, TaskCreation.CreateOrUpdate, null, null, TaskLogonType.InteractiveToken, null);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.ToString());
                }
            }
        }

        private void RemoveTask(string name)
        {
            using (TaskService ts = new TaskService())
            {
                try
                {
                    ts.RootFolder.DeleteTask(name);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.ToString());
                }
            }
        }

        public void RemoveTaskSchedule()
        {
            RemoveTask(ScheduleNameWithUserName);
        }
    }

    // The MIT License(MIT)
    // Copyright(c) 2016 DOBON! <http://dobon.net>
    // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    // The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    // http://dobon.net/vb/dotnet/system/isadmin.html
    public static class UAC
    {
        static public bool IsAdministratorsMember()
        {
            //ローカルコンピュータストアのPrincipalContextオブジェクトを作成する
            using (PrincipalContext pc = new PrincipalContext(ContextType.Machine))
            {
                //現在のユーザーのプリンシパルを取得する
                UserPrincipal up = UserPrincipal.Current;
                //ローカルAdministratorsグループを探す
                //"S-1-5-32-544"はローカルAdministratorsグループを示すSID
                GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, "S-1-5-32-544");
                //グループのメンバーであるか調べる
                return up.IsMemberOf(gp);
            }
        }

        static public bool IsAdministrator()
        {
            //現在のユーザーを表すWindowsIdentityオブジェクトを取得する
            System.Security.Principal.WindowsIdentity wi =
                System.Security.Principal.WindowsIdentity.GetCurrent();
            //WindowsPrincipalオブジェクトを作成する
            System.Security.Principal.WindowsPrincipal wp =
                new System.Security.Principal.WindowsPrincipal(wi);
            //Administratorsグループに属しているか調べる
            return wp.IsInRole(
                System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass,
            IntPtr TokenInformation,
            uint TokenInformationLength,
            out uint ReturnLength);

        public enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }

        public enum TOKEN_ELEVATION_TYPE
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        /// <summary>
        /// 昇格トークンの種類を取得する
        /// </summary>
        /// <returns>昇格トークンの種類を示すTOKEN_ELEVATION_TYPE。
        /// 取得に失敗した時でもTokenElevationTypeDefaultを返す。</returns>
        public static TOKEN_ELEVATION_TYPE GetTokenElevationType()
        {
            TOKEN_ELEVATION_TYPE returnValue =
                TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;

            //Windows Vista以上か確認
            if (Environment.OSVersion.Platform != PlatformID.Win32NT ||
                Environment.OSVersion.Version.Major < 6)
            {
                return returnValue;
            }

            TOKEN_ELEVATION_TYPE tet =
                TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
            uint returnLength = 0;
            uint tetSize = (uint)Marshal.SizeOf((int)tet);
            IntPtr tetPtr = Marshal.AllocHGlobal((int)tetSize);
            try
            {
                //アクセストークンに関する情報を取得
                if (GetTokenInformation(
                    System.Security.Principal.WindowsIdentity.GetCurrent().Token,
                    TOKEN_INFORMATION_CLASS.TokenElevationType,
                    tetPtr, tetSize, out returnLength))
                {
                    //結果を取得
                    returnValue = (TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(tetPtr);
                }
            }
            finally
            {
                //解放する
                Marshal.FreeHGlobal(tetPtr);
            }

            return returnValue;
        }

        public static bool HasAdministratorAuthorization()
        {
            if (IsAdministrator())
            {
                return true;
            }

            TOKEN_ELEVATION_TYPE type = GetTokenElevationType();
            if (type == TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
            {
                return false;
            }

            return true;
        }
    }
}
