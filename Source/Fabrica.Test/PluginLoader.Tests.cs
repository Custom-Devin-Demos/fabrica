// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GEAviation.Fabrica.Extensibility;
using NUnit.Framework;

namespace Fabrica.Test
{
    public interface ITestPlugin
    {
    }

    public class TestPlugin : ITestPlugin
    {
    }

    [TestFixture]
    public class PluginLoaderTests
    {
        [Test]
        public void loadsPluginsFromDirectoryInCurrentDomain()
        {
            string lLoadPath = TestContext.CurrentContext.TestDirectory;
            Regex lFileType = new Regex( Regex.Escape( "Fabrica.Test.dll" ) + "$" );

            PluginLoader<ITestPlugin> lLoader = null;
            Assert.DoesNotThrow( () => lLoader = new PluginLoader<ITestPlugin>( lLoadPath, lFileType ) );

            Type[] lTypes = lLoader.loadAllTypes().ToArray();
            ITestPlugin[] lPlugins = lLoader.load().ToArray();

            Assert.AreEqual( 1, lLoader.TotalFiles );
            Assert.Contains( typeof( TestPlugin ), lTypes );
            Assert.AreEqual( 1, lPlugins.Length );
            Assert.IsInstanceOf<TestPlugin>( lPlugins[0] );
        }
    }
}
