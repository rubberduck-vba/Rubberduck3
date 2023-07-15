#Requires -RunAsAdministrator

[cmdletbinding(SupportsShouldProcess = $True)]
param (
    [Parameter(Mandatory = $False)]
    [ValidateSet("CurrentVersion", "rd2", "rd3", "2", "3")] 
    [string]$requestedOp
)

################################ Script starts at line 714 ################################

$script:toggleModel = @{}
$script:rdHKCRKeys = @()
$script:rd3KeysCacheErrorMsg = ""
$script:numberOfRD3Keys = 2
$script:rd3ModelsFromFile = @{}
$script:rd3HKLMCachePath = ""

$HKLMClsidPath = "Registry::HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\"
$HKCRClsidPath = "Registry::HKEY_CLASSES_ROOT\CLSID\"

$extensionClsid = "69E0F697-43F0-3B33-B105-9B8188A6F040"
$dockableToolWindowClsid = "69E0F699-43F0-3B33-B105-9B8188A6F040"

$rdExtension = @{
    HKLMPath = "${HKLMClsidPath}{${extensionClsid}}"
    ShortHKLMPath = "HKLM:\SOFTWARE\Classes\CLSID\{<Extension>}"
    HKCRPath = "${HKCRClsidPath}{${extensionClsid}}"
    ShortHKCRPath = "HKCR:\CLSID\{<Extension>}"
    InProcServer32Path = "Registry::HKEY_CLASSES_ROOT\CLSID\{${extensionClsid}}\InProcServer32"
}

$rdDockableToolWindow = @{
    HKLMPath = "${HKLMClsidPath}{${dockableToolWindowClsid}}"
    ShortHKLMPath = "HKLM:\SOFTWARE\Classes\CLSID\{<DockableToolWindow>}"
    HKCRPath = "${HKCRClsidPath}{${dockableToolWindowClsid}}"
    ShortHKCRPath = "HKCR:\CLSID\{<DockableToolWindow>}"
    InProcServer32Path = "Registry::HKEY_CLASSES_ROOT\CLSID\{${dockableToolWindowClsid}}\InProcServer32"
}

$rdRegistryKeyPropertyNames = @("Assembly", "Class", "CodeBase", "RuntimeVersion")

#null parameter allows injecting SubKeyNames for testing
function Initialize-ScriptScopeFields($rdHKCRKeys = $null){
    if ($null -ne $rdHKCRKeys){
        $script:rdHKCRKeys = $rdHKCRKeys
    } else {
        $script:rdHKCRKeys = 
            (Get-SubKeyNames "ClassesRoot" "CLSID\{${extensionClsid}}\InProcServer32")
    }
    $script:toggleModel = New-ToggleModel $script:rdHKCRKeys    
    $script:rd3KeysCacheErrorMsg = "Prior RD3 values unavailable." + `
        "Please clean/build the Rubberduck3 solution to setup the registry"

    $script:rd3HKLMCachePath = (Get-ChildItem env:USERPROFILE).Value + `
        "\AppData\Roaming\Rubberduck\RD3HKLMValues.json"

    #There are currently(4/23) 2 registry keys to be added for RD3: 
    #Rubberduck.Extension and Rubberduck.DockableToolWindow
    $script:numberOfRD3Keys = 2

    if (-not (Test-IsConfiguredForRD3) -and (Test-Path (Get-CachedRD3KeyValuesFilepath))){

        $script:rd3ModelsFromFile = New-RegistryKeyModelsFromFile
        if (($null -ne $script:rd3ModelsFromFile) -and ($script:rd3ModelsFromFile.Count -eq $script:numberOfRD3Keys)){
            $script:toggleModel.Rd3Version = Get-Rd3VersionFromCacheModel $script:rd3ModelsFromFile
        }    
    }
}

function Test-CanProcessToTargetVersion($targetIsRD2){

    Write-Verbose "Evaluating Registry content..."

    $result = @{CanProcess = $False}

    $activeVersion = Convert-VersionToXVersion (Get-ActiveVersion)

    if ((Test-IsConfiguredForRD2) -and $targetIsRD2){
        $result.Message = ("Registry is configured for Rubberduck Version ${activeVersion} - Nothing to do")
        $result
        return
    }
    
    if ((Test-IsConfiguredForRD3) -and (-not $targetIsRD2)){
        $result.Message = ("Registry is configured for Rubberduck Version ${activeVersion} - Nothing to do")
        $result
        return
    }
    
    if (-not $targetIsRD2){

        $valuesFromFileFailureMsg = $script:rd3KeysCacheErrorMsg

        $cachedDataPath = Get-CachedRD3KeyValuesFilepath
        if (-not (Test-Path $cachedDataPath) -or (Test-FileIsLocked $cachedDataPath)){
            $result.Message = $valuesFromFileFailureMsg
            $result
            return
        }

        $regKeyModelsFromFile = New-RegistryKeyModelsFromFile

        if ($null -eq $regKeyModelsFromFile){
            $result.Message = $valuesFromFileFailureMsg
            $result
            return
        }

        if ($regKeyModelsFromFile.Count -ne $numberOfRD3Keys){
            $result.Message = $valuesFromFileFailureMsg
            $result
            return
        }

    } else {
        foreach ($prototypeKey in Get-RD2PrototypeKeys){
            if (-not (Test-Path $prototypeKey)){
                $result.Message = "RD2 Registry Key(s) not found - script expects to discover existing registry entries for RD2"
                $result
                return
            }
        }   
    }

    $result = @{
        "CanProcess" = $True
        "ModelsFromFile" = $regKeyModelsFromFile
    }
    $result
}

