param($installPath, $toolsPath, $package, $project)

Write-Host

if (-Not $dte) {
  $dte = $project.DTE
}
$solution = $dte.Solution
if (-Not $solution.Saved) {
  $dte.ExecuteCommand("File.SaveAll")
}

$solutionName = $solution.Name
$projectName = $project.Name
$solutionPath = (Get-ItemProperty -Path $solution.FullName).DirectoryName
$projectPath = (Get-ItemProperty -Path $project.FullName).DirectoryName
$contentFolderName = "T4"
$contentFolderPath = Join-Path -Path $projectPath -ChildPath $contentFolderName
$packageFolderPath = Join-Path -Path (Join-Path -Path $installPath -ChildPath "content") -ChildPath $contentFolderName;
$ttFilePattern = "NTierEF.*.ttinclude"

Write-Host "Update n-tier entity framework T4 templates for $projectName" 

$projectFolder = $project.ProjectItems.Item($contentFolderName)
if ($projectFolder) {
  ###
  # force update from nuget package content folder to VS project folder
  ###
  $t4Files = Get-ChildItem $packageFolderPath
  $t4Files | Foreach-Object { Write-Host "  force update" $_.Name }
  $t4Files | Copy-Item -Destination $contentFolderPath -Force

  ###
  # move tt files from VS project folder to solution level folder
  ###
  $sourcePath = Join-Path -Path $contentFolderPath -ChildPath $ttFilePattern
  $destinationPath = Join-Path -Path $solutionPath -ChildPath $contentFolderName
  
  Write-Host "  move T4 templates from '$sourcePath' to '$destinationPath'"
  
  # create solution level directory
  if(-Not (Test-Path $destinationPath)) {
    New-Item -Path $destinationPath -ItemType Directory -Force | Out-Null
  }
  # move tt files
  Move-Item -Path $sourcePath -Destination $destinationPath -Force
  
  # remove tt files from VS project folder
  $projectFolder.ProjectItems | Where-Object { $_.Name -like $ttFilePattern } | Foreach-Object { $_.Delete() }
  
  # remove VS project folder from VS project
  if ($projectFolder.ProjectItems.Count -eq 0) {   
	$projectFolder.Remove()
	Remove-Item $contentFolderPath
  }
  
  # add solution folder to VS solution
  $solutionFolder = $solution.Projects | Where-Object { $_.Name -eq $contentFolderName }
  if (-Not $solutionFolder) {
    $solutionFolder = $solution.AddSolutionFolder($contentFolderName)
  }
  
  # add tt files to VS solution folder
  Get-ChildItem $destinationPath | Foreach-Object { 
	$fileName = $_.Name
	$item = $solutionFolder.ProjectItems | Where-Object { $_.Name -eq $fileName }
	if (-Not $item) {
	  $solutionFolder.ProjectItems.AddFromFile($_.FullName) | Out-Null 
	}
  }
  
  # close tt files
  $dte.Documents | Where-Object { $_.Name -like $ttFilePattern } | Foreach-Object { $_.Close() }
}

Write-Host
