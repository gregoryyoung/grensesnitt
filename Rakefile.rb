require "rubygems"
require "bundler"
Bundler.setup
$: << './'

require 'albacore'
require 'rake/clean'
require 'semver'

require 'buildscripts/utils'
require 'buildscripts/paths'
require 'buildscripts/project_details'
require 'buildscripts/environment'

# to get the current version of the project, type 'SemVer.find.to_s' in this rake file.

desc 'generate the shared assembly info'
assemblyinfo :assemblyinfo => ["env:release"] do |asm|
  data = commit_data() #hash + date
  asm.product_name = asm.title = PROJECTS[:a][:title]
  asm.description = PROJECTS[:a][:description] + " #{data[0]} - #{data[1]}"
  asm.company_name = PROJECTS[:a][:company]
  # This is the version number used by framework during build and at runtime to locate, link and load the assemblies. When you add reference to any assembly in your project, it is this version number which gets embedded.
  asm.version = BUILD_VERSION
  # Assembly File Version : This is the version number given to file as in file system. It is displayed by Windows Explorer. Its never used by .NET framework or runtime for referencing.
  asm.file_version = BUILD_VERSION
  asm.custom_attributes :AssemblyInformationalVersion => "#{BUILD_VERSION}", # disposed as product version in explorer
    :CLSCompliantAttribute => false,
    :AssemblyConfiguration => "#{CONFIGURATION}",
    :Guid => PROJECTS[:a][:guid]
  asm.com_visible = false
  asm.copyright = PROJECTS[:a][:copyright]
  asm.output_file = File.join(FOLDERS[:src], 'SharedAssemblyInfo.cs')
  asm.namespaces = "System", "System.Reflection", "System.Runtime.InteropServices", "System.Security"
end


desc "build sln file"
msbuild :msbuild do |msb|
  msb.solution   = FILES[:sln]
  msb.properties :Configuration => CONFIGURATION
  msb.targets    :Clean, :Build
end


task :a_output => [:msbuild] do
  target = File.join(FOLDERS[:binaries], PROJECTS[:a][:id])
  copy_files FOLDERS[:a][:out], "*.{mdb,dll}", target
  CLEAN.include(target)
end


task :g_output => [:msbuild] do
  target = File.join(FOLDERS[:binaries], PROJECTS[:g][:id])
  copy_files FOLDERS[:g][:out], "*.{xml,dll,pdb,config}", target
  CLEAN.include(target)
end

task :output => [:a_output, :g_output]
task :nuspecs => [:g_nuspec]

desc "Create a nuspec for 'grensesnitt.Framework'"
nuspec :g_nuspec do |nuspec|
  nuspec.id = "#{PROJECTS[:g][:nuget_key]}"
  nuspec.version = BUILD_VERSION
  nuspec.authors = "#{PROJECTS[:g][:authors]}"
  nuspec.description = "#{PROJECTS[:g][:description]}"
  nuspec.title = "#{PROJECTS[:g][:title]}"
  # nuspec.projectUrl = 'http://github.com/haf' # TODO: Set this for nuget generation
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0" # TODO: set this for nuget generation
  nuspec.requireLicenseAcceptance = "false"
  
  nuspec.output_file = FILES[:g][:nuspec]
  
  puts "## target: #{FOLDERS[:g][:nuspec]}/tools"
  copy_files FOLDERS[:a][:out], "#{PROJECTS[:a][:title]}.{xml,dll,mdb}", File.join(FOLDERS[:g][:nuspec], "tools")
  nuspec_copy(:g, "#{PROJECTS[:g][:id]}.{dll,pdb,xml}")
end

task :nugets => [:"env:release", :nuspecs, :g_nuget]

desc "nuget pack 'grensesnitt.Framework'"
nugetpack :g_nuget do |nuget|
   nuget.command     = "#{COMMANDS[:nuget]}"
   nuget.nuspec      = "#{FILES[:g][:nuspec]}"
   # nuget.base_folder = "."
   nuget.output      = "#{FOLDERS[:nuget]}"
end

task :publish => [:"env:release", :g_nuget_push]

desc "publishes (pushes) the nuget package 'grensesnitt.Framework'"
nugetpush :g_nuget_push do |nuget|
  nuget.command = "#{COMMANDS[:nuget]}"
  nuget.package = "#{File.join(FOLDERS[:nuget], PROJECTS[:g][:nuget_key] + "." + BUILD_VERSION + '.nupkg')}"
# nuget.apikey = "...."
  nuget.source = URIS[:nuget_offical]
  nuget.create_only = false
end

task :default  => ["env:release", "assemblyinfo", "msbuild", "output", "nugets"]