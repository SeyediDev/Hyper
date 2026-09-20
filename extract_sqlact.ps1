$sourceFile = "E:\Projects\Hyper\Backend\src\Core\Hyper.Domain\Entities\Database\SqlServerEntities.cs"
$targetFile = "E:\Projects\Hyper\Backend\src\Core\Hyper.Domain\Entities\Database\SqlActServerEntities.cs"

# Read the source file
$lines = Get-Content $sourceFile

# Find all SqlAct class start lines
$classStarts = @()
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match '^public sealed class SqlAct\w+') {
        $classStarts += $i
    }
}

# Extract each class (from attributes before to closing brace)
$extractedClasses = @()

for ($idx = 0; $idx -lt $classStarts.Count; $idx++) {
    $startLine = $classStarts[$idx]
    
    # Find the start of attributes (go backwards)
    $attrStart = $startLine
    for ($j = $startLine - 1; $j -ge 0; $j--) {
        if ($lines[$j] -match '^\[DisplayName' -or $lines[$j] -match '^\[DbMap') {
            $attrStart = $j
        } else {
            break
        }
    }
    
    # Find the end of the class (matching braces)
    $braceCount = 0
    $endLine = -1
    $foundClass = $false
    for ($j = $startLine; $j -lt $lines.Count; $j++) {
        $line = $lines[$j]
        if ($line -match 'public sealed class SqlAct\w+') {
            $foundClass = $true
        }
        if ($foundClass) {
            $braceCount += ($line -replace '[^{]', '').Length
            $braceCount -= ($line -replace '[^}]', '').Length
            if ($braceCount -eq 0 -and $foundClass) {
                $endLine = $j
                break
            }
        }
    }
    
    if ($endLine -ge 0) {
        $classLines = $lines[$attrStart..$endLine]
        $extractedClasses += $classLines -join "`n"
    }
}

# Write extracted classes to a temp file for verification
$extractedClasses -join "`n`n" | Set-Content "E:\temp\extracted_classes.cs" -Encoding UTF8
Write-Host "Extracted $($extractedClasses.Count) classes"
Write-Host "Saved to E:\temp\extracted_classes.cs"