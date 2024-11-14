using System;
using System.IO;

namespace OBan
{
    public class GetDirectory
    {
        internal string GetParentDirectory()
        {
            string parentPath = Path.GetDirectoryName(Plugin.plugin.ConfigPath);
            for (int i = 0; i < 2; i++)
            {
                parentPath = Directory.GetParent(parentPath)?.FullName;
                if (parentPath == null)
                {
                    throw new InvalidOperationException("It is impossible to go higher than the root directory.");
                }
            }
            return parentPath;
        }
    }
}