function Restore-RubberduckV2($rdHKLMClsidKeys){

    $rd2Version = Convert-VersionToXVersion $script:toggleModel.Rd2Version

    Write-RestoreStartMessage $rd2Version

    $registryKeyModels = Get-RegistryKeyModels $rdHKLMClsidKeys

    if (-not (Test-FileIsLocked (Get-CachedRD3KeyValuesFilepath)) ){
        Export-RD3RegistryValues $registryKeyModels
    }
        
    Remove-RD3RegistryKeys $rdHKLMClsidKeys

    Set-RD2InProcServer32Values

    Write-RestoreCompleteMessage $rd2Version
}

function Restore-RubberduckV3($regKeyModels){

    $rd3Version = Convert-VersionToXVersion $script:toggleModel.Rd3Version

    Write-RestoreStartMessage $rd3Version
    
    Add-RD3RegistryKeys $regKeyModels

    Set-RD3InProcServer32Values $regKeyModels

    Write-RestoreCompleteMessage $rd3Version
}

function Write-RestoreStartMessage($rdVersion){
    
    if ($WhatIfPreference){
        "What if: Restoring registry values to support Rubberduck Version " + $rdVersion
    } else{
        Write-Verbose ("Registry keys set to support Rubberduck Version " + $rdVersion)
    }
}

function Write-RestoreCompleteMessage($rdVersion){
    
    if ($WhatIfPreference){
        "What if: Registry keys set to support Rubberduck Version " + $rdVersion
    } else{
        Write-Verbose ("Registry keys set to support Rubberduck Version " + $rdVersion)
    }
}

