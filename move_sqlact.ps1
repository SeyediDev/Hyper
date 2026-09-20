$sourceFile = "E:\Projects\Hyper\Backend\src\Core\Hyper.Domain\Entities\Database\SqlServerEntities.cs"
$targetFile = "E:\Projects\Hyper\Backend\src\Core\Hyper.Domain\Entities\Database\SqlActServerEntities.cs"

# Read the source file
$lines = Get-Content $sourceFile -Encoding UTF8

# Find all SqlAct class start lines
$classStarts = @()
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match '^public sealed class SqlAct\w+') {
        $classStarts += $i
    }
}

# Extract each class and track line ranges to remove
$extractedClasses = @()
$rangesToRemove = @()

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
        $rangesToRemove += @($attrStart, $endLine)
    }
}

# Write extracted classes to verify
$extractedClasses -join "`n`n" | Set-Content "E:\temp\extracted_classes.cs" -Encoding UTF8
Write-Host "Extracted $($extractedClasses.Count) classes"

# Now remove from source file (process ranges in reverse to keep line numbers valid)
# Sort ranges by start line descending
$rangesToRemove = $rangesToRemove | Sort-Object { $_[0] } -Descending

$newLines = $lines
foreach ($range in $rangesToRemove) {
    $start = $range[0]
    $end = $range[1]
    # Remove the range (inclusive)
    $newLines = $newLines[0..($start-1)] + $newLines[($end+1)..($newLines.Count-1)]
}

# Clean up any excessive blank lines (more than 2 consecutive)
$cleanLines = @()
$blankCount = 0
foreach ($line in $newLines) {
    if ($line.Trim() -eq '') {
        $blankCount++
        if ($blankCount -le 2) {
            $cleanLines += $line
        }
    } else {
        $blankCount = 0
        $cleanLines += $line
    }
}

# Write back to source file
$cleanLines | Set-Content $sourceFile -Encoding UTF8
Write-Host "Updated source file: removed $($rangesToRemove.Count) class ranges"

# Now append to target file
$targetContent = Get-Content $targetFile -Encoding UTF8 -Raw
$classesToAppend = $extractedClasses -join "`n`n"
$newTargetContent = $targetContent.TrimEnd() + "`n`n" + $classesToAppend + "`n"
$newTargetContent | Set-Content $targetFile -Encoding UTF8
Write-Host "Updated target file: appended $($extractedClasses.Count) classes"

Write-Host "Done!"