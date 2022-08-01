﻿using System.Management.Automation;

namespace Utility.Common.Helper {
   public class UninstallHelper {

      public static void Run(string applicationName) {
         using var ps = PowerShell.Create();
         ps.AddScript(Script.Replace("$applicationName", applicationName)).Invoke();
      }

      private static readonly string Script = @"function Get-InstalledSoftware {
#https://4sysops.com/archives/find-the-product-guid-of-installed-software-with-powershell/
    <#
    .SYNOPSIS
        Retrieves a list of all software installed
    .EXAMPLE
        Get-InstalledSoftware
        
        This example retrieves all software installed on the local computer
    .PARAMETER Name
        The software title you'd like to limit the query to.
    #>
    [OutputType([System.Management.Automation.PSObject])]
    [CmdletBinding()]
    param (
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [string]$Name
    )
    $UninstallKeys = (""HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall"", ""HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"")
    $null = New-PSDrive -Name HKU -PSProvider Registry -Root Registry::HKEY_USERS
    $UninstallKeys += Get-ChildItem HKU: -ErrorAction SilentlyContinue | Where-Object { $_.Name -match 'S-\d-\d+-(\d+-){1,14}\d+$' } | ForEach-Object { ""HKU:\$($_.PSChildName)\Software\Microsoft\Windows\CurrentVersion\Uninstall"" }
    if (-not $UninstallKeys) {
        Write-Verbose -Message 'No software registry keys found'
    } else {
        foreach ($UninstallKey in $UninstallKeys) {
            if ($PSBoundParameters.ContainsKey('Name')) {
                $WhereBlock = { ($_.PSChildName -match '^{[A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}}$') -and ($_.GetValue('DisplayName') -like ""$Name"") }
            } else {
                $WhereBlock = { ($_.PSChildName -match '^{[A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}}$') -and ($_.GetValue('DisplayName')) }
            }
            $gciParams = @{
                Path        = $UninstallKey
                ErrorAction = 'SilentlyContinue'
            }
                     $selectProperties = @(
                @{n='GUID'; e={$_.PSChildName}}, 
                @{n='Name'; e={$_.GetValue('DisplayName')}}
            )
            Get-ChildItem @gciParams | Where $WhereBlock | Select-Object -Property $selectProperties
        }
    }
}

function Get-UninstallSoftware
# Quietly removes all installed software with the name $applicationName
{
    foreach($v in InstalledSoftware(""$applicationName""))
{
       Write-Output $v.GUID
       Start-Process  ""C:\Windows\System32\msiexec.exe"" -ArgumentList ""/x $($v.GUID) /quiet /noreboot"" -Wait
    }
}
UninstallSoftware
";
   }
}
