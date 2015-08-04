using System;
using System.IO;

namespace Sourcery.Tests
{
    [NUnit.Framework.SetUpFixture]
    public class ClearTestDataSetUpFixture
    {
        [NUnit.Framework.SetUp]
        public void SetUp()
        {
            var di = new DirectoryInfo(AppDomain.CurrentDomain.GetData("DataDirectory") as string ??
                                       (AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/App_Data"));
            if (di.Exists) di.Delete(true);
            di.Create();

        }
    }
}