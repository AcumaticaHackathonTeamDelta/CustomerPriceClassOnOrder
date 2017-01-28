#
#   Created by BHennelly@jaas.net
#   Rebuild Web.Config from Web.Config.* files
#

$devFileName = 'web.config.DEV'
$baseWebConfigName = 'web.config.BASE'
$siteRoot = Join-Path $PSScriptRoot '..\Site'
$erpFolder = Join-Path $PSScriptRoot '..\ERP'
[string]$snapshotsFolder = Join-Path $erpFolder 'Snapshots'
[string]$customizationTempFolder = Join-Path $erpFolder 'Customization'
[string]$backupFolder = Join-Path $erpFolder 'Backup'

Function CheckExistsWebConfig()
{
    $siteWebConfig = Join-Path $siteRoot 'web.config'
    $baseWebConfig = Join-Path $siteRoot $baseWebConfigName

    #if not exists create it
    if(!(Test-Path $siteWebConfig) -and (Test-Path $baseWebConfig))
    {
        Write-Host ('Creating {0} from base' -f $siteWebConfig)
        #copy base as normal for first run...
        Copy-Item $baseWebConfig $siteWebConfig
    }
}

# Check for the DEV settings file and if not exist create it
Function CheckDevFile()
{
    $filePath = Join-Path $siteRoot $devFileName

    #if not exists create it
    if(!(Test-Path $filePath))
    {
        #read current web.config and copy the db string
        $webConfigFilePath = Join-Path $siteRoot 'web.config'

        $xml = [xml](Get-Content $webConfigFilePath)
     
        $connNodes = $xml.configuration.connectionStrings.add
        $connectionString = ''
        #starting in 6.0 there are 2 connection strings in the web config...
        foreach($node in $connNodes)
        {
            if($node.name -eq "ProjectX")
            {
                $connectionString = $node.connectionString
                #$node.connectionString = 'data source=[YourServer];Initial Catalog=[YourDB];Integrated Security=False;User ID=[user];Password=[pass]'
            }
            # new starting in 6.0 beta version 6.00.0732 released 7/26/2016
            #if($node.name -eq "ProjectX_MySql")
            #{
            #    $node.connectionString = 'Server=localhost;Database=debug50_3;Uid=root;Pwd=Aw34esz;found rows=true;Unicode=true;'
            #}
        }

        New-Item $filePath -type file
        Add-Content $filePath $connectionString
    }
}

Function MakeErpFolders()
{
    if(!(Test-Path $erpFolder))
    {
        Write-Host ('[Dev Web Config] Creating ERP Folder: {0}' -f $erpFolder)
        New-Item $erpFolder -type directory | Out-Null
    }

    if(!(Test-Path $snapshotsFolder))
    {
        Write-Host ('[Dev Web Config] Creating Snapshot Folder: {0}' -f $snapshotsFolder)
        New-Item $snapshotsFolder -type directory | Out-Null
    }

    if(!(Test-Path $customizationTempFolder))
    {
        Write-Host ('[Dev Web Config] Creating Customization Temp Folder: {0}' -f $customizationTempFolder)
        New-Item $customizationTempFolder -type directory | Out-Null
    }

    if(!(Test-Path $backupFolder))
    {
        Write-Host ('[Dev Web Config] Creating Backup Folder: {0}' -f $backupFolder)
        New-Item $backupFolder -type directory | Out-Null
    }
}

# Take the Web.Config.BASE and make it the new Web.Config file
#  Also copy in the connection string from Web.Config.DEV
Function ReplaceWebConfigFromBase()
{
    $sourcePath = Join-Path $siteRoot $baseWebConfigName
    $destinationPath = Join-Path $siteRoot 'web.config'
    
    if(Test-Path $sourcePath)
    {
        Copy-Item -Force $sourcePath $destinationPath
    }

    $devFile = Join-Path $siteRoot $devFileName
    $devContent = Get-Content $devFile
    $dbString = [string]$devContent

    # copy in the db string
    $xml = [xml](Get-Content $destinationPath)
     
    # update connection string
    $connNodes = $xml.configuration.connectionStrings.add
    #starting in 6.0 there are 2 connection strings in the web config...
    foreach($node in $connNodes)
    {
        if($node.name -eq "ProjectX")
        {
            $node.connectionString = $dbString
        }
    }

    # make sure debug set to true
    $node2 = $xml.configuration.'system.web'.compilation
    $node2.debug = "True"

    MakeErpFolders

    $nodeCustomizationPaths = $xml.selectNodes('//configuration//appSettings//add')
    foreach ($path in $nodeCustomizationPaths) {
      if($path.key -eq "CustomizationTempFilesPath"){
        $path.value = $customizationTempFolder
      }

      if($path.key -eq "SnapshotsFolder"){
        $path.value = $snapshotsFolder
      }

      if($path.key -eq "BackupFolder"){
        $path.value = $backupFolder
      }

      #Write-Host $path.value
    }

    $xml.Save($destinationPath)
}


$sleepTimeSeconds = 5

Try
{
    CheckExistsWebConfig
    CheckDevFile
    ReplaceWebConfigFromBase
}
Catch
{
    Write-Warning '*************************************'
    Write-Warning 'script failed'
    Write-Error $_.Exception.Message
    Write-Warning '*************************************'
    
    $sleepTimeSeconds = 30
}
Finally
{
    Write-Host ''
    Write-Host 'End of process'
    #for short display to the user...
    Start-Sleep -s $sleepTimeSeconds
}