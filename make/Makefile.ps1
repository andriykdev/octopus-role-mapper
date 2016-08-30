Define-Step -Name 'Update Assembly Info' -Target 'build' -Body {
	. (require 'psmake.mod.update-version-info')
	Update-VersionInFile AssemblyVersion.cs $VERSION 'Version("%")'
}

Define-Step -Name 'Build' -Target 'build' -Body {
	call $Context.NugetExe restore OctopusRoleMapper.sln
	call "${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\msbuild.exe" OctopusRoleMapper.sln /t:"Clean,Build" /p:Configuration=Release /m /verbosity:m /nologo /p:TreatWarningsAsErrors=true
}

Define-Step -Name 'Unit Tests' -Target 'build' -Body {
	. (require 'psmake.mod.testing')

	$tests = @()
	$tests += Define-NUnitTests -GroupName 'Unit Tests' -TestAssembly "*.Tests\bin\Release\*.Tests.dll"

	$tests | Run-Tests -EraseReportDirectory -Cover -CodeFilter '+[OctopusRoleMapper*]* -[*.Tests*]*' -TestFilter '*.Tests.dll' | Generate-CoverageSummary | Check-AcceptableCoverage -AcceptableCoverage 90
}

Define-Step -Name 'Packaging' -Target 'build' -Body {
	. (require 'psmake.mod.packaging')
	
	Package-DeployableNuSpec -Package 'OctopusRoleMapper.Console.nuspec' -version $VERSION
}
