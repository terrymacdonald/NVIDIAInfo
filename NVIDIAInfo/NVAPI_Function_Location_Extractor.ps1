# Written by Soroush Falahati
$filename = "..\..\..\video-driver-sdks\NVIDIA\R510-developer\amd64\nvapi64.lib"
$dumpbinAddress = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Tools\MSVC\14.29.30133\bin\Hostx64\x64\dumpbin.exe"
$dumpbinParameter = "/DISASM $filename"
Start-Process $dumpbinAddress $dumpbinParameter -Wait -WindowStyle Hidden -RedirectStandardOutput "$filename.asm"
$content = Get-Content "$filename.asm"
$functionName = ""
foreach ($line in $content)
{
	if (!$line)
	{
		if ($functionName -ne "") {			
			#Write-Host "$functionName = FAILED,"
		}
		$functionName = ""
		continue;
	}
	if ($functionName -eq "" -and $line.EndsWith(":") -and ($line.StartsWith("NvAPI_")))
	{
		$functionName = $line.TrimEnd(':')
		continue;
	}
	$leadingPattern = "ecx,"
	if ($functionName -ne "" -and $line.Contains($leadingPattern) -and $line.EndsWith("h"))
	{
		$functionAddress = $line.Substring($line.IndexOf($leadingPattern) + $leadingPattern.Length).TrimEnd('h')
		$functionAddressNumberic = 0
		if ([int32]::TryParse($functionAddress, 
			[System.Globalization.NumberStyles]::HexNumber, 
			[System.Globalization.CultureInfo]::CurrentCulture, 
			[ref] $functionAddressNumberic))
		{
			$functionName = $functionName.Replace("NvAPI_","NvId_")
			$functionAddress = $functionAddressNumberic.ToString("X")
			Write-Host "private const uint $functionName = 0x$functionAddress;"
			$functionName = ""
			continue;
		}
	}
}
Remove-Item "$filename.asm"
# SIG # Begin signature block
# MIIFUgYJKoZIhvcNAQcCoIIFQzCCBT8CAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUc/cY7aXldIV8/EUFpc6n32X5
# emGgggLwMIIC7DCCAdSgAwIBAgIQcwA3FQBkOqxNa3bZxB2k1jANBgkqhkiG9w0B
# AQsFADAXMRUwEwYDVQQDEwxMaXR0bGVCaXRCaWcwHhcNMjAxMjE4MDg1NTQ0WhcN
# MjExMjE4MTQ1NTQ0WjAXMRUwEwYDVQQDEwxMaXR0bGVCaXRCaWcwggEiMA0GCSqG
# SIb3DQEBAQUAA4IBDwAwggEKAoIBAQDYPh2ySpyU7dqzi3CK0UQ73ZfRRurqrmxi
# rjO+fRvq/sl/zMLm463IxgHLLPWZx0APJbeS/iWqVhgyOnpwe/LoAfeIWSCQDdYR
# ssO68wRG1h8hi13NkAXt3Mr24PSzvC2FMXzp/UzIQanLPgF9KouPuAb34v4cdVb4
# cEd4Qmr5R8/gC28/bA7RBmgKzIQ4abu9ip0XhL7nQe8dh8LEGvRt4DzBngXoTDhn
# wwSsAUhhLsArjtuMDoV4Kwt+P13RRcR5Ugzr4bLDtI9WYom50AwNYxjTD9HjbSR1
# cTdCbNOovkl0a4EFpPTqk+RalPj4MkXP6bpB3SNM4bYsqbeKyT1FAgMBAAGjNDAy
# MAwGA1UdEwEB/wQCMAAwIgYDVR0lAQH/BBgwFgYIKwYBBQUHAwMGCisGAQQBgjdU
# AwEwDQYJKoZIhvcNAQELBQADggEBALo9BFEQKuwYOXOOXwXunWsCG27cNvc/7VoE
# A5QPbi1XyYEg7O6LAlIvmR02Ql3wm+J9tSjuMsUTLSXoYpbDNYp1uS+ldMYuEiBF
# qQnDdJj3AfSLfHZvSqWpKAvp/ObbcDn937IU2sBPuL9rlFKwn9v7boQX2PWZCF1u
# axXa9k6lbzaQ+6cOSRqrV4Ng39T8yQGalvzxX+K0YNApMOfD8XyoOZtcksk3pEHf
# sJX9WAbB2AtulAIdH+C4f6U1ERz06yxUxdIPLquDLvc0GwEl3jzgMoJ8qljT7ej8
# BYJChAPQqZjJgxgD1CB4h0QIxmeRGiHnyLBHzsVsiQjzkiYFHV0xggHMMIIByAIB
# ATArMBcxFTATBgNVBAMTDExpdHRsZUJpdEJpZwIQcwA3FQBkOqxNa3bZxB2k1jAJ
# BgUrDgMCGgUAoHgwGAYKKwYBBAGCNwIBDDEKMAigAoAAoQKAADAZBgkqhkiG9w0B
# CQMxDAYKKwYBBAGCNwIBBDAcBgorBgEEAYI3AgELMQ4wDAYKKwYBBAGCNwIBFTAj
# BgkqhkiG9w0BCQQxFgQUYigaTaB40vd+Y0ndIETpl5t2O2kwDQYJKoZIhvcNAQEB
# BQAEggEAk7CkFMF88np389uZR/OiosAUJNMPT7i+Iwkvj/xbIz5znY2G6n1IWnKg
# gtuhMjkRASEaSlJlgH3igGSYTzfAyWYQc/tNJJ1X5cuJOOO1iRik00dWDF/HFxa3
# BzFBmdmUnbtcCaPProKNI5yxAVwHydPQIp84vQ3FQ6OwYBX3TXjfm1PsKhVD7/uR
# VXJpmwLeHxGA7TWjtPh2dZ+YFL3m9fc0NuS5VYsvLzqQabxGqPEjmfYcsmUghsH6
# ZGROjtffQXTXzWO0T70oJmKTocEv2gqDEpIQphfq0dNsh4fwojmnxoT996IEnRCR
# DJr23btE7yDo0MKRrGlJbwsyIXSydg==
# SIG # End signature block
