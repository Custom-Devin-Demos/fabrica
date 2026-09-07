// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GEAviation.Fabrica.Definition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Fabrica.Test
{
    public static class CompileTestParts
    {
        public static Assembly getPartsAssembly(string aResourceName)
        {
            // Creating this dummy def to force load the main Fabrica dll...
            //var lDummy = PartSpecification.createPartSpecification( typeof(SpecTestPart1) );

            // Snagged from: https://stackoverflow.com/questions/24871955/c-sharp-compilerresults-generateinmemory
            var lReferencedAssemblies =
                AppDomain.CurrentDomain.GetAssemblies()
                         .Where(a => !a.IsDynamic) //necessary because a dynamic assembly will throw and exception when calling a.Location
                         .Where(a => !string.IsNullOrEmpty(a.Location))
                         .Select(a => MetadataReference.CreateFromFile(a.Location))
                         .ToArray();

            var lCodeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( aResourceName );
            string lGoodPartsCode = string.Empty;

            using( StreamReader lSR = new StreamReader( lCodeStream ) )
            {
                lGoodPartsCode = lSR.ReadToEnd();
            }

            var lCompilation = CSharpCompilation.Create( Path.GetRandomFileName(),
                                                         new[] { CSharpSyntaxTree.ParseText( lGoodPartsCode ) },
                                                         lReferencedAssemblies,
                                                         new CSharpCompilationOptions( OutputKind.DynamicallyLinkedLibrary ) );

            using( var lAssemblyStream = new MemoryStream() )
            {
                var lEmitResult = lCompilation.Emit( lAssemblyStream );

                if( !lEmitResult.Success )
                {
                    var lErrors = lEmitResult.Diagnostics.Where( aDiag => aDiag.Severity == DiagnosticSeverity.Error );
                    throw new InvalidOperationException( string.Join( Environment.NewLine, lErrors ) );
                }

                return Assembly.Load( lAssemblyStream.ToArray() );
            }
        }

        public static Assembly getGoodPartsAssembly()
        {
            return getPartsAssembly( "Fabrica.Test.GoodTestParts.Code.cs" );
        }
    }
}
