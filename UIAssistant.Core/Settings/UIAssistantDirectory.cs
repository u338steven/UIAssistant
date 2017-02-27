using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;

namespace UIAssistant.Core.Settings
{
    public static class UIAssistantDirectory
    {
        public static string Executable => Directory.GetParent(Assembly.GetCallingAssembly().Location).ToString();
        public static string Plugins => Path.Combine(Executable, "Plugins");
        public static string _configurations = null;
        public static string Configurations
        {
            get
            {
                if (_configurations != null)
                {
                    return _configurations;
                }
                if (HasWritePermission(Executable))
                {
                    _configurations = Path.Combine(Executable, "Configurations");
                    return _configurations;
                }
                _configurations = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UIAssistant", "Configurations");
                return _configurations;
            }
        }

        private static bool IsCurrentUserRule(WindowsPrincipal currentUser, FileSystemAccessRule rule)
        {
            if (rule.IdentityReference.Value.StartsWith("S-1-"))
            {
                var sid = new SecurityIdentifier(rule.IdentityReference.Value);
                if (!currentUser.IsInRole(sid))
                {
                    return false;
                }
            }
            else
            {
                if (!currentUser.IsInRole(rule.IdentityReference.Value))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool HasWritePermission(string FilePath)
        {
            try
            {
                FileSystemSecurity security;
                if (File.Exists(FilePath))
                {
                    security = File.GetAccessControl(FilePath);
                }
                else
                {
                    security = Directory.GetAccessControl(Path.GetDirectoryName(FilePath));
                }
                var rules = security.GetAccessRules(true, true, typeof(NTAccount));

                var currentUser = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                var result = false;
                foreach (FileSystemAccessRule rule in rules)
                {
                    if (!rule.FileSystemRights.HasFlag(FileSystemRights.WriteData))
                    {
                        continue;
                    }

                    if (!IsCurrentUserRule(currentUser, rule))
                    {
                        continue;
                    }

                    // Deny: high priority
                    if (rule.AccessControlType == AccessControlType.Deny)
                    {
                        return false;
                    }

                    if (rule.AccessControlType == AccessControlType.Allow)
                    {
                        result = true;
                    }
                }
                return result;
            }
            catch
            {
                return false;
            }
        }
    }
}