function Get-Rd3VersionFromCacheModel($regKeyModels){
    if ($regKeyModels.Count -lt 1){
        return
    }

    $assemblyValue = $regKeyModels[0].Properties.Assembly
    $idxOf3 = $assemblyValue.IndexOf("3")

    $assemblyValue.Substring(`
        $idxOf3, $assemblyValue.IndexOf(",", $idxOf3) - $idxOf3)
}

function Add-RD3RegistryKeys($regKeyModels){

    foreach ($regModel in $regKeyModels){        
        if (Test-ContainsExtensionClsid $regModel.KeyPath){
            $shortPath = $rdExtension.ShortHKLMPath
        } else {
            $shortPath =  $rdDockableToolWindow.ShortHKLMPath
        }

        Write-Verbose ("Restore RD3 values to Key ${shortPath}\InProcServer32")

        Add-RegistryKeys (Get-Rd3HKLMPaths $regModel.KeyPath)

        $rdRegistryKeyPropertyNames | 
            ForEach-Object { Set-RegistryKeyProperties $regModel $_ }    
    }
}   

function New-ToggleModel($rdHKCRKeys){
    
    $toggleModel = @{
        Rd2Version = "TBD";
        Rd3Version = "TBD";
        ActiveVersion = "TBD"
    }
    
    $rdHKCRKeys | ForEach-Object { 
        if ($_ -like "2.*"){
            $toggleModel.Rd2Version = $_
        } elseif ($_ -like "3.*"){
            $toggleModel.Rd3Version = $_
        } 
    }
    
    # If both 2.5.2.x AND 3.0.0.0 are appear in HKCR, then RD3 is active
    # If only one RD key is found, then the version number of the found key is active.  
    #The missing key will have the value of 'TBD'
    if($rdHKCRKeys.Count -eq 2){
        $toggleModel.ActiveVersion = $toggleModel.Rd3Version
    } elseif ($toggleModel.Rd2Version -ne "TBD"){
        $toggleModel.ActiveVersion = $toggleModel.Rd2Version
    } elseif ($toggleModel.Rd3Version -ne "TBD"){
        $toggleModel.ActiveVersion = $toggleModel.Rd3Version
    }

    $toggleModel
}

function Get-CachedRD3KeyValuesFilepath($filepath = $null){

    if (-not $null -eq $filepath){
        $script:rd3HKLMCachePath = $filePath
    }
    $script:rd3HKLMCachePath
}

function Get-HKLMKeysOfInterest($keyNames){

    if ($keyNames.Count -eq 0){
        @()
        return
    }

    $result = $keyNames | Where-Object { Test-IsRDClsidOfInterest $_ } | 
        ForEach-Object {"${HKLMClsidPath}${_}"}

    if ($null -eq $result){
        $result = @()
    }

    $result
}

function Set-RD2InProcServer32Values(){
    [cmdletbinding(SupportsShouldProcess = $True)]
    param ()

    foreach ($key in Get-RD2PrototypeKeys){

        $InProcServer32Key = Get-InProcServer32PathFromKey $key

        if (Test-ContainsExtensionClsid $key){
            Write-Verbose ("Restore RD2 values to Key " + $rdExtension.ShortHKCRPath + "\InProcServer32")
        } else {
            Write-Verbose ("Restore RD2 values to Key " + $rdDockableToolWindow.ShortHKCRPath + "\InProcServer32")
        }

        if ($PSCmdlet.ShouldProcess($InProcServer32Key)){} 

        $model = New-RegistryKeyModelFromRegistryKey $key

        $model.KeyPath = $InProcServer32Key
        
        $rdRegistryKeyPropertyNames | ForEach-Object { 
            Set-RegistryKeyProperties $model $_        
        } 
    }
}

function Set-RD3InProcServer32Values(){
    [cmdletbinding(SupportsShouldProcess = $True)]
    param (
        [Parameter (Mandatory = $true)]
        [Object[]] $regKeyModels
    )

    foreach ($model in $regKeyModels){

        if (Test-ContainsExtensionClsid $model.KeyPath){
            $shortPath = $rdExtension.ShortHKCRPath
            $keyPath = $rdExtension.HKCRPath
        } else {
            $shortPath = $rdDockableToolWindow.ShortHKCRPath
            $keyPath = $rdDockableToolWindow.HKCRPath
        }

        $InProcServer32Key = $keyPath + "\InProcServer32"
        Write-Verbose ("Restore RD3 values to Key ${shortPath}\InProcServer32")

        if ($PSCmdlet.ShouldProcess($InProcServer32Key)){} 

        $clone = (New-RdRegistryKeyModel $model.KeyPath $model.Properties)
        $clone.KeyPath = $InProcServer32Key
        
        $rdRegistryKeyPropertyNames | ForEach-Object { 
            Set-RegistryKeyProperties $clone $_        
        } 
    }
}

function Get-RD2PrototypeKeys(){

    $rd2Version = Get-Rd2Version
    @(
        $rdExtension.InProcServer32Path + "\${rd2Version}"
        $rdDockableToolWindow.InProcServer32Path + "\${rd2Version}"
    )
}

function Get-RegistryKeyModels($rdHKLMKeys){
    
    $rd3Version = Get-Rd3Version
    $registryKeyModels = @()
    foreach ($k in $rdHKLMKeys){
        $registryKeyModels += `
            (New-RegistryKeyModelFromRegistryKey `
                ($k + "\InProcServer32\${rd3Version}"))
    }

    $registryKeyModels
}

function Remove-RD3RegistryKeys($regKeys){
    Write-Verbose "Removing RD3 HKey_LOCAL_MACHINE registry keys..."
    Remove-RegistryKeys $regKeys
}

function Export-RD3RegistryValues($registryKeyModels, $exportPath = $null){

    Write-Verbose "Saving current RD3 HKey_LOCAL_MACHINE registry values..."
    
    $filepath = Get-CachedRD3KeyValuesFilepath $exportPath

    ($registryKeyModels | ConvertTo-Json) | Out-File  $filepath
}

function Get-Rd3HKLMPaths($targetKey, $scriptVersion = $null){
    
    $versionToken = Get-Rd3Version
    if ($null -ne $scriptVersion){
        $versionToken = $scriptVersion
    }

    $baseTargetKey = $rdExtension.HKLMPath
    if (Test-ContainsDockableToolWindowClsid $targetKey){
        $baseTargetKey = $rdDockableToolWindow.HKLMPath
    }

    @(
        $baseTargetKey
        ($baseTargetKey + "\InProcServer32")
        ($baseTargetKey + "\InProcServer32\" + $versionToken)
    )
}

function New-RdRegistryKeyModel($registryKey = $null, $properties = @{}){
    @{
        "KeyPath" = $registryKey
        "IsRDRegistryKeyModel" = $True
        'Properties' = $properties
    }
}

function New-RegistryKeyModelFromRegistryKey($registryKey){

    $properties = @{}
    if (Test-Path $registryKey){
        $rdRegistryKeyPropertyNames | ForEach-Object {
            $properties.Add($_, (Get-RegistryPropertyValue $registryKey $_))}
    }

    New-RdRegistryKeyModel $registryKey $properties
}

function New-RegistryKeyModelsFromFile($filePath = $null){
    
    $results = $null
    $modelsFilepath = Get-CachedRD3KeyValuesFilepath $filepath

    $content = Get-Content $modelsFilepath
    if ($null -eq $content -or ($content.Trim() -eq "")){
        $results
        return
    }
    
    try {
        $jsonObjs = $content | ConvertFrom-Json
    }
    catch {
        $results
        return
    }

    $results = @()
    foreach ($obj in $jsonObjs){
        $model = (Convert-JsonToRegistryKeyModel $obj)
        if ($null -eq $model){
            $results = $null
            $results
            return
        } else {
            $results += $model
        }
    }
    $results
}

function Convert-JsonToRegistryKeyModel($jsonObject){
    #https://stackoverflow.com/questions/3740128/pscustomobject-to-hashtable
    $props = @{}
    $jsonObject.Properties.psobject.properties | ForEach-Object { $props[$_.Name] = $_.Value }  
    
    $model = New-RdRegistryKeyModel $jsonObject.KeyPath $props

    if (Test-IsValidRegistryKeyModel $model){
        $model 
        return
    }
    $null
}

function Get-InProcServer32PathFromKey($key){
    $target = "InProcServer32"
    $key.Substring(0, $key.IndexOf($target) + $target.Length)
}

function Get-Rd3VersionFromRd3RegistryKey($key){
    $target = "InProcServer32\3."
    $key.Substring($key.IndexOf($target) + ("InProcServer32\").Length)
}


function Test-IsValidRegistryKeyModel($model){
 
    if ($null -eq $model){
        $False
        return
    } elseif  (-not ((Test-HasAllKeys $model) -and (Test-AllKeysNotNull $model))){
        $False
        return
    } elseif (-not ((Test-HasAllProperties $model) -and (Test-AllPropertiesNotNull $model))){
        $False
        return
    }

    $True
 }
 
function Test-ContainsExtensionClsid($token){
    ($token -like "*${extensionClsid}*")
}

function Test-ContainsDockableToolWindowClsid($token){
    ($token -like "*${dockableToolWindowClsid}*")
}

function Test-IsRDClsidOfInterest($clsid){
    (Test-ContainsExtensionClsid $clsid) -or `
        (Test-ContainsDockableToolWindowClsid $clsid)
}

function Test-HasAllKeys($model){
    (@("KeyPath", "Properties") | 
        Where-Object {-not $model.ContainsKey($_)}).Count -eq 0       
}

function Test-AllKeysNotNull($model){
    (@("KeyPath", "Properties") | 
        Where-Object {$null -eq $model[$_]}).Count -eq 0
}

function Test-HasAllProperties($model){
    ($rdRegistryKeyPropertyNames | 
        Where-Object {-not $model.Properties.ContainsKey($_)}).Count -eq 0       
}

function Test-AllPropertiesNotNull($model){
    ($rdRegistryKeyPropertyNames | 
        Where-Object {$null -eq $model.Properties[$_]}).Count -eq 0       
}

function Get-Rd2Version(){
    $script:toggleModel.Rd2Version
}

function Get-Rd3Version(){
    $script:toggleModel.Rd3Version
}

function Get-ActiveVersion(){
    $script:toggleModel.ActiveVersion
}

function Get-InActiveVersion(){
    $InactiveVersion = Get-Rd2Version
    if (Test-IsConfiguredForRD2){
        $InactiveVersion = Get-Rd3Version
    }
    $InactiveVersion
}

function New-ViewModel(){
    param(
        [Parameter(Mandatory = $True)]
        [string]$activeVersion,
        [Parameter(Mandatory = $true)]
        [string]$inactiveVersion,
        [Parameter(Mandatory = $True)]
        [string]$rd2Version,
        [Parameter(Mandatory = $true)]
        [string]$rd3Version
    )
    
    $vm = @{
        NV = $inactiveVersion
        CV = $activeVersion
        ErrorVersion = ""
        DisplayError = $false
        Preview = $true
        ErrMsg =  @{Primary = "";  Fix = ""}
    }

    if ($rd2Version -eq "TBD"){
        $vm.ErrorVersion="2"
    }

    if ($rd3Version -eq "TBD"){
        $vm.ErrorVersion="3"
    }

    if (($vm.CV) -eq "TBD"){
        $vm.ErrorVersion="NO RD VERSION REGISTERED"
    }

    $vm.HasDisplayError = $vm.ErrorVersion.Length -ne 0

    $vm.ErrMsg = Get-ViewErrorMessaging $vm

    $vm
}

function Get-ViewErrorMessaging($vm){
    
    $errMsg = @{Primary = "";  Fix = ""}

    if ($vm.ErrorVersion.StartsWith("2")){
        $errMsg.Primary = "Rubberduck v2 Registry Entries not found";
        $errMsg.Fix = "Please Build or Install Rubberduck v2"
    } elseif ($vm.ErrorVersion.StartsWith("3")){
        $errMsg.Primary = "Unable to locate v3 setup data"
        $errMsg.Fix = "To switch to v3, please Clean\Build or Install Rubberduck v3"
    } elseif ($vm.ErrorVersion.StartsWith("NO RD VERSION")){
        $errMsg.Primary = "Rubberduck version(s) data not found"
        $errMsg.Fix = "Please Install or Build the Rubberduck version of interest"
    }
    $errMsg
}

function Test-IsConfiguredForRD2(){    
    (Get-ActiveVersion).StartsWith("2.")
}

function Test-IsConfiguredForRD3(){
    (Get-ActiveVersion).StartsWith("3.")
}

function Test-FileIsLocked($file){
    #https://stackoverflow.com/questions/24992681/powershell-check-if-a-file-is-locked
    try { 
        [IO.File]::OpenWrite($file).close()
        $false 
    }
    catch {
        $true
    }    
}

function Get-SubKeyNames($hiveName, $subKeyPath){
    $hiveKeys = @()
    try 
    {
        $reg = [Microsoft.Win32.RegistryKey]::OpenRemoteBaseKey(`
            [Microsoft.Win32.RegistryHive]($hiveName), $env:COMPUTERNAME)

        $subKey = $reg.OpenSubKey($subKeyPath)

        $subKeyNames = $subKey | 
            Select-Object Name, @{
                label='SubKeys'
                expression={$_.GetSubKeyNames()}
            }
            
        $hiveKeys = $subKeyNames.SubKeys
    }
    catch {
        $hiveKeys = @()
    }
    finally {
        $subKey.Dispose()
    }

    $hiveKeys
}

function Remove-RegistryKeys(){
    [cmdletbinding(SupportsShouldProcess = $True)]
    param (
        [Parameter(Mandatory = $True)]
        [string[]]$keyPaths
    )

    foreach ($key in $keyPaths | Where-Object {Test-Path $_}){ 
        if ($PSCmdlet.ShouldProcess($key, "Remove-Item")){
            Write-Verbose ("Deleting Registry Key: " + $key)
            Remove-Item -Path $key -Recurse
        }
    }      
}
 
function Add-RegistryKeys(){
    [cmdletbinding(SupportsShouldProcess = $True)]
    param (
        [Parameter (Mandatory = $true)]
        [Object] $pathsToCreate
    )

    foreach ($kp in $pathsToCreate){
        if ($PSCmdlet.ShouldProcess($kp, "New-Item")){
            New-Item -Path $kp
            Write-Verbose ("Added Registry Key: " + $kp)
        }
    }
}

function Set-RegistryKeyProperties(){
    [cmdletbinding(SupportsShouldProcess = $True)]
    param (
        [Parameter (Mandatory = $true)]
        [Object] $registryKeyModel,
        [Parameter (Mandatory = $true)]
        [string] $valueName
    )

    $valueData = $registryKeyModel.Properties[$valueName]

    $shouldProcessMsg = "'" + $valueName + "' = '" + $valueData + "'"
    
    if ($PSCmdlet.ShouldProcess($shouldProcessMsg, "Set-ItemProperty")){
        Set-ItemProperty -Path $registryKeyModel.KeyPath -Name $valueName -Value $valueData
        Write-Verbose ("Set Property: '" + $shouldProcessMsg + "'")
    }
}

function Get-RegistryPropertyValue($registryKey, $propertyName){
    (Get-ItemPropertyValue -Path $registryKey -Name $propertyName)
}

function New-WindowFromXamlBlob($xamlBlob){
    [xml] $viewAsXML = $xamlBlob
    
    try {
        $nodeReader= (New-Object System.Xml.XmlNodeReader $viewAsXML)
        $uiWIndow = [Windows.Markup.XamlReader]::load($nodeReader)
    }
    catch {
        Write-Output $_
        break;
    }
    $uiWIndow
}

function Set-UIButtonVersionLabels($uiWindow, $viewModel){
    $currentV = Convert-VersionToXVersion $viewModel.CV
    $nextV = Convert-VersionToXVersion $viewModel.NV

    $switchToVersionLabel = $uiWindow.findName("SwitchToVButtonLabel")
    $switchToVersionLabel.Text = $nextV

    $keepCurrentV = $uiWindow.FindName("CurrentVButtonLabel")
    $keepCurrentV.Text = $currentV

    $previewVersion = $uiWindow.FindName("PreviewVButtonLabel")
    $previewVersion.Text = $nextV
}

function Convert-VersionToXVersion($versionNumber){

    $result = "TBD"
    
    try {
        $idxLastDot = $versionNumber.LastIndexOf(".")
        $V = "v" +  $versionNumber.Substring(0, $idxLastDot) + ".x"
        if ($V.StartsWith("vv")){
            $V = $V.Substring(1)
        }
        $result = $V
    }
    catch {}

    $result
}
################################ Start of Script ################################

function ShowUI(){
    param (
        [Parameter(Mandatory = $True)]
        [hashtable]$viewModel
    )

    $theXaml = 
@"
<?xml version="1.0" encoding="utf-8"?><Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
Title="Rubberduck Version Toggling Script" Height="450" Width="600" SizeToContent="WidthAndHeight">
<Window.Resources><Style TargetType="Button"><Setter Property="Padding" Value="10, 5, 10, 5" /><Setter Property="FontSize" Value="20" /><Setter Property="VerticalAlignment" Value="Top" /><Setter Property="FontWeight" Value="Bold" /></Style></Window.Resources>
<DockPanel HorizontalAlignment="Center"><StackPanel HorizontalAlignment="Center" DockPanel.Dock="Top">
<StackPanel HorizontalAlignment="Center" Margin="20,20,20,30"><TextBlock FontSize="20.0" HorizontalAlignment="Center"><Bold>Toggle Rubberduck Registry Setup</Bold></TextBlock><Button Name="KeepCurrent" Margin="10,15,3,10" HorizontalAlignment="Center"><StackPanel><TextBlock FontSize="12.0" HorizontalAlignment="Center"><Bold>(Current Setup)</Bold></TextBlock><TextBlock Name="CurrentVButtonLabel" FontSize="20.0" HorizontalAlignment="Center"><Bold>v2.5.9.x</Bold></TextBlock></StackPanel><Button.ToolTip><TextBlock Margin="5,0,5,0" Width="300" TextWrapping="WrapWithOverflow"><Bold><Italic>DOES NOT modify the Registry</Italic></Bold><LineBreak /><LineBreak />&#xD;
Dismisses the dialog box&#xD;
</TextBlock></Button.ToolTip></Button><StackPanel Name="ShowToggleButtons"><Button Name="SwitchVersion" Margin="10,15,3,0" HorizontalAlignment="Center"><StackPanel><TextBlock FontSize="12.0" HorizontalAlignment="Center"><Bold>(Switch to)</Bold></TextBlock><TextBlock Name="SwitchToVButtonLabel" FontSize="16.0" HorizontalAlignment="Center"><Bold>v3.0.0.x</Bold></TextBlock></StackPanel><Button.ToolTip><StackPanel><TextBlock Margin="5,0,5,0" Width="300" TextWrapping="WrapWithOverflow"><Bold><Italic>MODIFIES THE REGISTRY</Italic></Bold><LineBreak /><LineBreak />Enables the indicated version&#xD;
<LineBreak /></TextBlock></StackPanel></Button.ToolTip></Button><Button Name="Preview" Margin="10,40,3,0" HorizontalAlignment="Center" ToolTipService.ShowDuration="15000"><StackPanel><TextBlock FontSize="12.0" HorizontalAlignment="Center"><Italic><Bold>Preview Registry changes for</Bold></Italic></TextBlock><TextBlock Name="PreviewVButtonLabel" FontSize="12.0" HorizontalAlignment="Center"><Bold>v3.0.0.x</Bold></TextBlock></StackPanel><Button.ToolTip><TextBlock Margin="5,0,5,0" Width="300" TextWrapping="WrapWithOverflow"><Bold><Italic>DOES NOT modify the Registry</Italic></Bold><LineBreak /><LineBreak />&#xD;
Posts messages to the Powershell &#xD;
session window providing a detailed narrative of changes &#xD;
that <Italic>would have</Italic> been made. &#xD;
<LineBreak /><LineBreak />  &#xD;
Executes the script declaring both the '-Verbose' and '-WhatIf' &#xD;
switch parameters.&#xD;
</TextBlock></Button.ToolTip></Button></StackPanel><StackPanel Name="ShowError" Visibility="Collapsed"><TextBlock Name="ErrorMessage" Margin="0, 10, 0, 0" HorizontalAlignment="Center" FontSize="20" TextWrapping="WrapWithOverflow" Foreground="Red"><Bold>This is the error message</Bold><LineBreak /></TextBlock><TextBlock Name="FixMessage" HorizontalAlignment="Center" TextWrapping="WrapWithOverflow"><Bold>This is how you fix it!!</Bold></TextBlock></StackPanel></StackPanel></StackPanel><Border Background="Gray" BorderBrush="SteelBlue" BorderThickness="3,5,3,5"><StackPanel><Button Name="Cancel" IsCancel="True" FontSize="12" HorizontalAlignment="Right" DockPanel.Dock="Bottom" Padding="10, 5, 10, 5" Margin="2,5,10,5">Exit&#xD;
</Button></StackPanel></Border></DockPanel></Window>
"@    
    
    Add-Type -Assembly PresentationFrameWork

    trap {
        $viewModel.Action = "Cancel" 
        $viewModel 
        exit
    }

    $mode = 
        [System.Threading.Thread]::CurrentThread.ApartmentState
        if ($mode -ne "STA"){
            $m = "This  script can only be run when PS is started with the -sta switch."
            throw $m
        }

    $viewModel.Action = "Cancel" 

    $uiWindow = New-WindowFromXamlBlob $theXaml

    Set-UIButtonVersionLabels $uiWindow $viewModel

    $viewModel.Preview = $false

    $keepCurrent = $uiWindow.FindName("KeepCurrent")
    $keepCurrent.add_Click({
        $viewModel.Action = "Cancel" 
        $uiWindow.Close()
    })

    $dismiss = $uiWindow.FindName("Cancel")
    $dismiss.add_Click({
        $viewModel.Action = "Cancel" 
        $uiWindow.Close()
    })

    if ($viewModel.HasDisplayError){
        $btnPanel = $uiWindow.FindName("ShowToggleButtons")
        $btnPanel.Visibility = "Collapsed"

        $errPanel = $uiWindow.FindName("ShowError")
        $errPanel.Visibility = "Visible"

        $errMsg = $uiWindow.FindName("ErrorMessage")
        $errMsg.Text = $viewModel.ErrMsg.Primary
        $theFix = $uiWindow.FindName("FixMessage")
        $theFix.Text = $viewModel.ErrMsg.Fix

    } else {
        $switchVersion = $uiWindow.FindName("SwitchVersion")
        $switchVersion.add_Click({
            $viewModel.Action = "Switch" 
            $viewModel.Preview = $false
            $uiWindow.Close()
        })
    
        $preview = $uiWindow.FindName("Preview")
        $preview.add_Click({
            $viewModel.Action = "Switch" 
            $viewModel.Preview = $true
            $uiWindow.Close()
        })
    }

    if ($uiWindow.ShowDialog()){}

    $viewModel
}

Initialize-ScriptScopeFields

#If the user did not provide CL a parameter, provide the UI
if (-not $PSBoundParameters.ContainsKey('requestedOp')){

    $vm = ShowUI (
        New-ViewModel (Get-ActiveVersion) (Get-InActiveVersion) (Get-Rd2Version) (Get-RD3Version)
    )

    try {
        $VerbosePreference = "Continue"
        $WhatIfPreference = $vm.Preview
    
        if($vm.Action -eq "Cancel"){
            Write-Verbose ("Current Registry Configuration: Rubberduck " + (Convert-VersionToXVersion($vm.CV)))
            $VerbosePreference = "SilentlyContinue"
            $WhatIfPreference = $false
            exit
        }
    
        if (Test-IsConfiguredForRD2){
            Restore-RubberduckV3 $script:rd3ModelsFromFile
        }
    
        if (Test-IsConfiguredForRD3){
            $rd3HKLMKeys = Get-HKLMKeysOfInterest( Get-SubKeyNames "LocalMachine" "SOFTWARE\Classes\CLSID")
            Restore-RubberduckV2 $rd3HKLMKeys
        }
    }
    finally {
        $VerbosePreference = "SilentlyContinue"
        $WhatIfPreference = $false
    }
    exit
}

if ($requestedOp -eq "CurrentVersion"){
    
    $activeVersion = Get-ActiveVersion

    if ($activeVersion -eq "TBD"){ 
        "Unknown: Rubberduck2 and/or Rubberduck3 may not be installed"
    } else {
        "Current Registry configuration: Rubberduck Version " + (Convert-VersionToXVersion $activeVersion)
    }
    exit
}

$targetFromRequestedOpIsRD2 = ($requestedOp -like "*2")

$validationResults = Test-CanProcessToTargetVersion $targetFromRequestedOpIsRD2

if (-not $validationResults.CanProcess){
    $validationResults.Message
    exit
}

if ($targetFromRequestedOpIsRD2){
    $rd3HKLMKeys = Get-HKLMKeysOfInterest (Get-SubKeyNames "LocalMachine" "SOFTWARE\Classes\CLSID")
    Restore-RubberduckV2 $rd3HKLMKeys

} else {
    if ($script:toggleModel.Rd3Version -eq "TBD"){
        $keyPaths = $validationResults.ModelsFromFile.KeyPath
        $script:toggleModel.Rd3Version = Get-Rd3VersionFromRd3RegistryKey($keyPaths[0])
    }
    Restore-RubberduckV3 $validationResults.ModelsFromFile
}

<#
.SYNOPSIS
Modifies the registry to switch between the Rubberduck 3 and Rubberduck 2 VBIDE Add-Ins.
.DESCRIPTION
Modifies the registry to switch between the Rubberduck 3 and Rubberduck 2 VBIDE Add-Ins.

Why: 
RD3 re-uses RD2 CLSIDs.  Consequently, the two Add-Ins cannot co-exist in memory.  
When the RD3 solution is built, the build process introduces new keys to the LocalMachine (HKLM) hive.  
Re-building RD2 does not configure the registry to enable RD2 once the RD3  HKLM keys have been introduced.

*** The script must be invoked from a PowerShell session with Administrator privileges ***

(The commands below assume a Powershell session has been opened in the folder containing the Switch-RDVersion.ps1 script)

USING THE GUI:
The easiest way to use the script is to invoke the script without any parameters as follows:
PS> .\Switch-RDVersion.ps1

The above invocation results in a pop-up dialog with 3 buttons (plus an Exit button):
Top button - indicates the currently configured RD Add-In.  Clicking this button does nothing except dismissing the window.
Middle button - indicates the available alternate Add-In.  Clicking this button modifies the registry.
Bottom button - Provides a preview/test of script actions when the Middle button is clicked.  The registry is not modified.

The Bottom button is equivalent to using the -WhatIf parameter in the CLI examples below.
Tooltip information is available for each button

USING COMMAND LINE INTERFACE PARAMETERS:

To run the script to see what it does WITHOUT making changes, enter the following:

Setup for RD2:    PS> .\Switch-RDVersion.ps1 2 -Verbose -WhatIf
Setup for RD3:    PS> .\Switch-RDVersion.ps1 3 -Verbose -WhatIf

****************************************************************************************
Note: Absence of the -WhatIf switch parameter from the above expressions will enable the 
script to make changes to the registry.
*****************************************************************************************

To see the currently active version: 
PS> .\Switch-RDVersion.ps1 CurrentVersion

Alternatively, cleaning and re-building the RD3 solution will also modify the Registry, enabling the RD3 Add-In  

There is more content available using the -examples, -detailed, or -full switches as indicated in REMARKS
.NOTES
Setting up for Rubberduck Version 2:

1. Deletes two HKLM registry keys (including their children) introduced by building RD3: 
    HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\{69E0F697-43F0-3B33-B105-9B8188A6F040} (Rubberduck.Extension)
    HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\{69E0F699-43F0-3B33-B105-9B8188A6F040} (Rubberuck.DockableToolWindow)
    Note: before deleting, the script saves values contained in these keys to a file (RD3HKLMValues.json) located in 
        the same folder as the Rubberduck.config file.

2. Modifies the values of:
    HKEY_CLASSES_ROOT\CLSID\{69E0F697-43F0-3B33-B105-9B8188A6F040}\InProcServer32 and
    HKEY_CLASSES_ROOT\CLSID\{69E0F699-43F0-3B33-B105-9B8188A6F040}\InProcServer32

    to match the values contained in 

    HKEY_CLASSES_ROOT\CLSID\{69E0F697-43F0-3B33-B105-9B8188A6F040}\InProcServer32\2.X.Y.Z and 
    HKEY_CLASSES_ROOT\CLSID\{69E0F699-43F0-3B33-B105-9B8188A6F040}\InProcServer32\2.X.Y.Z respectively

Setting up for Rubberduck Version 3:

1. Re-creates the RD3 HKLM registry keys and their children based on data contained in 
    RD3HKLMValues.json.  The RD3HKLMValues.json file is generated/refreshed when the script is run 
    to update from RD3 to RD2.

2. Modifies the following registry keys with RD3 values

    HKEY_CLASSES_ROOT\CLSID\{69E0F697-43F0-3B33-B105-9B8188A6F040}\InProcServer32 and
    HKEY_CLASSES_ROOT\CLSID\{69E0F699-43F0-3B33-B105-9B8188A6F040}\InProcServer32


3. If the RD3HKLMValues.json file is missing or cannot be used for any reason, the user is prompted to 
rebuild the Rubbberduck3 solution.  Rebuilding the solution results in the same registry changes
as described in steps 1 and 2 above. 

Note: Closing and re-opening the Registry Editor application will show that adding the two HKLM keys 
in #1 above, resulted in the creation of:
    HKEY_CLASSES_ROOT\CLSID\{69E0F697-43F0-3B33-B105-9B8188A6F040}\InProcServer32\3.X.Y.Z and 
    HKEY_CLASSES_ROOT\CLSID\{69E0F699-43F0-3B33-B105-9B8188A6F040}\InProcServer32\3.X.Y.Z

.EXAMPLE
...\Switch-RDVersion.ps1
Provides a dialog window to support switching RD Add-In versions

.EXAMPLE
...\Switch-RDVersion.ps1 CurrentVersion
Script output identifies the Rubberduck version currently setup in the registry

.EXAMPLE
...\Switch-RDVersion.ps1 2 -WhatIf
See operations that 'would have' taken place to setup for RD2

.EXAMPLE
...\Switch-RDVersion.ps1 2 -Verbose -WhatIf
See operations that 'would have' taken place to setup for RD2 with the most descriptive content

.EXAMPLE
...\Switch-RDVersion.ps1 rd2 -Verbose
Allow the script to make changes to the registry to setup RD2, providing a narrative of the process

.EXAMPLE
.\Switch-RDVersion.ps1 3 -Verbose -WhatIf
See operations that 'would have' taken place to setup for RD3 with the most descriptive content

.EXAMPLE
...\Switch-RDVersion.ps1 rd3 -Verbose -WhatIf
See operations that 'would have' taken place to setup for RD3 with the most descriptive content

.EXAMPLE
...\Switch-RDVersion.ps1 rd3
Allow the script to make changes to the registry to setup RD3 with limited descriptive content
#>


